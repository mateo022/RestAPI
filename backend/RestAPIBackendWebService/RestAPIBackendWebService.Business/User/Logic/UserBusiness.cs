using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestAPIBackendWebService.Business.Mailer.Contracts;
using RestAPIBackendWebService.Business.Role.Contracts;
using RestAPIBackendWebService.Business.User.Contracts;
using RestAPIBackendWebService.Domain.Auth.Constants;
using RestAPIBackendWebService.Domain.Auth.Entities;
using RestAPIBackendWebService.Domain.Auth.Models;
using RestAPIBackendWebService.Domain.Role.Constants;
using RestAPIBackendWebService.Domain.Services.Localization;
using RestAPIBackendWebService.Domain.Services.Mailing;
using RestAPIBackendWebService.Domain.User.Constants;
using RestAPIBackendWebService.Domain.User.DTOs;
using RestAPIBackendWebService.Domain.User.Models;
using System.Security.Claims;


namespace RestAPIBackendWebService.Business.User.Logic
{
    public class UserBusiness : IUserBusiness
    {
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly IRoleBusiness _roleBusiness;
        private readonly IMailerBusiness _mailerBusiness;
        private readonly IPasswordValidator<CustomIdentityUser> _passwordValidator;

        private readonly Func<IdentityError, string> _errorsMessagesMapper = e => e.Description;
        public UserBusiness(
            UserManager<CustomIdentityUser> userManager,
            IMailerBusiness mailerBusiness,
            IRoleBusiness roleBusiness,
            IPasswordValidator<CustomIdentityUser> passwordValidator)
        {
            _mailerBusiness = mailerBusiness;
            _userManager = userManager;
            _roleBusiness = roleBusiness;
            _passwordValidator = passwordValidator;
        }

        public async Task<List<BasicUserInformation>> GetAllUsersInformation(string requesterEmail)
        {
            var requesterUser = await _userManager.FindByEmailAsync(requesterEmail);
            var requesterRoles = await _userManager.GetRolesAsync(requesterUser);
            var result = new List<BasicUserInformation>();

            if (requesterUser != null && requesterRoles != null)
            {
                var users = await _userManager.Users.ToListAsync(); // Obtener todos los usuarios

                foreach (var user in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // Agregar información básica del usuario
                    result.Add(new BasicUserInformation
                    {
                        Email = user.Email,
                        Name = user.UserName,
                        Cellphone = user.PhoneNumber,
                        EmailConfirmed = user.EmailConfirmed,
                        Roles = userRoles.ToList()
                    });
                }
            }

            return result;
        }
        public async Task<IdentityResult> InitialRegisterAsync(NewUserRequestDTO newUserData, ClaimsIdentity requesterUserClaims)
        {
            var user = new CustomIdentityUser { Email = newUserData.Email, UserName = newUserData.Name };
            var requesterUserRoles = _roleBusiness.GetUserRolesByClaims(requesterUserClaims);


            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                if (_roleBusiness.CanAssignRole(newUserData.RoleName, requesterUserRoles))
                {
                    _ = await _userManager.AddToRoleAsync(user, newUserData.RoleName);
                }

                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //Send email with link
                _ = await _mailerBusiness.SendRegisterEmailAsync(new AuthEmail
                {
                    Token = confirmationToken,
                    Email = user.Email
                });

            }

            return result;
        }

        public CustomIdentityUser GetUserByEmailNotConfirmedAsync(string email)
        {
            var user = _userManager.Users.Where(u => u.Email.Equals(email)).FirstOrDefault();

            // Validate if user is not confirmed
            if (user != null && !user.EmailConfirmed)
                return user;

            return null;
        }
        public async Task<UserConfirmationResult> ConfirmNewUserInformationAsync(CompleteUserRegisterRequestDTO newUserData)
        {
            var result = new UserConfirmationResult();
            result.Success = true;

            var user = await _userManager.FindByEmailAsync(newUserData.Email);

            if (user != null)
            {
                var passwordValidationResult = await _passwordValidator.ValidateAsync(_userManager, null, newUserData.Password);

                user.PhoneNumber = newUserData.PhoneNumber;
                user.PhoneNumberIndicator = newUserData.PhoneNumberIndicator;

                var updatePhoneNumberResult = await _userManager.UpdateAsync(user);

                result.ErrorsList.AddErrorsMessagesFromIdentityResult(passwordValidationResult, _errorsMessagesMapper, AuthRequestLabels.PASSWORD);
                result.ErrorsList.AddErrorsMessagesFromIdentityResult(updatePhoneNumberResult, _errorsMessagesMapper, UserRequestLabels.PHONE_NUMBER);

                result.Success = passwordValidationResult.Succeeded &&
                    updatePhoneNumberResult.Succeeded;

                if (result.Success)
                {
                    var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, newUserData.Token);
                    var removePasswordresult = await _userManager.RemovePasswordAsync(user);
                    var changePasswordresult = await _userManager.AddPasswordAsync(user, newUserData.Password);

                    result.ErrorsList.AddErrorsMessagesFromIdentityResult(confirmEmailResult, _errorsMessagesMapper, UserRequestLabels.EMAIL);
                    result.ErrorsList.AddErrorsMessagesFromIdentityResult(removePasswordresult, _errorsMessagesMapper, UserRequestLabels.EMAIL);
                    result.ErrorsList.AddErrorsMessagesFromIdentityResult(changePasswordresult, _errorsMessagesMapper, UserRequestLabels.EMAIL);

                    result.Success = result.Success &&
                    confirmEmailResult.Succeeded;
                }
            }
            else
            {
                result.ErrorsList.AddErrorForKey(UserRequestLabels.COMMON_USER, string.Format(ApplicationTranslations.IdentityErrors["UserDontExists"], newUserData.Email));
            }

