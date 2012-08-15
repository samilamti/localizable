using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return View();
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
        public ActionResult Upload(HttpPostedFileBase qqfile)
        {
            Stream inputStream;
            if (!TryGetInputStream(qqfile, out inputStream))
            {
                ModelState.AddModelError("qqfile", "Unable to read file");
                return new EmptyResult();
            }

            var tables = new List<LocalizedStringTable>();

            var fileName = GetFileName(qqfile);
            var outputFormat = GetOutputFormat(fileName);
            var sourceLanguage = GetSourceLanguage(fileName);

            if (fileName.EndsWith(".dll"))
            {
                tables.AddRange(ngenstrings.MainClass.ParseDll(inputStream).Values);
            }
            else
            {
                using(var reader = new StreamReader(inputStream))
                {
                    tables.Add(StringsParser.ExtractTable(reader.ReadToEnd()));
                }
            }

            using (var context = new Context())
            {
                // 1. Add all keys to db
                var foundKeys = new List<TranslationKey>();
                foreach(var source in tables)
                foreach (var key in source.Keys)
                {
                    var tkey = context.Keys.FirstOrDefault(tk => tk.Key == key);
                    if (tkey == null)
                    {
                        tkey = new TranslationKey(key, source[key].Comment);
                        context.Keys.Add(tkey);
                    }
                    foundKeys.Add(tkey);
                    var value = source[key];
                    if (!String.IsNullOrEmpty(value.Value) && !context.Values.Any(v => v.Value == value.Value && v.Language == sourceLanguage))
                        context.Values.Add(new Translation { Key = tkey, Value = value.Value, Language = "en" });
                }
                context.SaveChanges();

                // 2. Produce output
                var languages = foundKeys
                    .SelectMany(collectionSelector => collectionSelector.Translations)
                    .Select(t => t.Language)
                    .Distinct();
                var dict = languages.ToDictionary(language => language, language => new LocalizedStringTable("undefined"));
                foreach (var translationKey in foundKeys)
                {
                    foreach (var translation in translationKey.Translations.OrderByDescending(t => t.RelativeScore))
                    {
                        var key = translation.Key.Key;
                        var table = dict[translation.Language];

                        if (table.ContainsKey(key))
                            continue;

                        table.Add(key, new LocalizedString
                        {
                            Comment = translation.Comment ?? translation.Key.Comment,
                            Key = key,
                            Value = translation.Value
                        });
                    }
                }

                // 3. Produce zip in format
                using (var zip = new ZipFile())
                {
                    foreach (var language in dict.Keys)
                    {
                        zip.AddEntry(Path.Combine(language, "Localizable.strings"), dict[language].ToString(OutputFormat.Strings));
                    }
                    var ms = new MemoryStream();
                    zip.Save(ms);
                    ms.Position = 0;
                    return File(ms, "application/zip", "translations.zip");
                }
            }
        }

        private string GetSourceLanguage(string fileName)
        {
            var match = Regex.Match(fileName, "resources\\.(?<language>.+?)-.+\\.resx");
            if (match.Success)
                return match.Groups["language"].Value;
            return "en";
        }

        private OutputFormat GetOutputFormat(string fileName)
        {
            if(fileName.EndsWith(".plist"))
                return OutputFormat.PList;
            if (fileName.EndsWith(".resx"))
                return OutputFormat.ResX;
            if (fileName.EndsWith(".xml"))
                return OutputFormat.Xml;
            return OutputFormat.Strings;
        }

        private string GetFileName(HttpPostedFileBase qqfile)
        {
            return (qqfile == null ? Request.QueryString["qqfile"] : qqfile.FileName).ToLower();
        }
    }
}