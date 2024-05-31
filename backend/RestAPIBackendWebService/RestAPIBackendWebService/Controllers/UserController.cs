using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RestAPIBackendWebService.Business.User.Contracts;
using RestAPIBackendWebService.Domain.Common.DTOs;
using RestAPIBackendWebService.Domain.Common.Errors;
using RestAPIBackendWebService.Domain.User.DTOs;
using System.Net;
using System.Security.Claims;

namespace RestAPIBackendWebService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [EnableCors]
    public class UserController : ControllerBase
    {
        public IUserBusiness _userBusiness;

        public UserController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [HttpGet()]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> GetUsersInformation()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var users = await _userBusiness.GetAllUsersInformation(userEmail);

            if (users != null && users.Any())
            {
                return Ok(new ResponseDTO
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Data = users,
                    Message = "Success"
                });
            }

            return NotFound();
        }

        [HttpPost("user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> InitialRegister(NewUserRequestDTO newUserData)
        {
            var claimsIdentity = (HttpContext.User.Identity as ClaimsIdentity);

            var registerResult = await _userBusiness.InitialRegisterAsync(newUserData, claimsIdentity);

            if (registerResult.Succeeded)
            {
                return Created(string.Empty, new ResponseDTO
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Message = "Success",
                });
            }
            else
            {
                return BadRequest(new ErrorResponseDTO<Dictionary<string, List<string>>>
                {
                    Message = "Error",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Errors = RequestFieldsErrorsCollection.MapIdentityErrorsByCommonKey(registerResult.Errors)
                });
            }
        }
        [HttpPost("userNew")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> CreateNewUser(NewUserRequestDTO newUserData)
        {
            var claimsIdentity = (HttpContext.User.Identity as ClaimsIdentity);

            var registerResult = await _userBusiness.RegisterAndConfirmUserAsync(newUserData, claimsIdentity);

            if (registerResult.Succeeded)
            {
                return Created(string.Empty, new ResponseDTO
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Message = "Success",
                });
            }
            else
            {
                return BadRequest(new ErrorResponseDTO<Dictionary<string, List<string>>>
                {
                    Message = "Error",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Errors = RequestFieldsErrorsCollection.MapIdentityErrorsByCommonKey(registerResult.Errors)
                });
            }
        }

        [HttpGet("user/confirmation-user-data")]
        public ActionResult<ResponseDTO> GetUserConfirmationDataByEmail([FromQuery] string email)
        {
            var user = _userBusiness.GetUserByEmailNotConfirmedAsync(email);

            if (user != null)
            {
                return Ok(new ResponseDTO
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Data = new UserDataForRegisterCompletionResponseDTO
                    {
                        Email = user.Email,
                        Name = user.UserName,
                    }
                });
            }

            return NotFound();
        }

        [HttpPost("confirm")]
        public async Task<ActionResult<ResponseDTO>> CompleteRegistration(CompleteUserRegisterRequestDTO newUserData)
        {
            var confirmationResult = await _userBusiness.ConfirmNewUserInformationAsync(newUserData);

            if (confirmationResult.Success && confirmationResult.ErrorsList.Collection.Count() == 0)
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                return BadRequest(new ErrorResponseDTO<Dictionary<string, List<string>>>
                {
                    Errors = confirmationResult.ErrorsList.Collection,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Failed"
                });
            }
        }
        [HttpGet("user/basic-info")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> GetUserBasicInformation()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userBusiness.GetUserInformationForEditByEmail(userEmail);

            if (user != null)
            {
                return Ok(new ResponseDTO
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Data = new BasicUserInformationResponseDTO
                    {
                        Email = user.Email,
                        Name = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberIndicator = user.PhoneNumberIndicator
                    },
                    Message = "Success"
                });
            }

            return NotFound();
        }
        [HttpPut("user")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> EditUserInformation(EditUserInformationRequestDTO newUserData)
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);
            newUserData.Email = userEmail;

            var editResult = await _userBusiness.EditUserInformation(newUserData);

            if (editResult.Success && editResult.ErrorsList.Collection.Count() == 0)
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                return BadRequest(new ErrorResponseDTO<Dictionary<string, List<string>>>
                {
                    Errors = editResult.ErrorsList.Collection,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Failed"
                });
            }
        }

        [HttpDelete("user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> DeleteUserByEmail([FromQuery] string email)
        {
            var userClaims = (HttpContext.User.Identity as ClaimsIdentity);
            var userEmail = userClaims.FindFirst(ClaimTypes.Email).Value;
            var requesterRoles = userClaims.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var deleteResult = await _userBusiness.DeleteUserByEmail(email, userEmail, requesterRoles);

            if (deleteResult.Success)
            {
                return Ok(new ResponseDTO
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Data = email,
                    Message = "Success"
                });
            }

            return BadRequest(new ErrorResponseDTO<List<string>>
            {
                Errors = deleteResult.Errors,
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Failed"
            });
        }
    }
}
