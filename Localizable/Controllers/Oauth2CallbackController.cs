using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Localizable.Controllers
{
    public class Oauth2CallbackController : Controller
    {
        //
        // GET: /Oauth2Callback/

        public ActionResult Index()
        {
            return View();
        }

    }
}
