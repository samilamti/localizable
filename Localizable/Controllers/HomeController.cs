using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Localizable.Database;
using Localizable.Models;
using System.Data.Entity;
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

        public ActionResult Translate(int skip = 0)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Translate", "Oauth");

            var ret = new TranslateModel();
            using (var context = new Context())
            {
                var translator = context.GetTranslator(User);
                ret.Language = translator.Language;
            }

            using (var context = new Context())
            {
                IQueryable<TranslationKey> untranslatedKeys = context.Keys;
                if (skip > 0)
                {
                    untranslatedKeys = untranslatedKeys.Where(k => k.Id > skip);
                    if (!untranslatedKeys.Any())
                        untranslatedKeys = context.Keys;
                }

                untranslatedKeys = untranslatedKeys
                    .Where(key => !key.Translations.Any(translation => translation.Language == ret.Language))
                    .Take(5);

                ret.Translations = new List<TranslationModel>(
                    untranslatedKeys.Select(key => new TranslationModel { Key = key.Key, Comment = key.Comment }).ToList());

                if (ret.Translations.Count > 0)
                    ret.To = untranslatedKeys.Max(k => k.Id);
            }

            return View(ret);
        }

        [HttpPost]
        public ActionResult Translate(TranslateModel model)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            bool changedLanguage = false;

            using (var context = new Context())
            {
                var translator = context.GetTranslator(User);
                if (translator.Language != model.Language)
                {
                    translator.Language = model.Language;
                    changedLanguage = true;
                }

                if (!changedLanguage && model.Translations != null)
                {
                    foreach (var translation in model.Translations.Where(t => t.Value != null || t.Untranslatable))
                    {
                        var key = context.Keys.First(k => k.Key == translation.Key);

                        if (translation.Value != null)
                            key.Translations.Add(new Translation
                                                     {
                                                         Language = model.Language, 
                                                         Value = translation.Value,
                                                         Translator = translator
                                                     });
                        else if (translation.Untranslatable)
                        {
                            if (!context.DownvotedKeys.Any(dk => dk.Key.Id == key.Id && dk.Translator.Id == translator.Id))
                            {
                                key.DownVotes++;
                                if (key.DownVotes > 5)
                                {
                                    context.Values.Where(v => v.Key.Id == key.Id).ToList().ForEach(v => context.Values.Remove(v));
                                    context.DownvotedKeys.Where(dk => dk.Key.Id == key.Id).ToList().ForEach(dk => context.DownvotedKeys.Remove(dk));
                                    context.Keys.Remove(key);
                                }
                                context.DownvotedKeys.Add(new DownvotedKey {Key = key, Translator = translator});
                            }
                        }
                    }
                }

                context.SaveChanges();
            }

            return RedirectToAction("Translate", new { skip = changedLanguage ? 0 : model.To });
        }

        public ActionResult Vote()
        {
            using (var context = new Context())
            {
                return View(context.Keys.Include(key => key.Translations).ToList());
            }
        }

        public ActionResult ContributeFile()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("ContributeFile", "Oauth");

            return View();
        }
    }
}
