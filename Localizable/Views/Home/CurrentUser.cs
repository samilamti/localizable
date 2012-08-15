using System.Web;
using System.Web.SessionState;

namespace Localizable.Models
{
    public class CurrentUser
    {
        private static HttpSessionState _session;
        private string _fullName, _mail;
        private string _lastVisitedUri;

        private static CurrentUser Instance
        {
            get
            {
                if (_session == null)
                    _session = HttpContext.Current.Session;

                var instance = _session["47A0325A-31BE-4A1A-9C6F-E98E6CDFC849"] as CurrentUser;
                if (instance == null)
                {
                    instance = new CurrentUser();
                    _session["47A0325A-31BE-4A1A-9C6F-E98E6CDFC849"] = instance;
                }

                return instance;
            }
        }

        public static string FullName { get { return Instance._fullName; } set { Instance._fullName = value; } }
        public static string EMail { get { return Instance._mail; } set { Instance._mail = value; } }
        public static string Name { get { return Instance._fullName ?? Instance._mail; } }
        public static bool IsAuthenticated { get { return Name != null; } }
        public static string LastVisitedUri { get { return Instance._lastVisitedUri ?? "/Home/Index"; } set { Instance._lastVisitedUri = value; } }

        public static void SaveReferrer()
        {
            if (HttpContext.Current.Request.UrlReferrer != null)
                LastVisitedUri = HttpContext.Current.Request.UrlReferrer.AbsolutePath;
        }
    }
}