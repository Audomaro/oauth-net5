using BO;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVC;
using System;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [AuthorizeCustom]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IUserService _userService;

        public AuthenticationController(IAuthenticateService authenticateService, IUserService userService)
        {
            _authenticateService = authenticateService;
            _userService = userService;
        }

        [AllowAnonymousCustom]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            AuthenticateResponse response = _authenticateService.Authenticate(model, ApplicationName(), IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymousCustom]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            string refreshToken = Request.Cookies["refreshToken"];
            AuthenticateResponse response = _authenticateService.RefreshToken(refreshToken, ApplicationName(), IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymousCustom]
        [HttpPost("revoke-token")]
        public IActionResult RevokeToken(RevokeTokenRequest model)
        {
            string token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            _authenticateService.RevokeToken(token, IpAddress());
            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            UserEntity user = _userService.GetById(id);
            return Ok(user.RefreshTokens);
        }

        private void SetTokenCookie(string token)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }

        private string ApplicationName()
        {
            if (Request.Headers.ContainsKey("X-Application-Name"))
            {
                return $"{Request.Headers["X-Application-Name"]}".Trim();
            }
            else
            {
                return "Unknown";
            }
        }
    }
}
