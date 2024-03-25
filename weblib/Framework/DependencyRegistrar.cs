using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Builder;
using Autofac.Core;

namespace Weblib.Framework
{
    using System.Linq;

    using LIB.Configuration;
    using LIB.Infrastructure;
    using LIB.Infrastructure.DependencyManagement;
    using LIB.Plugins;
    using LIB.Tools.Utils;

    using WebLib.EmbeddedViews;
    using WebLib.Themes;
    using WebLib.UI;

    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();

            //controllers
            //builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());
            
            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerHttpRequest();
            
            builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerHttpRequest();
            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerHttpRequest();
            builder.RegisterType<EmbeddedViewResolver>().As<IEmbeddedViewResolver>().SingleInstance();
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerHttpRequest();
           

        }

        public int Order
        {
            get { return 0; }
        }
    }


    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }
        /*
        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    var currentStoreId = c.Resolve<IStoreContext>().CurrentStore.Id;
                    //uncomment the code below if you want load settings per store only when you have two stores installed.
                    //var currentStoreId = c.Resolve<IStoreService>().GetAllStores().Count > 1
                    //    c.Resolve<IStoreContext>().CurrentStore.Id : 0;

                    //although it's better to connect to your database and execute the following SQL:
                    //DELETE FROM [Setting] WHERE [StoreId] > 0
                    return c.Resolve<ISettingService>().LoadSetting<TSettings>(currentStoreId);
                })
                .InstancePerHttpRequest()
                .CreateRegistration();
        }
        */
        public bool IsAdapterForIndividualComponents { get { return false; } }
    }

}
