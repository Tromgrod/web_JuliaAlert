// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Natur Bravo Pilot">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the MvcApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlert
{
    using System.Web.Hosting;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using LIB.Infrastructure;

    using WebLib.EmbeddedViews;

    using Weblib.Framework;

    using WebLib.Themes;
    using LIB.Tools.Utils;
    using System.Web;
    using System;
    using JuliaAlert;
    using JuliaAlert.Helpers;
    using System.Web.Configuration;
    using LIB.Tools.Security;
    using System.Net;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //initialize engine context
            EngineContext.Initialize(false);

            //set dependency resolver
            var dependencyResolver = new LibDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            //model binders
            //ModelBinders.Binders.Add(typeof(BaseNopModel), new NopModelBinder());

            //remove all view engines
            ViewEngines.Engines.Clear();
            //except the themeable razor view engine we use
            ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            ExtendedRazorViewEngine engine = new ExtendedRazorViewEngine();
            engine.AddViewLocationFormat("~/Plugins/{1}/Views/{0}.cshtml");

            ViewEngines.Engines.Add(engine);

            //Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new MetadataProvider();

            //fluent validation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            //ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NopValidatorFactory()));

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DynamicJsGenerator.RegisterDynamic();
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //register virtual path provider for embedded views
            var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
        void Application_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;

            Exception exception = ctx.Server.GetLastError();

            if (Config.GetConfigValue("SendExceptionsByEmail") == "1")
                ExceptionManagement.HandleExceptionByEmail(exception, false, true);

            if (Config.GetConfigValue("SendExceptionsByAPI") == "1")
                ExceptionManagement.HandleExceptionByAPI(exception, ctx);

            var currentUser = Authentication.GetCurrentUser();

            var routeValues = new RouteValueDictionary();
            var compilationSection = (CompilationSection)System.Configuration.ConfigurationManager.GetSection(@"system.web/compilation");

            if (!compilationSection.Debug && (currentUser == null || (currentUser != null && !currentUser.DisplayError)))
            {
                if (exception is HttpException httpException)
                {
                    switch (httpException.GetHttpCode())
                    {
                        case 404:
                            // page not found
                            Response.Clear();
                            Server.ClearError();
                            Response.TrySkipIisCustomErrors = true;

                            routeValues.Add("action", "NotFound");
                            ctx.Response.RedirectToRoute("ErrorHandle", routeValues);

                            break;
                        case 503:
                            // server error
                            Response.Clear();
                            Server.ClearError();
                            Response.TrySkipIisCustomErrors = true;

                            routeValues.Add("action", "Unavailable");
                            ctx.Response.RedirectToRoute("ErrorHandle", routeValues);

                            break;
                        default:
                            Response.Clear();
                            Server.ClearError();
                            Response.TrySkipIisCustomErrors = true;

                            routeValues.Add("action", "Index");
                            ctx.Response.RedirectToRoute("ErrorHandle", routeValues);
                            break;
                    }
                }
                else
                {
                    Response.Clear();
                    Server.ClearError();
                    Response.TrySkipIisCustomErrors = true;

                    routeValues.Add("action", "Index");
                    ctx.Response.RedirectToRoute("ErrorHandle", routeValues);
                }
            }
        }
    }
}