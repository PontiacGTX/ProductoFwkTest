using ProductoFwkTest.App_Start;
using ProductoFwkTest.Repository;
using ProductoFwkTest.Services;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace ProductoFwkTest
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.AddExtension(new Diagnostic());
            container.RegisterInstance<Microsoft.Extensions.Logging.ILoggerFactory>(new Microsoft.Extensions.Logging.LoggerFactory(), new ContainerControlledLifetimeManager());
            container.RegisterFactory(typeof(Microsoft.Extensions.Logging.ILogger<>), null, (c, t, n) =>
            {
                var factory = c.Resolve<Microsoft.Extensions.Logging.ILoggerFactory>();
                var genericType = t.GetGenericArguments().First();
                var mi = typeof(Microsoft.Extensions.Logging.LoggerFactoryExtensions).GetMethods().Single(m => m.Name == "CreateLogger" && m.IsGenericMethodDefinition);
                var gi = mi.MakeGenericMethod(t.GetGenericArguments().First());
                return gi.Invoke(null, new[] { factory });
            });
            container.RegisterType(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(Microsoft.Extensions.Logging.Logger<>), (new HierarchicalLifetimeManager()));
            var con = ConfigurationManager.AppSettings["MyConnectionString"];
            var paramStringCon = new ParameterOverrides();

            container.RegisterFactory(typeof(StringCon), instance => new StringCon(con), FactoryLifetime.Scoped);
            var logProd = container.Resolve(typeof(Microsoft.Extensions.Logging.ILogger<ProductoRepository>));
            var strCon = container.Resolve<StringCon>();
            container.RegisterType<ProductoRepository>(nameof(ProductoRepository), new InjectionConstructor(strCon, logProd));
            var prodRepo = container.Resolve<ProductoRepository>();
            container.RegisterType<ProductoService>(nameof(ProductoService), new InjectionConstructor(prodRepo));

            var logProdCat = container.Resolve(typeof(Microsoft.Extensions.Logging.ILogger<CategoriaProductoRepository>));


            container.RegisterType<CategoriaProductoRepository>(nameof(CategoriaProductoRepository), new InjectionConstructor(strCon, logProdCat));
            var prodCatRepo = container.Resolve<CategoriaProductoRepository>();
            container.RegisterType<ProductoCatService>(nameof(ProductoCatService), new InjectionConstructor(prodCatRepo));

            DependencyResolver.SetResolver(new Unity.AspNet.Mvc.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.AspNet.WebApi.UnityDependencyResolver(container);
        }
    }
}