            return result;
        }

        public async Task<CustomIdentityUser> GetUserInformationForEditByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
          
            return user;
        }

        public async Task<EditUserResult> EditUserInformation(EditUserInformationRequestDTO userNewInfo)
        {
            var result = new EditUserResult();
            result.Success = false;

            var user = await _userManager.FindByEmailAsync(userNewInfo.Email);

            if (user != null)
            {
                var generateResetPasswordToken = "";
                IdentityResult changePasswordResult = null;

                if (!String.IsNullOrEmpty(userNewInfo.Password) &&
                    !String.IsNullOrEmpty(userNewInfo.ConfirmPassword))
                {
                    generateResetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    changePasswordResult = await _userManager.ResetPasswordAsync(user, generateResetPasswordToken, userNewInfo.Password);

                    result.ErrorsList.AddErrorsMessagesFromIdentityResult(changePasswordResult, _errorsMessagesMapper, AuthRequestLabels.PASSWORD);
                }

                user.PhoneNumber = userNewInfo.PhoneNumber;
                user.PhoneNumberIndicator = userNewInfo.PhoneNumberIndicator;
                user.UserName = userNewInfo.Name;
                user.Email = userNewInfo.Email;

                var updateUserInfoResult = await _userManager.UpdateAsync(user);

                result.ErrorsList.AddErrorsMessagesFromIdentityResult(updateUserInfoResult, _errorsMessagesMapper, UserRequestLabels.PHONE_NUMBER);

                result.Success = (changePasswordResult == null || changePasswordResult.Succeeded) &&
                    updateUserInfoResult.Succeeded;

                if (!result.Success)
                {
                    result.ErrorsList.AddErrorForKey(UserRequestLabels.COMMON_USER, string.Format(ApplicationTranslations.IdentityErrors["UserDontExists"], userNewInfo.Email));
                }
            }
           

            return result;
        }
        public async Task<UserDeleteResult> DeleteUserByEmail(string userToDeleteEmail, string requesterEmail, List<string> requesterRoles)
        {
            var result = new UserDeleteResult
            {
                Success = false
            };

            var userToDelete = await _userManager.FindByEmailAsync(userToDeleteEmail);
            var requesterUser = await _userManager.FindByEmailAsync(requesterEmail);

            if (userToDelete != null)
            {
                var userToDeleteRoles = await _userManager.GetRolesAsync(userToDelete);

                var userToDeleteHighestRole = RolesHierarchy.GetHighestRoleInHierarchy(userToDeleteRoles.ToList());
                var requesterHighestRole = RolesHierarchy.GetHighestRoleInHierarchy(requesterRoles);

                // Validate if requester can delete specified user
                if (userToDeleteHighestRole.Key < requesterHighestRole.Key)
                {
                    try
                    {
                        var identityDeleteOperationresult = await _userManager.DeleteAsync(userToDelete);

                        if (!identityDeleteOperationresult.Succeeded)
                        {
                            foreach (var error in identityDeleteOperationresult.Errors)
                            {
                                result.Errors.Add(error.Description);
                            }
                        }
                        else
                        {
                            result.Success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Error al eliminar el usuario: {ex.Message}");
                    }
                }
                else
                {
                    result.Errors.Add("El usuario solicitante no tiene permisos suficientes para eliminar al usuario especificado.");
                }
            }
            else
            {
                result.Errors.Add($"El usuario '{userToDeleteEmail}' no existe en el sistema.");
            }

            return result;
        }
        public async Task<IdentityResult> RegisterAndConfirmUserAsync(NewUserRequestDTO newUserData, ClaimsIdentity requesterUserClaims)
        {
            // Crear el usuario
            var user = new CustomIdentityUser { Email = newUserData.Email, UserName = newUserData.Name };
            var requesterUserRoles = _roleBusiness.GetUserRolesByClaims(requesterUserClaims);

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // Asignar rol si es permitido
                if (_roleBusiness.CanAssignRole(newUserData.RoleName, requesterUserRoles))
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(user, newUserData.RoleName);
                    if (!addRoleResult.Succeeded)
                    {
                        result = IdentityResult.Failed(addRoleResult.Errors.ToArray());
                        return result;
                    }
                }

                // Validar contraseña
                var passwordValidationResult = await _passwordValidator.ValidateAsync(_userManager, null, newUserData.Password);
                if (!passwordValidationResult.Succeeded)
                {
                    return passwordValidationResult;
                }

                // Actualizar número de teléfono
                user.PhoneNumber = newUserData.PhoneNumber;
                user.PhoneNumberIndicator = newUserData.PhoneNumberIndicator;

                var updatePhoneNumberResult = await _userManager.UpdateAsync(user);
                if (!updatePhoneNumberResult.Succeeded)
                {
                    return updatePhoneNumberResult;
                }

                // Confirmar email sin token
                user.EmailConfirmed = true;
                var updateEmailConfirmedResult = await _userManager.UpdateAsync(user);
                if (!updateEmailConfirmedResult.Succeeded)
                {
                    return updateEmailConfirmedResult;
                }

                // Cambiar contraseña
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    return removePasswordResult;
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, newUserData.Password);
                if (!addPasswordResult.Succeeded)
                {
                    return addPasswordResult;
                }
            }

            return result;
        }
    }
}
