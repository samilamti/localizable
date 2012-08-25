using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using LanguageFileParsers;
using Localizable.Database;
using Localizable.Models;
using Models;
using ngenstrings;

namespace Localizable.Controllers
{
    public class TranslateController : Controller
    {
        public ActionResult File()
        {
            var model = new UploadModel();
            model.OutputFormats = new List<SelectListItem> { 
                new SelectListItem { Selected = true, Text = "Best guess, based upload", Value = OutputFormat.Unknown.ToString() },
                new SelectListItem { Text = "Android XML files", Value = OutputFormat.AndroidXml.ToString() },
                new SelectListItem { Text = "iOS PList files", Value = OutputFormat.PList.ToString() },
                new SelectListItem { Text = "Microsoft ResX files", Value = OutputFormat.ResX.ToString() },
                new SelectListItem { Text = "iOS Strings files", Value = OutputFormat.Strings.ToString() },
                //new SelectListItem { Text = "iOS Custstom XML files", Value = OutputFormat.Xml.ToString() }
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult DownVoteKey(int id)
        {
            if (!Request.IsAuthenticated)
                return Json(false, JsonRequestBehavior.AllowGet);

            using (var context = new Database.Context())
            {
                var contributor = context.GetTranslator(User);
                if (context.DownvotedKeys.Any(k => k.Key.Id == id && k.Translator.Id == contributor.Id))
                    return Json(false, JsonRequestBehavior.AllowGet);

                var key = context.Keys.Find(id);
                key.DownVotes++;
                if (key.DownVotes > 5)
                {
                    context.Values.Where(v => v.Key.Id == key.Id).ToList().ForEach(v => context.Values.Remove(v));
                    context.DownvotedKeys.Where(dk => dk.Key.Id == key.Id).ToList().ForEach(dk => context.DownvotedKeys.Remove(dk));
                    context.Keys.Remove(key);
                }
                else
                    context.DownvotedKeys.Add(new DownvotedKey { Key = key, Translator = contributor });

                context.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private bool TryGetInputStream(HttpPostedFileBase file, out Stream inputStream)
        {
            inputStream = null;
            if (file != null && file.ContentLength > 0)
                inputStream = file.InputStream;

            if (file == null && Request.InputStream.CanRead)
                inputStream = Request.InputStream;
            
            return inputStream != null;
        }

        [HttpPost]
        public ActionResult Upload(UploadModel model)
        {
            Session["UploadStarted"] = DateTime.UtcNow;

            var addValues = Request.IsAuthenticated && !String.IsNullOrEmpty(model.Language);

            Stream inputStream;
            if (!TryGetInputStream(model.PostedFile, out inputStream))
            {
                ModelState.AddModelError("PostedFile", "Unable to read file");
                return new EmptyResult();
            }

            var fileName = GetFileName(model.PostedFile);
            var tables = MvcApplication.FileParser.ExtractStringTables(inputStream, fileName);

            MvcApplication.TranslationService.AddKeysToDatabase(tables);

            if (addValues)
                MvcApplication.TranslationService.AddTranslationsToDatabase(tables, User, model.Language);

            OutputFormat outputFormat;
            if (!Enum.TryParse<OutputFormat>(model.SelectedOutputFormat, out outputFormat) || outputFormat == OutputFormat.Unknown)
                outputFormat = GuessOutputFormat(fileName);

            if (addValues)
            {
                TempData["Message"] = "Thank you for adding your knowledge to ours!";
                return RedirectToAction("Index", "Home");
            }

            var outputTables = MvcApplication.TranslationService.ProduceOutputTables(tables);
            var outputStream = MvcApplication.CompressionService.ProduceOutputStream(outputTables, outputFormat);

            Session["UploadEnded"] = DateTime.UtcNow;
            return File(outputStream, "application/zip", "translations.zip");
        }

        [HttpGet]
        public JsonResult IsUploadComplete()
        {
            var uploadStartedValue = Session["UploadStarted"];
            if (uploadStartedValue == null) return Json(false); 
            var uploadEndedValue = Session["UploadEnded"];

            var uploadStarted = (DateTime)uploadStartedValue;
            var uploadEnded = (DateTime)uploadEndedValue;

            if (uploadEnded > uploadStarted)
            {
                Session.Remove("UploadStarted");
                Session.Remove("UploadEnded");
                return Json(true);
            }

            return Json(false);
        }

        private OutputFormat GuessOutputFormat(string fileName)
        {
            if(fileName.EndsWith(".plist"))
                return OutputFormat.PList;
            if (fileName.EndsWith(".resx"))
                return OutputFormat.ResX;
            if (fileName.EndsWith(".xml"))
                return OutputFormat.AndroidXml;
            return OutputFormat.Strings;
        }

        private string GetFileName(HttpPostedFileBase qqfile)
        {
            return (qqfile == null ? Request.QueryString["qqfile"] : qqfile.FileName).ToLower();
        }

        public ActionResult Key(int id)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Translate", "Oauth");

            using (var context = new Context())
            {
                var key = context.Keys.Find(id);
                var language = context.GetTranslator(User).Language;
                var translations = context.Values
                    .Where(v => v.Key.Id == key.Id && v.Language == language)
                    .Select(v => v.Value);

                return View(new TranslateKeyModel
                                {
                                    KeyId = key.Id,
                                    Text = key.Key,
                                    Comment = key.Comment,
                                    Language = language,
                                    Translations = translations.ToList()
                                });
            }
        }

        [HttpPost]
        public ActionResult Key(TranslateKeyModel model)
        {
            if (String.IsNullOrEmpty(model.NewTranslation))
            {
                TempData["Message"] = "Please provide your translation before hitting the 'Submit translation' key";
                return RedirectToAction("Key", new { id = model.KeyId });
            }

            using (var context = new Context())
            {
                var translator = context.GetTranslator(User);
                translator.Language = model.Language;

                var key = context.Keys.Find(model.KeyId);
                key.Translations.Add(new Translation(model.Language, model.NewTranslation) { Translator = translator });
                context.SaveChanges();
                TempData["Message"] = "Thank you!";
            }

            return RedirectToAction("Vote", "Home");
        }
    }
}