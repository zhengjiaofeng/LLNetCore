using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LLC.Common.DB;
using LLC.IService.IServices.Users;
using LLC.Models.Users;
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

            if (tUsers.Result != null)
            {
                result.state = "200";
                result.msg = "success";
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
    }
}