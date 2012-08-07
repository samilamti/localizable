using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Localizable.Models;
using Models;

namespace Localizable.Controllers
{
    public class TranslateController : Controller
    {
        //
        // GET: /Translate/

        public ActionResult Index(string language)
        {
            ViewBag.Language = language;
            using (var context = new DatabaseContext())
            {
                var untranslatedKeys =
                    context.Keys.Where(key => !key.Translations.Any(translation => translation.Language == language));
                return View(untranslatedKeys.Select(key => new TranslateModel { Key = key.Key, ValueId = key.Id }).ToList());
            }
        }

        [HttpPost]
        public ActionResult Translate(string key, string value, string lang)
        {
            using(var context = new DatabaseContext())
            {
                var translatedKey = context.Keys.Find(int.Parse(key.Substring(5)));
                translatedKey.Translations.Add(new Translation
                                                   {
                                                       Value = value, 
                                                       Language = lang
                                                   });
                context.SaveChanges();
            }

            return Json(true);
        }
    }
}
