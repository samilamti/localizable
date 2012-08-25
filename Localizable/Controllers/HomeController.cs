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
                ret.Language = context.GetTranslator(User).Language;

                IQueryable<TranslationKey> untranslatedKeys;
                if (skip > 0)
                {
                    untranslatedKeys = context.Keys.Where(k => k.Id > skip);
                    if (!untranslatedKeys.Any())
                        untranslatedKeys = context.Keys;
                }
                else
                    untranslatedKeys = context.Keys;

                untranslatedKeys = untranslatedKeys
                    .Where(key => !key.Translations.Any(translation => translation.Language == ret.Language))
                    .Take(5);

                ret.Translations = untranslatedKeys
                    .Select(key => new TranslationModel { Key = key.Key, Comment = key.Comment })
                    .ToList();

                if (ret.Translations.Count > 0)
                    ret.To = untranslatedKeys.Max(k => k.Id);
                else
                {
                    TempData["Message"] = "There are currently no untranslated keys. However, perhaps you could help us by voting on other people's translations? Thanks!";
                    return RedirectToAction("Vote");
                }
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
                            key.Translations.Add(new Translation(model.Language, translation.Value) { Translator = translator });
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
                                else
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
                var data = context.Keys.Include(key => key.Translations)
                    .Where(key => key.Translations.Any())
                    .OrderByDescending(key => key.Added)
                    .ToList();

                return View(data);
            }
        }

        public JsonResult AddVote(int translation, string direction)
        {
            using (var context = new Context())
            {
                var translator = context.GetTranslator(User);
                var evt = context.Events
                    .Where(e => e.Translator.Id == translator.Id && e.ObjectId == translation && e.EventName == "vote")
                    .FirstOrDefault();

                if (evt != null)
                    return Json(context.Values.Find(translation).RelativeScore);

                context.Events.Add(new Event { 
                    Translator = translator,
                    EventName = "vote",
                    ObjectId = translation
                });

                var t = context.Values.Find(translation);
                if (direction == "up")
                    t.UpVotes++;
                else if (direction == "down")
                    t.DownVotes++;
                t.RelativeScore = t.UpVotes - t.DownVotes;
                context.SaveChanges();
                return Json(t.RelativeScore);
            }
        }

        public ActionResult ContributeFile()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("ContributeFile", "Oauth");

            return View();
        }

        public ActionResult Resources()
        {
            return View();
        }
    }
}
