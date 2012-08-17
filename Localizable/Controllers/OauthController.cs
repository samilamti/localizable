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
using Localizable.Database;
using Localizable.Models;
using Models;

namespace Localizable.Controllers
{
    public class OauthController : Controller
    {
        [HttpPost]
        public ActionResult LogOn(string loginIdentifier)
        {
            if (!Identifier.IsValid(loginIdentifier))
            {
                ModelState.AddModelError("loginIdentifier", "The specified login identifier is invalid");
                return Redirect(Request.UrlReferrer.AbsolutePath);
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
                TempData["Error"] = "Login failed";
                return RedirectToAction("Translate", "Home");
            }

            //http://thenextweb.com/socialmedia/2010/11/04/facebook-connect-oauth-and-openid-the-differences-and-the-future/
            //http://developers.facebook.com/docs/authentication/
            //http://developers.facebook.com/docs/guides/web/

            switch (response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    var claimsResponse = response.GetExtension<ClaimsResponse>();
                    var email = claimsResponse.Email;
                    var fullName = claimsResponse.FullName;
                    var user = Membership.FindUsersByEmail(email).OfType<MembershipUser>().FirstOrDefault();
                    if (user == null)
                    {
                        Membership.CreateUser(email, "2E2F37C2-79AA-4521-95D8-842B38BB2809", email);
                        using (var context = new Context())
                        {
                            var translator = context.Translators.FirstOrDefault(t => t.EMail == email) ?? context.Translators.Add(new Translator());
                            translator.EMail = email;
                            translator.FullName = fullName;
                            context.SaveChanges();
                        }
                    }
                    FormsAuthentication.SetAuthCookie(email, true);
                    return RedirectToAction("Translate", "Home");
                    break;
                case AuthenticationStatus.Canceled:
                    ModelState.AddModelError("loginIdentifier", "Login was cancelled at the provider");
                    TempData["Error"] = "Login was cancelled at the provider";
                    break;
                case AuthenticationStatus.Failed:
                    ModelState.AddModelError("loginIdentifier", "Login failed using the provided OpenID identifier");
                    TempData["Error"] = "Login failed using the provided OpenID identifier";
                    break;
            }

            return RedirectToAction("Translate", "Home");
        }

        public ActionResult Translate()
        {
            return View();
        }

        public ActionResult Vote(int translation, string direction)
        {
            return View();
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ContributeFile()
        {
            return View();
        }
    }
}
