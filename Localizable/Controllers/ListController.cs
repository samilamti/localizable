using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Localizable.Database;

namespace Localizable.Controllers
{
    public class ListController : Controller
    {
        public JsonResult Translations(int id)
        {
            using (var context = new Context())
            {
                var key = context.Keys.Find(id);
                return Json(key.Translations.Select(translation => translation.Value).ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Vote(int translation, string direction)
        {
            using (var context = new Context())
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
