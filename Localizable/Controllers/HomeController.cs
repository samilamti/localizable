using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Localizable.Database;
using Localizable.Models;
using Models;

namespace Localizable.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Credits()
        {
            return View();
        }

        public ActionResult Translate()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Translate", "Oauth");

            var ret = new TranslateModel();
            using (var context = new Context())
            {
                var translator = context.Translators.FirstOrDefault(t => t.EMail == User.Identity.Name) ?? new Translator();
                ret.Language = translator.Language;
            }

            using (var context = new Context())
            {
                var untranslatedKeys = context.Keys
                    .Where(key => !key.Translations.Any(translation => translation.Language == ret.Language))
                    .Take(5);
                ret.Translations = new List<TranslationModel>(untranslatedKeys.Select(key => new TranslationModel() { Key = key.Key }).ToList());
            }

            return View(ret);
        }

        [HttpPost]
        public ActionResult Translate(TranslateModel model)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            using (var context = new Context())
            {
                var translator = context.Translators.FirstOrDefault(t => t.EMail == User.Identity.Name) ?? new Translator();
                translator.Language = model.Language;

                if(model.Translations != null)
                foreach (var translation in model.Translations)
                {
                    
                }

                context.SaveChanges();
            }

            return RedirectToAction("Translate");
        }

        public ActionResult Vote()
        {
            return View();
        }
    }
}
