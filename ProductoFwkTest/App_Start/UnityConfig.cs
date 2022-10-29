//Copyright(c) 2022 - Present, Cristofher Parada All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

//*Redistributions of source code must retain the above copyright notice, this list of conditions
//  and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, this list of conditions
//  and the following disclaimer in the documentation and/or other materials provided with the distribution.
//* Neither the name of Cristofher Parada,
// nor the names of its contributors may be used to endorse or promote products
//  derived from this software without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


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