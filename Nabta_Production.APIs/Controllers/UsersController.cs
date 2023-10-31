using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;
using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Nabta_Production.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ClimateConfNewContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ISmsManager _smsManager;
        private readonly IAuthManager _authManager;
        private readonly IEmailManager _emailManager;
        private readonly IFileManager _fileManager;
        private readonly ISocialLoginManager _socialLoginManager;

        private string urlUserPicture { get; }


        public UsersController(ISocialLoginManager socialLoginManager,IFileManager fileManager, IAuthManager authManager, IEmailManager emailManager, ISmsManager smsManager, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, ClimateConfNewContext context, SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _smsManager = smsManager;
            _authManager = authManager;
            _emailManager = emailManager;
            _fileManager = fileManager;
            _socialLoginManager = socialLoginManager;
            urlUserPicture = $"{_configuration.GetSection("ServerDownloadPath").Value!}/users/attachments";
        }

        #region Seeding Old Users

        [HttpPost]
        [Route("seedingOldUsers")]
        public async Task<ActionResult> SeedingUsersData()
        {
            var usersToCopy = _context.Set<ClmUser>().ToList();
            if (usersToCopy == null)
            {
                return BadRequest();
            }

            foreach (var user in usersToCopy)
            {
                var newUser = new ApplicationUser
                {
                    UserName = user.Mobile,
                    Email = user.Email,
                    CreateDate = user.CreateDate,
                    FullName = user.Name,
                    RegistrationIp = user.RegistrationIp,
                    LastActivityIp = user.LastActivityIp,
                    ActivationCode = user.ActivationCode,
                    LoginCount = user.LoginCount,
                    Focus = user.Focus,
                    PhoneNumber = user.Mobile,
                    ProfilePicture = user.ProfilePicture,
                    IsDeleted = user.IsDeleted,
                    DeletedDate = user.DeletedDate,
                    OldId = user.Id,
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                    LikedPosts = user.LikedPosts
                };

                var result = await _userManager.CreateAsync(newUser, user.Password!);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                
                //if (user.ProfilePicture is not null)
                //{
                //    var claimsList = new List<Claim>
                //    {
                //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //        new Claim(ClaimTypes.Role, "User"),
                //        new Claim(ClaimTypes.Email, user.Email!),
                //        new Claim(ClaimTypes.MobilePhone , user.UserName!),
                //        new Claim(ClaimTypes.GivenName , user.Name!),
                //        new Claim("UserPicture", $"{urlUserPicture}/{user.Id}/{user.ProfilePicture}"),
                //    };
                //    await _userManager.AddClaimsAsync(newUser, claimsList);
                
                //var claimsList = new List<Claim>
                //        {
                //            new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                //            new Claim(ClaimTypes.Role, "User")
                //        };
            }

            return Ok("Old users seeded successfully.");
        }

        #endregion

        #region Login either confirming mail or phone

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto credentials)
        {
            // _signInManager.ExternalLoginSignInAsync(loginProvider,providerKey,isPersistent:false,,);


            ApplicationUser? user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user is null)
            {
                return BadRequest("Invalid User name");
            }
            var isLocked = await _userManager.IsLockedOutAsync(user);
            if (isLocked)
            {
                return BadRequest($"{credentials.UserName} is locked out");
            }
            bool isAuthenticated = await _userManager.CheckPasswordAsync(user, credentials.Password);
            if (!isAuthenticated)
            {
                await _userManager.AccessFailedAsync(user);
                return BadRequest("Invalid Password");
            }
            if (user.IsDeleted == true)
            {
                return BadRequest("Account has been deactivated or deleted ");
            }

            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            var isPhoneConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
            if (!isEmailConfirmed && !isPhoneConfirmed)
            {
                return BadRequest("Email or Phone number must be Confirmed ");
            }

            var jwt = await _authManager.CreateJwtToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return new TokenDto
            {
                Token = tokenString,
                Expiry = jwt.ValidTo
            };
        }

        #endregion

        #region Register As User Using Email confirmation

        [HttpPost]
        [Route("RegisterAsUserUsingEmail")]
        public async Task<ActionResult> RegisterByMail(RegisterDto registrDto)
        {
            var newUser = new ApplicationUser
            {
                UserName = registrDto.UserName,
                Email = registrDto.Email,
                PhoneNumber = registrDto.UserName,
                FullName = registrDto.FullName,
                Birthdate = registrDto!.Birthdate,
                IsDeleted = false,
                IsForgotPassEmailConfirmed = false,
                DeletedDate = null,
                CreateDate = DateTime.Now,
                JobTitle = registrDto.JobTitle
            };

            var result = await _userManager.CreateAsync(newUser, registrDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmationLink = Url.Action(
                action: "ConfirmEmail",
                controller: "Users",
                values: new { userId = newUser.Id, token },
                protocol: Request.Scheme);

            var emailResult = _emailManager.SendEmail(newUser.Email, "Confirm Email", $"Please confirm your email by clicking the following link: {confirmationLink}");
            //var message = new EmailDto(
            //  new string[] { newUser.Email },
            // $"Please confirm your email by clicking the following link: {confirmationLink}",
            // "Email Confirmation"
            // );

            //_emailManager.SendEmail(message);

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri(confirmationLink!);
            //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return Ok(newUser.Id);
        }

        #endregion

        #region Confirm Email 

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, "User"),
                };

                await _userManager.AddClaimsAsync(user, claims);

                return Ok("Email Confirmed Successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        #endregion

        #region Register As User Using Phone confirmation

        [HttpPost]
        [Route("RegisterAsUserUsingPhone")]
        public async Task<ActionResult> RegisterByPhone(RegisterDto registrDto, IFormFile? ProfilePicture)
        {
            var activationCode = new Random().Next(100000, 999999).ToString();
            var newUser = new ApplicationUser
            {
                UserName = registrDto.UserName,
                Email = registrDto.Email,
                PhoneNumber = registrDto.UserName,
                FullName = registrDto.FullName,
                CreateDate = DateTime.Now,
                ActivationCode = activationCode,
                IsDeleted = false,
                IsForgotPassEmailConfirmed = false,
                DeletedDate = null,
                Birthdate = registrDto.Birthdate,
                JobTitle = registrDto.JobTitle,
            };
            var result = await _userManager.CreateAsync(newUser, registrDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            _smsManager.SendSMS(newUser.PhoneNumber, $"From Nabta : Your verification code is :{activationCode} ");

            //await _fileManager.UploadFile(ProfilePicture, newUser.Id, "user").ConfigureAwait(false);
            //newUser.ProfilePicture = _fileManager.RenameFile(ProfilePicture, newUser.Id, "user");

            // var vodaConfig = _configuration.GetSection("VodafoneConfiguration").Get<VodafoneConfiguration>()!;

            // Web2SMS sms = new Web2SMS();

            //Web2SMS.Configure(
            //    url: vodaConfig.SmsUrl,
            //    accountId:vodaConfig.SmsAccoundId,
            //    clientSecret:vodaConfig.SmsClientSecret,
            //    defaultSenderName:"IDSC");

            //List<Recepient> recepients = new List<Recepient>()
            //{
            //    new Recepient()
            //    {
            //      SMSText = $"From Nabta : Your verification code is :{activationCode} ",
            //      SenderName = "IDSC",
            //      ReceiverMSISDN = $"+2{newUser.PhoneNumber}"
            //    }
            //};

            //Web2SMS.Send(recepients);


            //var phone = new PhoneDto(newUser.PhoneNumber, $"Your verification code is :{activationCode}");
            //await _phoneManager.Send(phone);

            return Ok(newUser.Id);
        }

        #endregion

        #region Confirm Phone

        //[HttpPost]
        //[Route("PhoneConfirmation")]
        //public async Task<ActionResult> ConfirmPhone(ConfirmPhoneDto userToConfirmPhone)
        //{
        //    ApplicationUser? user = await _userManager.FindByIdAsync(userToConfirmPhone.UserId.ToString());
        //    if (user is null)
        //    {
        //        return BadRequest("Invalid User");
        //    }
        //    if (userToConfirmPhone.ActivationCode != user.ActivationCode)
        //    {
        //        return BadRequest("Wrong activation code");
        //    }

        //    user.PhoneNumberConfirmed = true;
        //    var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.NameIdentifier, userToConfirmPhone.UserId.ToString()),
        //            new Claim(ClaimTypes.Role, "User"),
        //        };
        //    await _userManager.AddClaimsAsync(user, claims);
        //    return Ok("Phone number Confirmed Successfully");
        //}

        #endregion

        #region Remove User Profile Picture

        [HttpDelete]
        [Authorize]
        [Route("remove_picture")]

        public async Task<ActionResult> RemoveUserPicture()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();


            user.ProfilePicture = null;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            var claimResult = await _authManager.UpdateUserClaim(user, new Claim("UserPicture", "null"));
            if (!claimResult.Succeeded) return BadRequest(claimResult.Errors); 

            var jwt = await _authManager.CreateJwtToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return Ok(new { message = "تم حذف صورة ملفك الشخصي بنجاح" , token = tokenString } );
        }

        #endregion

        #region Update User Profile Picture 

        [HttpPost]
        [Authorize]
        [Route("update_picture")]

        public async Task<ActionResult> UpdateUserPicture(IFormFile newPicture)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var uploadImageResult = "";

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            bool isImage = allowedExtensions.Contains(Path.GetExtension(newPicture.FileName).ToLower());
            
            if (isImage)
            {
                uploadImageResult = await _fileManager.UploadFile(newPicture, user.Id, $"user{user.Id}", "user");
                if (!uploadImageResult.IsNullOrEmpty() && user.ProfilePicture is null)
                {
                    user.ProfilePicture = uploadImageResult;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded) return BadRequest(updateResult.Errors);

                    if (user.OldId is null)
                    {
                         await _authManager.UpdateUserClaim(user, new Claim("UserPicture", $"{urlUserPicture}/{user.Id}/{user.ProfilePicture}"));
                    }
                    else
                    {
                        await _authManager.UpdateUserClaim(user, new Claim("UserPicture", $"{urlUserPicture}/{user.OldId}/{user.ProfilePicture}"));
                    }
                }
            }
            else
            {
                return BadRequest(new {message = "يجب تحديث صورة المستخدم باستخدام ملف خاص بالصور فقط"});
            }

            if (String.IsNullOrEmpty(uploadImageResult)) return BadRequest(new { result = uploadImageResult, message = "عذرا لم يتم تحديث الصوره" });

            var jwt = await _authManager.CreateJwtToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return Ok(new {message = "تم تحديث الصوره بنجاح" , token = tokenString});
        }


        #endregion

        #region Update User Full Name

        [HttpPost]
        [Authorize]
        [Route("UpdateUserName")]
        public async Task<ActionResult> UpdateUserName(UpdateUserNameDto userToUpdateName)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            user.FullName = userToUpdateName.NewUserName;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest($"NO UPDATE {result.Errors}");
            }

            var updateClaimResult = await _authManager.UpdateUserClaim(user, new Claim(ClaimTypes.GivenName, userToUpdateName.NewUserName));
            if (!updateClaimResult.Succeeded) return BadRequest(updateClaimResult.Errors);

            var jwt = await _authManager.CreateJwtToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return Ok(new { message = "تم تحديث الاسم الكامل بنجاح" , token = tokenString });
        }

        #endregion

        #region Update User Email

        [HttpPost]
        [Authorize]
        [Route("UpdateUserEmail")]
        public async Task<ActionResult> UpdateUserMail(UpdateUserMailDto userToUpdateEmail)
        {
            var isEmailExisted = await _userManager.FindByEmailAsync(userToUpdateEmail.NewEmail);
            if (isEmailExisted is not null) return BadRequest(new { message = "هذا البريد الالكتروني مستخدم بالفعل" });

            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            if (userToUpdateEmail.NewEmail == user.Email)
            {
                return BadRequest(new { message = "البريد الإلكتروني الجديد مستخدم بالفعل" });
            }

            user.EmailConfirmed = false;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var jwtEmailTokenString =  _authManager.CreateJwtEmailToken(user);
            var jwtEmailToken = WebUtility.UrlEncode(jwtEmailTokenString);

            var changeEmailTokenString = await _userManager.GenerateChangeEmailTokenAsync(user, userToUpdateEmail.NewEmail);
            var changeEmailToken = WebUtility.UrlEncode(changeEmailTokenString);

            var confirmationLink = Url.Action(
                action: "ChangeEmailConfirmation",
                controller: "Users",
                values: new { jwtEmailToken, changeEmailToken, userToUpdateEmail.NewEmail },
                protocol: Request.Scheme);

            await _emailManager.SendEmail(userToUpdateEmail.NewEmail, "Update Email", $"Please confirm your updated email by clicking the following link: {confirmationLink}");

            return Ok(new { message = "يرجى التحقق من بريدك الإلكتروني الجديد" });
        }

        #endregion

        #region Update User Email Confirmation

        [HttpGet]
        [AllowAnonymous]
        [Route("UpdateEmailConfirmation")]

        public async Task<IActionResult> ChangeEmailConfirmation([FromQuery] string jwtEmailToken, [FromQuery] string changeEmailToken, [FromQuery] string newEmail)
        {
            if (jwtEmailToken == null || changeEmailToken == null || newEmail == null)
            {
                return BadRequest();
            }

            var decodedJwtEmailToken = WebUtility.UrlDecode(jwtEmailToken);
            var decodedChangeEmailToken = WebUtility.UrlDecode(changeEmailToken);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(decodedJwtEmailToken) as JwtSecurityToken;
            var userIdClaim = jsonToken!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            if (userIdClaim == null) return BadRequest();

            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, decodedChangeEmailToken);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var updateClaimResult = await _authManager.UpdateUserClaim(user, new Claim(ClaimTypes.Email, newEmail));
            if (!updateClaimResult.Succeeded) return BadRequest(updateClaimResult.Errors);

            user.EmailConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }
            return Ok("Email Updated Successfully");
        }

        #endregion

        #region Update User Phone Number

        [HttpPost]
        [Authorize]
        [Route("UpdateUserPhone")]

        public async Task<ActionResult> UpdateUserPhoneNumber(UpdateUserPhoneDto updatePhoneDto)
        {
            var isExisted = await _userManager.FindByNameAsync(updatePhoneDto.NewUserPhone);
            if (isExisted is not null) return BadRequest(new { message = "هذا الرقم مستخدم لدينا بالفعل" });

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest();

            var activationCode = new Random().Next(100000, 999999).ToString();
            var SmsResult = _smsManager.SendSMS(updatePhoneDto.NewUserPhone, $"{activationCode}: كود التفعيل الخاص بكم");
            if (SmsResult is  null) return BadRequest();
            user.ActivationCode = activationCode;
            user.PhoneNumberConfirmed = false;
            await _userManager.UpdateAsync(user);
            return Ok(new 
            {
                message = " برجاء ادخال كود التأكيد المرسل اليكم" ,
                newPhone = updatePhoneDto.NewUserPhone,
                smsResult = SmsResult
            });
        }

        #endregion

        #region Update User Phone Number Confirmation

        [HttpPost]
        [Authorize]
        [Route("UpdateUserPhoneConfirmation")]

        public async Task<ActionResult> UpdateUserPhoneConfirmation(ConfirmUpdatePhoneDto confirmationDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest();

            if (confirmationDto.ActivationCode != user.ActivationCode) return BadRequest(new {message = "كود التفعيل خاطئ"});

            var updateCliamResult = await _authManager.UpdateUserClaim(user, new Claim(ClaimTypes.MobilePhone, confirmationDto.NewPhoneNumber));
            if (!updateCliamResult.Succeeded) return BadRequest(updateCliamResult.Errors);

            var result = await _userManager.SetUserNameAsync(user,confirmationDto.NewPhoneNumber);
            if (!result.Succeeded) return BadRequest(result.Errors);

            user.PhoneNumber = confirmationDto.NewPhoneNumber;
            user.PhoneNumberConfirmed = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) return BadRequest(updateResult.Errors);

            var jwt = await _authManager.CreateJwtToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return Ok(new { message = "تم تغيير رقم الهاتف بنجاح"  , token = tokenString});

        }

        #endregion

        #region Change Password

        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]

        public async Task<ActionResult> ChangePassword(ChangePasswordDto userToResetPassword)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            bool isAuthenticated = await _userManager.CheckPasswordAsync(user, userToResetPassword.OldPassword);
            if (!isAuthenticated)
            {
                return Unauthorized(new { message = "كلمة المرور القديمة غير صالحة" });
            }

            if (userToResetPassword.OldPassword == userToResetPassword.NewPassword)
            {
                return Unauthorized(new { message = "يرجى تقديم كلمة مرور جديدة بدلاً من القديمة" });
            }

            var result = await _userManager.ChangePasswordAsync(user, userToResetPassword.OldPassword, userToResetPassword.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { message = "تم تغيير الرقم السري بنجاح" });
        }

        #endregion

        #region Delete User

        [HttpPost]
        [Authorize]
        [Route("DeleteUser")]

        public async Task<ActionResult> DeleteUser(DeleteUserDto userToDelete)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }
            bool isAuthenticated = await _userManager.CheckPasswordAsync(user, userToDelete.UserPassword);
            if (!isAuthenticated)
            {
                return Unauthorized(new { message = "رمز مرور خاطئ" });
            }

            user.DeletedDate = DateTime.Now;
            user.IsDeleted = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "تم حذف المستخدم بنجاح" });
        }

        #endregion

        #region Forget Password Using Email

        [HttpPost]
        [Route("ForgotPasswordByEmail")]

        public async Task<ActionResult> ForgetPasswordUsingEmail(ForgotPasswordUsingEmailDto credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(credentials.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "لا يوجد مستخدم لديه البريد الإلكتروني المقدم" });
            }
            var isLocked = await _userManager.IsLockedOutAsync(user);
            if (isLocked)
            {
                return Unauthorized(new {message = $"تم تعليق هذا الحساب برجاء المحاوله مرة أخرى" });
            }
            if (user.IsDeleted == true)
            {
                return Unauthorized(new { message = "هذا الحساب تم مسحه من قبل او عدم تفعيله" });
            }

            user.IsForgotPassEmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            string jwtEmailTokenString = _authManager.CreateJwtEmailToken(user);
            string jwtEmailToken = WebUtility.UrlEncode(jwtEmailTokenString);

            var emailToken = _userManager.GeneratePasswordResetTokenAsync(user);
            string emailTokenString = WebUtility.UrlEncode(emailToken.Result);
            
            var confirmationLink = Url.Action(
                action: "ConfirmForgetPasswordByEmail",
                controller: "Users",
                values: new { jwtEmailToken, emailTokenString },
                protocol: Request.Scheme);

            await _emailManager.SendEmail(credentials.Email, "IDSC: Reset Password", $"Please reset your password by clicking the following link: {confirmationLink}");

            var jwt = await _authManager.CreateJwtToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var userTokenString = tokenHandler.WriteToken(jwt);

            return Ok(new { UserToken = userTokenString, EmailToken = emailTokenString });
        }

        #endregion

        #region Confirm Forget Password by Email

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmForgetPasswordByEmail")]
        public async Task<ActionResult> ConfirmForgetPasswordByEmail(string jwtEmailToken, string emailTokenString)
        {
            if (jwtEmailToken == null || emailTokenString == null)
            {
                return BadRequest();
            }

            var decodedJwtEmailToken = WebUtility.UrlDecode(jwtEmailToken);
            var decodedEmailToken = WebUtility.UrlDecode(emailTokenString);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(decodedJwtEmailToken) as JwtSecurityToken;
            var userIdClaim = jsonToken!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            if (userIdClaim == null) return BadRequest();

            ApplicationUser? user = await _userManager.FindByIdAsync(userIdClaim);
            user!.IsForgotPassEmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("Please provide a new password back to the app.");
        }

        #endregion

        #region Reset Password by Email

        [HttpPost]
        [Authorize]
        [Route("ResetPasswordByEmail")]

        public async Task<ActionResult> ResetPasswordByEmail(ResetPasswordDto userNewCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User is not Existed");
            }

            if (user.IsForgotPassEmailConfirmed == false || user.IsForgotPassEmailConfirmed == null)
            {
                return Unauthorized(new { message = "يجب تأكيد البريد الإلكتروني" });
            }

            var result = await _userManager.ResetPasswordAsync(user, userNewCredentials.EmailTokenString, userNewCredentials.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "تم إعادة تعيين كلمة المرور بنجاح" });
        }

        #endregion

        #region Forget Password Using Phone

        [HttpPost]
        [Route("ForgotPasswordByPhone")]
        public async Task<ActionResult> ForgetPasswordUsingPhone(ForgotPasswordUsingPhoneDto credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser? user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user == null)
            {
                return Unauthorized(new { message = "لا يوجد مستخدم بهذا رقم الهاتف" });
            }

            var isLocked = await _userManager.IsLockedOutAsync(user);
            if (isLocked)
            {
                return Unauthorized(new { message = $"تم تعليق هذا الحساب برجاء المحاوله مرة أخرى" });
            }

            if (user.IsDeleted == true)
            {
                return Unauthorized(new { message = "تم مسح هذا المستخدم من قبل" });
            }

            var phoneToken = _userManager.GeneratePasswordResetTokenAsync(user);
            string phoneTokenString = phoneToken.Result;

            var jwt = await _authManager.CreateJwtToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var userTokenString = tokenHandler.WriteToken(jwt);

            var activationCode = new Random().Next(100000, 999999).ToString();
            user.ActivationCode = activationCode;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            _smsManager.SendSMS(credentials.UserName, $"{activationCode}: كود التفعيل");

            return Ok(new { UserToken = userTokenString, ResetPasswordToken = phoneTokenString });
        }

        #endregion

        #region Reset Password Using Phone

        [HttpPost]
        [Authorize]
        [Route("ResetPasswordByPhone")]

        public async Task<ActionResult> ResetPasswordByPhone(ResetPasswordByPhoneDto userNewCredentials)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            if (user.ActivationCode != userNewCredentials.ActivationCode)
            {
                return Unauthorized(new { message = "رمز التفعيل خاطئ" });
            }

            var result = await _userManager.ResetPasswordAsync(user, userNewCredentials.ResetPasswordToken, userNewCredentials.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "تم إعادة تعيين كلمة المرور بنجاح" });
        }

        #endregion

        #region Number to Reactivate Account

        [HttpPost]
        [AllowAnonymous]
        [Route("NumberForReactivateAccount")]

        public async Task<ActionResult> NumberForReactivateAccount(ReactivateAccountDto userToReactivateAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser? user = await _userManager.FindByNameAsync(userToReactivateAccount.UserName);
            if (user == null)
            {
                return Unauthorized(new { message = "لا يوجد مستخدم لدينا بهذا رقم الهاتف" });
            }

            // should add a logic to maintain new user reactivate account concept

            //if (user.IsDeleted == false && user.PhoneNumberConfirmed == true)
            //{
            //    return Unauthorized(new { message = "هذا المستخدم مفعل" });
            //}

            if (user.IsDeleted == true && user.PhoneNumberConfirmed == true)
            {
                TimeSpan? timeOfDeletion = user.DeletedDate - userToReactivateAccount.RequestDate;
                int daysOfDeletion = timeOfDeletion!.Value.Days;
                if (daysOfDeletion >= 30)
                {
                    return Unauthorized(new { message = "تم مسح هذا المستخدم برجاء انشاء حساب آخر" });
                }

                var activationCode = new Random().Next(100000, 999999).ToString();
                user.ActivationCode = activationCode;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                _smsManager.SendSMS(userToReactivateAccount.UserName, $"{activationCode}:  كود التفعيل الخاص بكم");

                var jwt = await _authManager.CreateJwtToken(user);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenString = tokenHandler.WriteToken(jwt);

                return Ok(new { UserToken = tokenString });
            }

            if (user.IsDeleted == false && user.PhoneNumberConfirmed == false)
            {
                var activationCode = new Random().Next(100000, 999999).ToString();
                user.ActivationCode = activationCode;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                _smsManager.SendSMS(userToReactivateAccount.UserName, $"{activationCode}: كود التفعيل");

                var jwt = await _authManager.CreateJwtToken(user);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenString = tokenHandler.WriteToken(jwt);

                return Ok(new { UserToken = tokenString });
            }

            return Unauthorized(new { message = "هذا المستخدم مفعل" });
        }

        #endregion

        #region Reactivate Account

        [HttpPost]
        [Authorize]
        [Route("ReactivateAccount")]

        public async Task<ActionResult> ReactivateAccount(ConfirmPhoneDto userToConfirmPhone)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            if (userToConfirmPhone.ActivationCode != user.ActivationCode)
            {
                return Unauthorized(new { message = "رمز التفعيل خاطئ" });
            }

            //if (user.IsDeleted == false)
            //{
            //    return BadRequest(new { message = "هذا المستخدم مفعل" });
            //}

            user.IsDeleted = false;
            user.PhoneNumberConfirmed = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "تم تفعيل هذا المستخدم بنجاح" });
        }

        #endregion

        #region Register Using Email and Phone number together

        [HttpPost]
        [Route("RegisterByEmailAndPhone")]

        public async Task<ActionResult> RegisterByEmailAndPhone([FromForm(Name = "file")] IFormFile? ProfilePicture, [FromForm(Name = "register")] RegisterDto registrDto)
        {
            var activationCode = new Random().Next(100000, 999999).ToString();
            var newUser = new ApplicationUser
            {
                UserName = registrDto.UserName,
                Email = registrDto.Email,
                PhoneNumber = registrDto.UserName,
                FullName = registrDto.FullName,
                Birthdate = registrDto!.Birthdate,
                IsDeleted = false,
                IsForgotPassEmailConfirmed = false,
                DeletedDate = null,
                CreateDate = DateTime.Now,
                JobTitle = registrDto.JobTitle,
                ActivationCode = activationCode
            };

            var isEmailExisted = await _userManager.FindByEmailAsync(registrDto.Email);
            if (isEmailExisted is not null)
            {
                return BadRequest(new { message = "تم استخدام البريد الالكتروني من قبل" });
            }

            var result = await _userManager.CreateAsync(newUser, registrDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            string jwtEmailTokenString = _authManager.CreateJwtEmailToken(newUser);
            var jwtEmailToken = WebUtility.UrlEncode(jwtEmailTokenString);

            var emailTokenString = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var emailToken = WebUtility.UrlEncode(emailTokenString);

            var confirmationLink = Url.Action(
                action: "ConfirmEmailByEmailAndPhone",
                controller: "Users",
                values: new { jwtEmailToken, emailToken },
                protocol: Request.Scheme);

            string emailMsg = $"Please confirm your email by clicking the following link: {confirmationLink}";

            var emailResult = await _emailManager.SendEmail(newUser.Email, "IDSC nabta", emailMsg);

            var SmsResult = _smsManager.SendSMS(newUser.PhoneNumber, $"{activationCode}: كود التفعيل الخاص بكم");

            string? uploadImageResult = string.Empty;

            if (ProfilePicture != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                bool isImage = allowedExtensions.Contains(Path.GetExtension(ProfilePicture.FileName).ToLower());
                {
                    if (isImage)
                    {
                        uploadImageResult = await _fileManager.UploadFile(ProfilePicture, newUser.Id, $"user{newUser.Id}", "user");
                        if (uploadImageResult is not null)
                        {
                            newUser.ProfilePicture = _fileManager.RenameFile(ProfilePicture, $"user{newUser.Id}");
                            await _userManager.UpdateAsync(newUser);
                        }
                    }
                }
            }


            var jwt = await _authManager.CreateJwtToken(newUser);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return Ok(new
            {
                smsResult = SmsResult,
                uploadedImageNamed = uploadImageResult,
                token = tokenString,
                EmailResult = emailResult,
            });
        }

        #endregion

        #region Confirm Email by register phone and email

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmailByEmailAndPhone")]
        public async Task<IActionResult> ConfirmEmailByEmailAndPhone(string jwtEmailToken, string emailToken)
        {
            if (jwtEmailToken == null || emailToken == null)
            {
                return BadRequest();
            }
            var encodedJwtEmailToken = WebUtility.UrlDecode(jwtEmailToken);
            var encodedEmailToken = WebUtility.UrlDecode(emailToken);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(encodedJwtEmailToken) as JwtSecurityToken;
            var userIdClaim = jsonToken!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            if (userIdClaim == null) return BadRequest();


            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, encodedEmailToken);
            if (result.Succeeded)
            {
                return Ok("Email Confirmed Successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        #endregion

        #region Confirm Phone by register phone and email

        [HttpPost]
        [Authorize]
        [Route("ConfirmPhoneByEmailAndPhone")]
        public async Task<ActionResult> ConfirmPhoneByEmailAndPhone(ConfirmPhoneDto userToConfirmPhone)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (userId == 0)
            {
                return NotFound(new {message = "No user found."});
            }
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return BadRequest(new { message = "Invalid User" });
            }
            if (userToConfirmPhone.ActivationCode != user.ActivationCode)
            {
                return BadRequest(new { message = "رمز التفعيل خاطئ" });
            }

            user.PhoneNumberConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "تم تأكيد رقم الهاتف بنجاح" });
            }
            return BadRequest(result.Errors);
        }

        #endregion

        #region Login by confirming mail and phone

        [HttpPost]
        [Route("Login_2FA")]
        public async Task<ActionResult<TokenDto>> Login_2FA(LoginDto credentials)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user is null)
            {
                return Unauthorized(new {message = "هاتف المستخدم هذا غير موجود" });
            }

            var isLocked = await _userManager.IsLockedOutAsync(user);
            if (isLocked)
            {
                return Unauthorized(new { message = $"تم تعليق هذا الحساب برجاء المحاوله مرة أخرى" });
            }

            if (user.IsDeleted == true)
            {
                return Unauthorized(new { message = $"هذا الحساب تم مسحه من قبل او عدم تفعيله" });
            }

            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!isEmailConfirmed)
            {
                return BadRequest(new { message = $"يجب تأكيد تفعيل البريد الالكتروني" });
            }
            var isPhoneConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
            if (!isPhoneConfirmed)
            {
                return Unauthorized(new { message = $"يجب تأكيد تفعيل رقم الهاتف عن طريق كود التفعيل المرسل اليكم" });
            }
            bool isAuthenticated = await _userManager.CheckPasswordAsync(user, credentials.Password);
            if (!isAuthenticated)
            {
                await _userManager.AccessFailedAsync(user);
                return Unauthorized(new { message = "رمز مرور خاطئ" });
            }

            var jwt = await _authManager.CreateJwtToken(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return new TokenDto
            {
                Token = tokenString,
                Expiry = jwt.ValidTo
            };
        }

        #endregion

        #region Send SMS Test

        [HttpPost]
        [Route("TestSms")]

        public ActionResult TestSMS([FromBody] string phone)
        {
            try
            {
                var result = _smsManager.SendSMS(phone, "Test SMS service");

                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Upload Image Test

        [HttpPost]
        [Route("UploadFileTest")]

        public ActionResult UploadFile([FromForm(Name = "file")] IFormFile formFile)
        {
            var file = _fileManager.UploadFile(formFile, 14, "test","user");
            return Ok("File Uploaded");
        }
        #endregion

        #region Test Send Email

        [HttpGet]
        [Route("send_email_test")]

        public async Task<ActionResult> SendEmailTest()
        {
            // just test the service

            var isSent = await _emailManager.SendEmail("idsc44139@gmail.com", "Khaled Test Sending", "HIIIII !!!!! email is sent.");
            if (isSent) return Ok();
            
            return BadRequest();
        }
        #endregion

        #region LikedPosts 

        [HttpGet]
        [Authorize]
        [Route("liked_posts")]

        public async Task<ActionResult<string[]>> GetLikedPosts()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            string? likedPostsString = user.LikedPosts;
            if (likedPostsString is null) return NoContent();

            string[] likedPosts = likedPostsString.Split(',');

            return Ok(likedPosts);
        }

        #endregion

        #region Social Login register by Google

        [HttpPost]
        [Route("register_by_google_signin")]
        public async Task<ActionResult<AuthModel>> RegisterGoogleSignIn([FromBody] string token)
        {
            
            var registerResult = await _socialLoginManager.RegisterUserByGoogleAuth(token);

            if (!registerResult.IsAuthenticated)
            {
                return BadRequest(registerResult.Message);
            }

            return Ok(registerResult);

            //// 116130660745 - 7istq3n0f7kt1allo6bo2ajt64e67vm4.apps.googleusercontent.com
            //// audience uses ClientId so should bring it using configuration services

            //try
            //{
            //    var payload = await GoogleJsonWebSignature.ValidateAsync(token, new ValidationSettings { Audience = new List<string> { "116130660745-4orcf7njvej2q7n5e28vkjoovu3n6tvu.apps.googleusercontent.com" } });
            //    var isUserExisted = _userManager.FindByEmailAsync(payload.Email);
            //    if (isUserExisted != null) return BadRequest(new {message = "User is existed already"});

            //    ApplicationUser user = new ApplicationUser()
            //    {
            //        FullName = payload.Name,
            //        Email = payload.Email,
            //        EmailConfirmed = payload.EmailVerified,
            //        ProfilePicture = payload.Picture,
            //    };
            //    var isUserCreated = await _userManager.CreateAsync(user);
            //    if (!isUserCreated.Succeeded)
            //    {
            //        return BadRequest(isUserCreated.Errors);
            //    }

            //    UserLoginInfo userLoginInfo = new UserLoginInfo("google", payload.Subject, "GOOGLE");

            //    var registerResult = await _userManager.AddLoginAsync(user, userLoginInfo);
            //    if (!registerResult.Succeeded)
            //    {
            //        return BadRequest(registerResult.Errors);
            //    }

            //    return Ok(new { token = "" });
            //}
            //catch (Exception ex)
            //{
            //    var message = ex.Message.ToString();

            //    return BadRequest(message);
            //}
        }

        #endregion

        //#region Sign out

        //[HttpPost]
        //[Authorize]
        //[Route("sign_out")]

        //public async Task<ActionResult> SignUserOut()
        //{
        //    await _signInManager.SignOutAsync();
        //    return Ok();
        //}

        //#endregion
    }
}

