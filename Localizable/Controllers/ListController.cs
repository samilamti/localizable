using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

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

        public JsonResult UpVote(int id)
        {
            using (var context = new DatabaseContext())
            {
                var translation = context.Values.Find(id);
                translation.UpVotes++;
                context.SaveChanges();
                return Json(translation.UpVotes - translation.DownVotes);
            }
        }
    }
}
