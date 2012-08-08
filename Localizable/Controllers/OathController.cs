using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace Localizable.Controllers
{
    public class OathController : Controller
    {
        //
        // GET: /Oath/

        public ActionResult Index()
        {
            return RedirectToAction("LogOn");
        }

        [HttpPost]
        public ActionResult LogOn(string loginIdentifier)
        {
            if (!Identifier.IsValid(loginIdentifier))
            {
                ModelState.AddModelError("loginIdentifier", "The specified login identifier is invalid");
                return View();
            }

            var openId = new OpenIdRelyingParty();
            var request = openId.CreateRequest(Identifier.Parse(loginIdentifier));
            request.AddExtension(new ClaimsRequest
                                     {
                                         FullName = DemandLevel.Require,
                                         Email = DemandLevel.Require
                                     });

            return request.RedirectingResponse.AsActionResult();
        }

        [HttpGet]
        public ActionResult LogOn()
        {
            var openId = new OpenIdRelyingParty();
            var response = openId.GetResponse();
            if (response == null)
            {
                ModelState.AddModelError("loginIdentifier", "Login failed");
                return View();
            }

            //http://thenextweb.com/socialmedia/2010/11/04/facebook-connect-oauth-and-openid-the-differences-and-the-future/
            //http://developers.facebook.com/docs/authentication/
            //http://developers.facebook.com/docs/guides/web/

            switch (response.Status)
            {
                    case AuthenticationStatus.Authenticated:
                    var claimsResponse = response.GetExtension<ClaimsResponse>();
                    Session["e-mail"] = claimsResponse.Email;
                    Session["fullName"] = claimsResponse.FullName;
                    return RedirectToAction("Index", "Home");
                    break;
                    case AuthenticationStatus.Canceled:
                    ModelState.AddModelError("loginIdentifier", "Login was cancelled at the provider");
                    break;
                    case AuthenticationStatus.Failed:
                    ModelState.AddModelError("loginIdentifier", "Login failed using the provided OpenID identifier");
                    break;
            }

            return View();
        }

    }
}
