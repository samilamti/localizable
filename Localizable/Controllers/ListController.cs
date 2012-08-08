using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Localizable.Models;

namespace Localizable.Controllers
{
    public class ListController : Controller
    {
        //
        // GET: /List/

        public ActionResult Index()
        {
            using(var context = new DatabaseContext())
            {
                return View(context.Keys.Include(key =>key.Translations).ToList());    
            }
        }

        public JsonResult Translations(int id)
        {
            using (var context = new DatabaseContext())
            {
                var key = context.Keys.Find(id);
                return Json(key.Translations.Select(translation => translation.Value).ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Vote(int translation, string direction)
        {
            using (var context = new DatabaseContext())
            {
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
    }
}
