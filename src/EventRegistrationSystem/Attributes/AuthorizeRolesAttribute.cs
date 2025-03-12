using System.Web;
using System.Web.Mvc;
using EventRegistrationSystem.Repositories;
using Microsoft.AspNet.Identity;

namespace EventRegistrationSystem.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorized = base.AuthorizeCore(httpContext);

            if (!isAuthorized)
                return false;

            // If the user is authenticated, check if they are in any of the required roles
            string userId = httpContext.User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return false;

            // Check if user is in any of the required roles
            var roleRepository = DependencyResolver.Current.GetService<IRoleRepository>();
            foreach (var role in _roles)
            {
                if (roleRepository.UserIsInRole(userId, role))
                    return true;
            }

            return false;
        }
    }
}