using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Slidable.Identity.Data
{
    public class SlidableClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly ILogger<SlidableClaimsPrincipalFactory> _logger;

        public SlidableClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> optionsAccessor, ILogger<SlidableClaimsPrincipalFactory> logger) : base(userManager, roleManager, optionsAccessor)
        {
            _logger = logger;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            _logger.LogInformation($"User: {user.Handle} - {user.UserName} - {user.Email}");
            var principal = await base.CreateAsync(user);

            var identity = ((ClaimsIdentity)principal.Identity);

            if (!string.IsNullOrWhiteSpace(user.Handle))
            {
                identity.AddClaim(new Claim(SlidableClaimTypes.Handle, user.Handle));
            }
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            }

            return principal;
        }
    }
}