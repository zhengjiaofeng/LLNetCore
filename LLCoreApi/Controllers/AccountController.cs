using LLC.Common.Redis;
using LLC.Common.Tool.Operation;
using LLC.Common.Tool.Security;
using LLC.IService.IServices.Users;
using LLC.Models.Users;
using LLCoreApi.Common.Tool;
using LLCoreApi.Models.Base;
using LLCoreApi.Models.UserInfo;
using LLCoreApi.Models.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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

        private readonly IRedisCacheManager iRedisCacheManager;
        public AccountController(IUserSevice _iUserSevice, IRedisCacheManager _iRedisCacheManager)
        {
            iUserSevice = _iUserSevice;
            iRedisCacheManager = _iRedisCacheManager;
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
            result.msg = "登录失败，请重新登录";
            Task<U_Users> tUsers = iUserSevice.GetUserByUserAcount(model.UserName);
            U_Users u_user = tUsers.Result;
            if (u_user != null)
            {
                if (u_user.UserPassWord != model.PassWord)
                {
                    result.msg = "密码错误！";

                }
                else
                {
                    List<Claim> claimList = new List<Claim>()
                    {
                         new Claim(ClaimTypes.Sid, u_user.Id.ToString()),
                         new Claim(ClaimTypes.Name, u_user.UserAcount)
                    };
                    DateTime jwtExpires = DateTime.Now;
                    string token = JWTTokenUtil.GetToken(claimList, ref jwtExpires);

                    if (!string.IsNullOrEmpty(token))
                    {
                        string refreshToken = Md5Encry.Encry(new Random().Next(10,1000).ToString()+u_user.Id);
                        //将refreshToken base64加密
                        string refTokenBase64 = Base64Util.Encode(refreshToken);

                        //设置缓存
                        var isRedSet = iRedisCacheManager.Set("llLoginToken_U" + u_user.Id, refreshToken, TimeSpan.FromMinutes(120));
                        if (isRedSet)
                        {
                            LoginViewModel loginVM = new LoginViewModel();
                            loginVM.token = token;
                            loginVM.tokenExpired = DateExtensions.ToShortDate(jwtExpires);
                            loginVM.refreshToken = refTokenBase64;
                            loginVM.refTokenExpired = DateExtensions.ToShortDate(DateTime.Now.AddMinutes(120));
                            loginVM.userId = u_user.Id.ToString();
                            result.state = "200";
                            result.msg = "登录成功！";
                            result.data = loginVM;
                        }
                        else
                        {
                            result.msg = "登录失败，请重新登录！";
                        }
                    }
                    else
                    {
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
        /// 重置token
        /// </summary>
        /// <param name="tokenDto">参数</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReSetToken([FromBody]ReSetTokenDto tokenDto)
        {
            /* 
             * 1. refreshtoken 存储在redis  因base64加密了，需解密
             * 2. jwtToken  有效
             * 3. 重新生成 jwtToken 
             * 
             */


            ResponeResult result = new ResponeResult();

            try
            {
                result.state = "501";
                result.msg = "失败！";
                if (string.IsNullOrEmpty(tokenDto.jwtToken) || string.IsNullOrEmpty(tokenDto.refreshToken) || string.IsNullOrEmpty(tokenDto.userId))
                {
                    result.msg = "参数异常！";
                    return new JsonResult(result);
                }

                string refToken = Base64Util.Decode(tokenDto.refreshToken);
                string redisRefToken = iRedisCacheManager.Get<string>("llLoginToken_U" + tokenDto.userId);
                if (string.IsNullOrEmpty(redisRefToken))
                {
                    result.msg = "refToken异常！";
                    return new JsonResult(result);
                }

                if (refToken != redisRefToken)
                {
                    result.msg = "refToken伪造！";
                    return new JsonResult(result);
                }

                UserInfoJwtDto jwtUserDto= JWTTokenUtil.SerializeJwt(tokenDto.jwtToken);


                List<Claim> claimList = new List<Claim>()
                    {
                         new Claim(ClaimTypes.Sid, jwtUserDto.UserId),
                         new Claim(ClaimTypes.Name, jwtUserDto.UserAcount)
                    };
                DateTime jwtExpires = DateTime.Now;
                string token = JWTTokenUtil.GetToken(claimList, ref jwtExpires);

                if (!string.IsNullOrEmpty(token))
                {
                    JwtTokenDto jwtDto = new JwtTokenDto();
                    jwtDto.token = token;
                    jwtDto.tokenExpired = DateExtensions.ToShortDate(jwtExpires);
                    result.state = "200";
                    result.data = jwtDto;
                    result.msg = "成功";
                }
                else
                {
                    result.msg = "请求失败，请重新操作！";
                    return new JsonResult(result);
                }
                //返回json
            }

            catch (Exception ex)
            {
                result.msg = "请求失败，请重新操作！";
            }

            return new JsonResult(result);
        }
    }
}