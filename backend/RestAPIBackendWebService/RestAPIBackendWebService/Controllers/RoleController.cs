using RestAPIBackendWebService.Business.Role.Contracts;
using RestAPIBackendWebService.Domain.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Asp.Versioning;

namespace RestAPIBackendWebService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/roles")]
    [EnableCors]
    public class RoleController : ControllerBase
    {
        private readonly IRoleBusiness _roleBusiness;

        public RoleController(IRoleBusiness roleBusiness)
        {
            _roleBusiness = roleBusiness;
        }

        [HttpGet("roles-for-create")]
        [Authorize(Roles = "Admin")]
        public ActionResult<ResponseDTO> GetRolesForCreate()
        {
            var claimsIdentity = (HttpContext.User.Identity as ClaimsIdentity);
            var rolesForCreation = _roleBusiness.GetValidRolesForUserCreate(claimsIdentity);

            if (rolesForCreation != null && rolesForCreation.Any())
            {
                return Ok(new ResponseDTO
                {
                    Message = "Success",
                    StatusCode = (int)HttpStatusCode.OK,
                    Data = rolesForCreation
                });
            }

            return NotFound();
        }
    }
}

