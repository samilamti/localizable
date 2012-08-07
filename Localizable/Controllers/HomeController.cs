using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Localizable.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string message = "Welcome to ASP.NET MVC!";
            using (var context = new DatabaseContext())
            {
                var value = context.Values.FirstOrDefault(v => v.Key.Key == "Cowabunga");
                if (value != null)
                    message = value.Value;
            }

            ViewBag.Message = message;

            return View();
        }

        public ActionResult About()
        {
            using (var context = new DatabaseContext())
            {
                var key = new TranslationKey {Key = Guid.NewGuid().ToString()};
                context.Keys.Add(key);
                context.SaveChanges();
            }

            return View();

        }
    }
}
