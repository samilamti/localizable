using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using LanguageFileParsers;
using Localizable.Database;
using Localizable.Services;

namespace Localizable
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        // Application services
        public static FileParserFacade FileParser { get; private set; }
        public static TranslationService TranslationService { get; private set; }
        public static CompressionService CompressionService { get; private set; }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            System.Data.Entity.Database.SetInitializer(new Initializer());

            FileParser = new FileParserFacade(new List<ILanguageFileParser> {
                new AndroidXmlFileParser(),
                new PlistFileParser(),
                new ResxFileParser(),
                new StringsFileParser(),
            });

            TranslationService = new TranslationService();
            CompressionService = new CompressionService();
        }
    }
}