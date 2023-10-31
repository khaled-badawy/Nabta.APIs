using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.DAL;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nabta_Production.BL
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly IConfiguration _config;
        private string urlUserPicture { get;}

        public AuthManager(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt , IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
            _jwt = jwt.Value;
            urlUserPicture = $"{_config.GetSection("ServerDownloadPath").Value!}/users/attachments";
        }

        public string CreateJwtEmailToken(ApplicationUser user)
        {
            List<Claim> claimsList = new List<Claim>
                 {
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 };

            //Getting the key ready
            string keyString = _jwt.SecretKey;
            byte[] keyInBytes = Encoding.ASCII.GetBytes(keyString);
            var key = new SymmetricSecurityKey(keyInBytes);

            //Combine Secret Key with Hashing Algorithm
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            //Putting All together
            var expiry = DateTime.Now.AddDays(30);
            var jwt = new JwtSecurityToken(
                            issuer: _jwt.Issuer,
                            audience: _jwt.Audience,
                            expires: expiry,
                            claims: claimsList,
                            signingCredentials: signingCredentials
                            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return tokenString;
        }

        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var claimsList = await _userManager.GetClaimsAsync(user);

            if (claimsList.IsNullOrEmpty())
            {
                if (user.ProfilePicture is not null && user.OldId is null)
                {
                    claimsList = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.MobilePhone , user.UserName!),
                        new Claim(ClaimTypes.GivenName , user.FullName!),
                        new Claim("UserPicture", $"{urlUserPicture}/{user.Id}/{user.ProfilePicture}"),
                    };
                }
                else if(user.ProfilePicture is not null && user.OldId is not null)
                {
                    claimsList = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.MobilePhone , user.UserName!),
                        new Claim(ClaimTypes.GivenName , user.FullName!),
                        new Claim("UserPicture", $"{urlUserPicture}/{user.OldId}/{user.ProfilePicture}"),
                    };
                }
                else
                {
                    claimsList = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, "User"),
                            new Claim(ClaimTypes.Email, user.Email!),
                            new Claim(ClaimTypes.MobilePhone , user.UserName!),
                            new Claim(ClaimTypes.GivenName , user.FullName!),
                            new Claim("UserPicture", "null"),
                        };
                }

                await _userManager.AddClaimsAsync(user, claimsList);
            }

            //Getting the key ready
            string keyString = _jwt.SecretKey;
            byte[] keyInBytes = Encoding.ASCII.GetBytes(keyString);
            var key = new SymmetricSecurityKey(keyInBytes);

            //Combine Secret Key with Hashing Algorithm
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            //Putting All together
            var expiry = DateTime.Now.AddMonths(_jwt.DurationInMonths);
            var jwt = new JwtSecurityToken(
                            issuer: _jwt.Issuer,
                            audience: _jwt.Audience,
                            expires: expiry,
                            claims: claimsList,
                            signingCredentials: signingCredentials
                            );
            return jwt;
        }

        public async Task<IdentityResult> UpdateUserClaim(ApplicationUser user, Claim newClaim)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            Claim userClaim = userClaims.FirstOrDefault(c => c.Type == newClaim.Type)!;

            var removeClaimResult = await _userManager.RemoveClaimAsync(user, userClaim);
            if (!removeClaimResult.Succeeded) return removeClaimResult;

            var addClaimResult = await _userManager.AddClaimAsync(user, newClaim);
            return addClaimResult;
        }
    }
}
