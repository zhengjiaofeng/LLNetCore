using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login([FromBody]UserInfoDto model )
        {
            var date = DateTime.Parse("2020-02-01");
            var nextDate = date.AddMonths(1);
            string startTime = date.Year + "-" + date.Month + "-" + "01";
            string endTime = nextDate.Year + "-" + nextDate.Month + "-" + "01";
            var req = Request;
            ResponeResult result = new ResponeResult();
            if (model.UserName == "zjf")
            {
                result.state = "200";
                result.msg = "success";
            }
            else
            {
                result.state = "501";
                result.msg = "faild";
            }
            //返回json
            return new JsonResult(result);
            
        }
    }
}