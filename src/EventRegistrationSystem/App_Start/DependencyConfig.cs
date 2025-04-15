using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using EventRegistrationSystem.Controllers;
using EventRegistrationSystem.Data;
using EventRegistrationSystem.Identity;
using EventRegistrationSystem.Models;
using EventRegistrationSystem.Repositories;
using EventRegistrationSystem.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Mvc5;

namespace EventRegistrationSystem
{
    public static class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var container = new UnityContainer();

            // Register the ApiClient with its dependencies
            var apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7264";
            var httpClient = new HttpClient();
            container.RegisterInstance<HttpClient>(httpClient);
            container.RegisterInstance<string>("ApiBaseUrl", apiBaseUrl);
            container.RegisterType<IApiClient, ApiClient>(new InjectionConstructor(
                new ResolvedParameter<HttpClient>(),
                new ResolvedParameter<string>("ApiBaseUrl")
            ));

            // Register the SqliteUserStore with its dependencies
            container.RegisterType<SqliteUserStore>(new ContainerControlledLifetimeManager());

            // Register repositories
            container.RegisterType<IEventRepository, EventRepository>();
            container.RegisterType<IRegistrationRepository, RegistrationRepository>();
            container.RegisterType<IRoleRepository, RoleRepository>();

            // Register identity stores if using custom SQLite implementation
            container.RegisterType<IUserStore<ApplicationUser>, SqliteUserStore>();
            container.RegisterType<IUserPasswordStore<ApplicationUser>, SqliteUserStore>();
            container.RegisterType<IUserEmailStore<ApplicationUser>, SqliteUserStore>();
            container.RegisterType<SqliteUserStore>();

            // Register your UserManager and SignInManager
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<ApplicationSignInManager>(new InjectionConstructor(
                new ResolvedParameter<ApplicationUserManager>(),
                new ResolvedParameter<IAuthenticationManager>(),
                new ResolvedParameter<IRoleRepository>()
            ));
            //container.RegisterType<ApplicationSignInManager>(new InjectionConstructor(
            //    new ResolvedParameter<ApplicationUserManager>(),
            //    new ResolvedParameter<IAuthenticationManager>()
            //));

            // Register IAuthenticationManager with a factory method to get it from OWIN context
            container.RegisterFactory<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication,
                new HierarchicalLifetimeManager());

            // Register MVC controllers
            container.RegisterType<AccountController>();
            container.RegisterType<ManageController>();
            container.RegisterType<AdminController>();
            container.RegisterType<EventController>();
            container.RegisterType<HomeController>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
