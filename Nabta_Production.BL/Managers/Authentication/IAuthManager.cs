using Microsoft.AspNetCore.Identity;
using Nabta_Production.DAL;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Nabta_Production.BL
{
    public interface IAuthManager
    {
        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
        string CreateJwtEmailToken(ApplicationUser user);
        Task<IdentityResult> UpdateUserClaim(ApplicationUser user, Claim newClaim);
    }
}
