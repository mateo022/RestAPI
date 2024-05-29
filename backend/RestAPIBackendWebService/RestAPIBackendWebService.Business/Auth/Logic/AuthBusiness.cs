using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestAPIBackendWebService.Business.Logger.Contracts;
using RestAPIBackendWebService.Business.Mailer.Contracts;
using RestAPIBackendWebService.DataAccess;
using RestAPIBackendWebService.DataAccess.APIs.Softipal.Auth.Contracts;
using RestAPIBackendWebService.Domain.Auth.Constants;
using RestAPIBackendWebService.Domain.Auth.DTOs;
using RestAPIBackendWebService.Domain.Auth.Entities;
using RestAPIBackendWebService.Domain.Auth.Models;
using RestAPIBackendWebService.Domain.Services.Localization;
using RestAPIBackendWebService.Domain.Services.Mailing;
using RestAPIBackendWebService.Domain.User.Constants;
using RestAPIBackendWebService.Domain.User.DTOs;
using RestAPIBackendWebService.Services.Security.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace RestAPIBackendWebService.Business.Auth.Contracts

{
    public class AuthBusiness : IAuthBusiness
    {
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly IMailerBusiness _mailerBusiness;
        private readonly RestAPIDbContext _dbContext;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ITwoAuth _apiSoftiAuth;
        private readonly ILoggerBusiness _loggerBusiness;
        private readonly IConfiguration _configuration;



        public AuthBusiness(
            UserManager<CustomIdentityUser> userManager,
            RestAPIDbContext dbContext,
            IPasswordHasherService passwordHasher,
            ITwoAuth apiSoftiAuth,
            IConfiguration configuration,
            ILoggerBusiness loggerBusiness,
            IMailerBusiness mailerBusiness)
        {
            _userManager = userManager;
            _mailerBusiness = mailerBusiness;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _apiSoftiAuth = apiSoftiAuth;
            _loggerBusiness = loggerBusiness;
            _configuration = configuration;
        }

        public async Task<LoginResultAuth> Login(LoginRequestDTO request)
        {
            var result = new LoginResultAuth
            {
                Success = false // Inicialmente asumimos que el inicio de sesión fallará
            };

            var user = await _userManager.FindByEmailAsync(request.Email);
            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (user == null)
            {
                result.ErrorsList.AddErrorForKey(UserRequestLabels.EMAIL, String.Format(ApplicationTranslations.IdentityErrors["UserDontExists"], request.Email));
                result.Success = false;

                return result;
            }

            if (!user.EmailConfirmed)
            {
                result.ErrorsList.AddErrorForKey(UserRequestLabels.EMAIL, String.Format(ApplicationTranslations.IdentityErrors["NotConfirmedUser"], request.Email));
                result.Success = false;

                return result;
            }

            if (!isValidPassword)
            {
                result.ErrorsList.AddErrorForKey(AuthRequestLabels.PASSWORD, ApplicationTranslations.IdentityErrors["PasswordMismatch"]);
                result.Success = false;

                return result;
            }

            // Actualizar el código de doble factor
            user.TwoFactorCode = GenerateRandomNumber().ToString();
            _dbContext.SaveChanges();
            // Establecer la fecha de expiración, 5 minutos desde ahora
            user.TwoFactorExpireDate = DateTime.Now.AddMinutes(5);
            _dbContext.SaveChanges();

            var phoneNumberWithIndicator = $"{user.PhoneNumberIndicator}{user.PhoneNumber}";

            //// Enviar el código de doble factor
            var successCodeMessage = await SendMessageCode(phoneNumberWithIndicator, user.TwoFactorCode);

            if (!successCodeMessage)
            {
                result.ErrorsList.AddErrorForKey("Mensaje texto", "No se pudo enviar el mensaje con el código de doble factor de autenticación");
                result.Success = false;
                return result;
            }
            var roles = await _userManager.GetRolesAsync(user);
           
            result.Success = true;
            if (result.Success)
            {

                result.UserInformation = new Domain.User.Models.BasicUserInformation
                {
                    Email = user.Email,
                    Name = user.UserName,
                    Cellphone = user.PhoneNumber,
                    Roles = roles.ToList()
                };
            }

            return result;
        }
   
        private async Task<bool> SendMessageCode(string phoneNumber, string randomNumber)
        {

            var requestBody = new TwoAuthRequestDTO
            {
                Token = _configuration["token_TwoFactorCode"],
                PhoneNumber = phoneNumber,
                Message = $"Su código de verificación es: {randomNumber}"
            };

            var result = await _apiSoftiAuth.TwoFactorCode(requestBody);

            return result;
        }


        public async Task<LoginResultAuth> VerifyTwoFactorCode(string phoneNumber, string code)
        {
            var result = new LoginResultAuth
            {
                Success = true
            };

            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(code))
            {
                result.ErrorsList.AddErrorForKey("Campos", "Número de teléfono y código de autenticación de dos factores son requeridos.");
                result.Success = false;
                return result;
            }

            // Buscar el usuario en la base de datos por el número de teléfono
            var foundUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            // Verificar si se encontró al usuario
            if (foundUser == null)
            {
                result.ErrorsList.AddErrorForKey(phoneNumber, "Usuario no encontrado. Verifique sus credenciales.");
                result.Success = false;
                return result;
            }

            // Verificar si el código de autenticación de dos factores ha expirado
            if (foundUser.TwoFactorExpireDate < DateTime.Now)
            {
                result.ErrorsList.AddErrorForKey(code, "El código de autenticación de dos factores ha expirado.");
                result.Success = false;
                return result;
            }

            // Verificar si el código proporcionado coincide con el código almacenado en la base de datos
            if (!foundUser.TwoFactorCode.Equals(code))
            {
                result.ErrorsList.AddErrorForKey(code, "Código de autenticación de dos factores inválido.");
                result.Success = false;
                return result;
            }

            // Si todo está bien, proporcionar la información del usuario en el resultado
            var roles = await _userManager.GetRolesAsync(foundUser);
            result.UserInformation = new Domain.User.Models.BasicUserInformation
            {
                Email = foundUser.Email,
                Name = foundUser.UserName,
                Cellphone = foundUser.PhoneNumber,
                Roles = roles.ToList()
            };
            result.Token = BuildToken(new LoginRequestDTO { Email = foundUser.Email }, roles.ToList(), foundUser.Id);
            return result;
        }
        private AuthTokenModel BuildToken(LoginRequestDTO userCredentials, List<string> roles, string userId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, userCredentials.Email),
                new Claim(CustomClaimTypes.UserId, userId)
            };

            //Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["tokensKey"]));

            var applicationCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["SessionDuration"]));

            var securityToken = new JwtSecurityToken
                (issuer: _configuration["jwtSettings:validIssuer"], 
                audience: null,
                claims: claims, 
                expires: expiration, 
                signingCredentials: 
                applicationCredentials);

            return new AuthTokenModel
            {
                TokenValue = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        public async Task<SendMailResult> ForgotPasswordEmailGenerationAsync(UserEmailDTO userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail.Email);
            SendMailResult result;

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                result = await _mailerBusiness.SendPasswordRecoveryEmailAsync(new AuthEmail
                {
                    Email = userEmail.Email,
                    Token = token
                });
            }
            else
            {
                result = new SendMailResult
                {
                    Message = String.Format(ApplicationTranslations.IdentityErrors["UserDontExists"], userEmail.Email),
                    Success = false
                };
            }

            return result;
        }

        public async Task LogoutAsync (string userEmail)
        {
            await _loggerBusiness.LogAuthAction(AuthLogType.LOGOUT, new AuthLogData
            {
                userEmail = userEmail,
            });
        }
        public async Task<RecoveryPasswordResult> ResetpasswordAsync(RecoveryPasswordRequestDTO recoveryPasswordRequestData)
        {
            var user = await _userManager.FindByEmailAsync(recoveryPasswordRequestData.Email);
            var result = new RecoveryPasswordResult
            {
                Success = false
            };

            if (user != null)
            {
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, recoveryPasswordRequestData.Token, recoveryPasswordRequestData.Password);

                result.Success = resetPasswordResult.Succeeded;

                Func<IdentityError, string> errorsMessagesMapper = e => e.Description;

                result.ErrorsList.AddErrorsMessagesFromIdentityResult(resetPasswordResult, errorsMessagesMapper, UserRequestLabels.EMAIL);

            }
            else
            {
                result.ErrorsList.AddErrorForKey(UserRequestLabels.COMMON_USER, string.Format(ApplicationTranslations.IdentityErrors["UserDontExists"], recoveryPasswordRequestData.Email));
            }

            return result;
        }

        private int GenerateRandomNumber()
        {
            //Generate random number using Random
            Random random = new Random();
            return random.Next(100000, 999999);
        }
    }

}
