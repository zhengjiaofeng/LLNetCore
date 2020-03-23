using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LLC.Common.DB;
using LLC.IService.IServices.Users;
using LLC.Models.Users;
using LLCoreApi.Common.Tool;
using LLCoreApi.Models.Base;
using LLCoreApi.Models.UserInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LLCoreApi.Controllers
{
    /// <summary>
    ///  Account
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserSevice iUserSevice;
        public AccountController(IUserSevice _iUserSevice)
        {
            iUserSevice = _iUserSevice;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login([FromBody]UserInfoDto model)
        {
            ResponeResult result = new ResponeResult();
            result.state = "501";
            result.msg = "faild";
            Task<U_Users> tUsers = iUserSevice.GetUserByUserAcount(model.UserName);
            U_Users u_user = tUsers.Result;
            if (u_user != null)
            {
                if (u_user.UserPassWord != model.PassWord)
                {
                    result.state = "501";
                    result.msg = "密码错误！";

                }
                else
                {
                    List<Claim> claimList = new List<Claim>()
                    {
                         new Claim(ClaimTypes.Sid, u_user.Id.ToString()),
                         new Claim(ClaimTypes.Name, u_user.UserAcount)
                    };
                    string token = JWTTokenUtil.GetToken(claimList);

                    if (!string.IsNullOrEmpty(token))
                    {
                        result.state = "200";
                        result.msg = "登录成功！";
                        result.data = token;
                    }
                    else
                    {
                        result.state = "501";
                        result.msg = "登录失败，请重新登录！";
                    }
                }

            }

            //返回json
            return new JsonResult(result);

        }


        /// <summary>
        /// 初始化用户
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public JsonResult LLInit()
        {
            ResponeResult result = new ResponeResult();
            result.state = "500";
            result.msg = "faild";
            int add = iUserSevice.AddLLUserInit();

            if (add > 0)
            {
                result.state = "200";
                result.msg = "success";
            }

            //返回json
            return new JsonResult(result);
        }


        /// <summary>
        /// RefreshToken
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RefreshToken(string jwtToken)
        {
            ResponeResult result = new ResponeResult();
            result.state = "500";
            result.msg = "faild";

            if(!string.IsNullOrEmpty(jwtToken))
            {
              JWTTokenUtil.SerializeJwt(jwtToken);
            }
            //返回json
            return new JsonResult(result);
        }
    }
}