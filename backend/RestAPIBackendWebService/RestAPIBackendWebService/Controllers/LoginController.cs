using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPIBackendWebService.Business.Auth.Contracts;
using RestAPIBackendWebService.Domain.Auth.Constants;
using RestAPIBackendWebService.Domain.Auth.DTOs;
using RestAPIBackendWebService.Domain.Common.DTOs;
using RestAPIBackendWebService.Domain.User.DTOs;
using System.Net;
using System.Security.Claims;

namespace RestAPIBackendWebService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/login")]
    public class LoginController : ControllerBase
    {
        public readonly IAuthBusiness _authBusiness;

        public LoginController(IAuthBusiness authBusiness)
        {
            _authBusiness = authBusiness;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDTO>> LoginMethod(LoginRequestDTO request)
        {
            var result = await _authBusiness.Login(request);

            if (result.Success)
            {
                return Ok(new ResponseDTO
                {
                    Data = result,
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK
                });
            }


            return BadRequest(new ErrorResponseDTO<Dictionary<string, List<string>>>
            {
                Message = "Failed",
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Errors = result.ErrorsList.Collection
            });
        }

        [HttpPost("verify-two-factor-code")]
        public async Task<ActionResult<ResponseDTO>> VerifyTwoFactorCode(VerifyTwoFactorCodeRequestDTO request)
        {
            
            var tokenResult = await _authBusiness.VerifyTwoFactorCode(request.PhoneNumber, request.Code);

            if (tokenResult.Success)
            {
                // La verificación fue exitosa, devolver el token JWT
                return Ok(new ResponseDTO
                {
                    Data = tokenResult,
                    Message = "Token JWT generado exitosamente.",
                    StatusCode = (int)HttpStatusCode.OK
                });
            }
            else
            {
                // La verificación falló, devolver los errores
                return Unauthorized(new ErrorResponseDTO<Dictionary<string, List<string>>>
                {
                    Message = "Failed",
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Errors = tokenResult.ErrorsList.Collection
                });
            }
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO>> Logout()
        {
            var userClaims = (HttpContext.User.Identity as ClaimsIdentity);
            var userEmail = userClaims.FindFirst(ClaimTypes.Email).Value;
            var userId = userClaims.FindFirst(CustomClaimTypes.UserId).Value;

            await _authBusiness.LogoutAsync(userEmail);

            return NoContent();
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ResponseDTO>> ForgotPassword(UserEmailDTO userEmail)
        {
            var forgotPasswordResult = await _authBusiness.ForgotPasswordEmailGenerationAsync(userEmail);

            if (forgotPasswordResult.Success)
            {
                return Ok(new ResponseDTO
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = forgotPasswordResult.Message
                });
            }
            else
            {
                return BadRequest(new ErrorResponseDTO
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = forgotPasswordResult.Message
                });
            }
        }
        //[HttpPost("restore-password")]
        //public async Task<ActionResult<ResponseDTO>> RecoveryPassword(RecoveryPasswordRequestDTO recoveryPasswordData)
        //{
        //    var recoveryPasswordResult = await _authBusiness.ResetpasswordAsync(recoveryPasswordData);

        //    if (recoveryPasswordResult.Success)
        //    {
        //        return Ok(new ResponseDTO
        //        {
        //            Data = recoveryPasswordResult,
        //            StatusCode = (int)HttpStatusCode.OK,
        //            Message = "Success"
        //        });
        //    }
        //    else
        //    {
        //        return BadRequest(new ErrorResponseDTO<Dictionary<string, List<string>>>
        //        {
        //            StatusCode = (int)HttpStatusCode.BadRequest,
        //            Message = "Failed",
        //            Errors = recoveryPasswordResult.ErrorsList.Collection
        //        });
        //    }
        //}
    }
}
