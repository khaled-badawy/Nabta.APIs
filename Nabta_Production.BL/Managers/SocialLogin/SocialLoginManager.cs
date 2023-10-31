using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.DAL;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Nabta_Production.BL
{
    public class SocialLoginManager : ISocialLoginManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly IConfiguration _config;
        public SocialLoginManager(UserManager<ApplicationUser> userManager , IConfiguration config, IOptions<JWT> jwt)
        {
            _config = config;
            _userManager = userManager;
            _jwt = jwt.Value;
        }

        private async Task<string> CreateJwtGoogleToken(ApplicationUser user)
        {
            var claimsList = await _userManager.GetClaimsAsync(user);

            if (claimsList.Count == 0)
            {
                if (user.ProfilePicture is not null)
                {
                    claimsList = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.MobilePhone , user.UserName!),
                        new Claim(ClaimTypes.GivenName , user.FullName!),
                        new Claim("UserPicture", user.ProfilePicture),
                    };
                }

                claimsList = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.MobilePhone , user.UserName!),
                        new Claim(ClaimTypes.GivenName , user.FullName!),
                        new Claim("UserPicture", "null"),
                    };

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
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return tokenString;
        }

        private async Task<string> LogUserInByGoogleAuth(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            string userToken = await CreateJwtGoogleToken(user!);

            return userToken;
        }

        public async Task<AuthModel>  RegisterUserByGoogleAuth(string token)
        {
            try
            {
                string userToken = "";
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, new ValidationSettings { Audience = new List<string> { "116130660745-4orcf7njvej2q7n5e28vkjoovu3n6tvu.apps.googleusercontent.com" } });
                var isUserExisted = await _userManager.FindByEmailAsync(payload.Email);
                if (isUserExisted != null)
                {
                    userToken = await LogUserInByGoogleAuth(payload.Email);
                    return new AuthModel 
                    {
                        IsAuthenticated = false ,
                        Message = "User is existed already" ,
                        Email = payload.Email,
                        Token = userToken,
                        UserFullName = payload.Name
                    };
                }

                ApplicationUser user = new ApplicationUser()
                {
                    FullName = payload.Name,
                    Email = payload.Email,
                    EmailConfirmed = payload.EmailVerified,
                    ProfilePicture = payload.Picture,
                };
                var isUserCreated = await _userManager.CreateAsync(user);
                if (!isUserCreated.Succeeded)
                {
                    return new AuthModel { Message = isUserCreated.Errors.ToString(), IsAuthenticated = false };
                }

                UserLoginInfo userLoginInfo = new UserLoginInfo("google", payload.Subject, "GOOGLE");

                var registerResult = await _userManager.AddLoginAsync(user, userLoginInfo);
                if (!registerResult.Succeeded)
                {
                    return new AuthModel { Message = registerResult.Errors.ToString(), IsAuthenticated = false };
                }

                userToken = await CreateJwtGoogleToken(user);
                return new AuthModel 
                {
                    IsAuthenticated = true ,
                    Email = user.Email ,
                    Message = "User added successfully" ,
                    Token = userToken,
                    UserFullName = user.FullName
                };
            }
            catch (Exception ex)
            {
                var message = ex.Message.ToString();

                return new AuthModel { Message = message.ToString(), IsAuthenticated = false };
            }
        }
    }
}
