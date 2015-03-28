using fingerprints_service.Services;
using fingerprints_service_api;
using Microsoft.Practices.Unity;
using Owin;
using System.Web.Http;
using Unity.WebApi;

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

            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IFingerprintsScanner, FutronicServices.FutronicScanner>();
            container.RegisterType<FingerprintsScanningService, FingerprintsScanningService>(
                new InjectionProperty("ServerUrl", Program.ServerUrl));

            config.DependencyResolver = new UnityDependencyResolver(container);

            config.Routes.MapHttpRoute(
                name: "StaticPages",
                routeTemplate: "static/{controller}/{page}"
            );

            config.EnableSystemDiagnosticsTracing();
            appBuilder.UseWebApi(config);
        }
    }
}
