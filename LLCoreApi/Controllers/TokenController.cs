using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LLCoreApi.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LLCoreApi.Controllers
{
    /// <summary>
    /// Token
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        #region  服务申明
        private readonly IOptions<JWTSetting> jwtsettings;
        #endregion

        /// <summary>
        /// TokenController
        /// </summary>
        /// <param name="_jwtsettings"></param>
        public TokenController(IOptions<JWTSetting> _jwtsettings)
        {
            jwtsettings = _jwtsettings;

        }


        /// <summary>
        ///  获取token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetToken()
        {
            ResponeResult result = new ResponeResult();
            result.data = GetAccessToken();
            return new JsonResult(result);
        }

        /// <summary>
        /// TestToken
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult TestToken()
        {
            ResponeResult result = new ResponeResult();
            result.msg = "123";
            var a = HttpContext.User;
            return new JsonResult(result);
        }

        private string GetAccessToken()
        {
            var claims = new[]
            {
                 new Claim(ClaimTypes.Name,"1"),
                 new Claim(ClaimTypes.Role,"user"),
                 new Claim(ClaimTypes.Email,"123"),
                 new Claim("userrName","123")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtsettings.Value.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwtsettings.Value.Issuer,
                jwtsettings.Value.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// ReadToken
        /// </summary>
        /// <param name="tokenstr"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ReadToken(string tokenstr)
        {
            JwtSecurityToken token = new JwtSecurityToken();
            try
            {
                 token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstr);
                
            }
            catch (Exception ex)
            { 
            
            }
            return new JsonResult(token);
        }
    }
}