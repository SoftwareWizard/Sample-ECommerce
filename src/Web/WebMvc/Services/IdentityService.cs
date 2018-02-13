using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using ShoesOnContainers.Web.WebMvc.Models;

namespace ShoesOnContainers.Web.WebMvc.Services
{
    public class IdentityService : IIdentityService<ApplicationUser>
    {
        public ApplicationUser Get(IPrincipal principal)
        {
            if (principal is ClaimsPrincipal claimsPrincipal)
            {
                return new ApplicationUser
                {
                    Email = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value ?? "",
                    Id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? ""
                };
            }

            throw new ArgumentException("The principal must be a ClaimsPrincipal", nameof(principal));
        }
    }
}