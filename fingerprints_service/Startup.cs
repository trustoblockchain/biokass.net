using Owin;
using System.Web.Http;

namespace fingerprints_service
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "StaticPages",
                routeTemplate: "static/{controller}/{page}"
            );

            config.EnableSystemDiagnosticsTracing();
            appBuilder.UseWebApi(config);
        }
    }
}
