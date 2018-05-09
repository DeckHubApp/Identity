using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeckHub.Identity.Data
{
    public class DeckHubClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly ILogger<DeckHubClaimsPrincipalFactory> _logger;

        public DeckHubClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> optionsAccessor, ILogger<DeckHubClaimsPrincipalFactory> logger) : base(userManager, roleManager, optionsAccessor)
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
                identity.AddClaim(new Claim(DeckHubClaimTypes.Handle, user.Handle));
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