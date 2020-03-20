using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LLC.Common.LogHeleper;
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
        private static int index = 0;
        private static object obj = new object();
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


        #region test
        /// <summary>
        /// AsyncTestWrite
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task AsyncTestWrite()
        {
            index++;
            int index_i = index;
            Log4Util.Debug("AsyncTestWriteStar" + index_i);
            await AsyncWrite1(index_i);
            await AsyncWrite2(index_i);
            Log4Util.Debug("AsyncTestWriteEnd" + index_i);
        }

        [HttpGet]
        public void AsyncTestWrite1()
        {
            Log4Util.Debug("AsyncTestWrite1Star");
            Task.Run(() =>
            {
                Log4Util.Debug("AsyncTestWrite1");
                Thread.Sleep(3000);
            });

            Task.Run(() =>
            {
                Log4Util.Debug("AsyncTestWrite2");
                Thread.Sleep(3000);
            });

            Log4Util.Debug("AsyncTestWrite1End");
        }

        private async Task AsyncWrite1(int index_i)
        {
            await Task.Run(() =>
            {
                Log4Util.Debug("AsyncWrite1" + index_i);
                Thread.Sleep(3000);
            });
            Log4Util.Debug("AsyncWrite1End" + index_i);

        }

        private async Task AsyncWrite2(int index_i)
        {
            await Task.Run(() =>
            {
                Log4Util.Debug("AsyncWrite2" + index_i);
                Thread.Sleep(3000);
            });
            Log4Util.Debug("AsyncWrite2End" + index_i);

        }

        /// <summary>
        /// Test
        /// </summary>
        [HttpGet]
        public void Test()
        {
            Log4Util.Debug("主线程启动，当前线程为：" + Thread.CurrentThread.ManagedThreadId);
            var task = GetLengthAsync();

            Log4Util.Debug("回到主线程，当前线程为：" + Thread.CurrentThread.ManagedThreadId);

            Log4Util.Debug("线程[" + Thread.CurrentThread.ManagedThreadId + "]睡眠5s:");
            Thread.Sleep(5000); //将主线程睡眠5s

            var timer = new System.Diagnostics.Stopwatch();
            timer.Start(); //开始计算时间

            Log4Util.Debug("task的返回值是" + task.Result);

            timer.Stop(); //结束点，另外stopwatch还有Reset方法，可以重置。
            Log4Util.Debug("等待了：" + timer.Elapsed.TotalSeconds + "秒"); //显示时间

            Log4Util.Debug("主线程结束，当前线程为：" + Thread.CurrentThread.ManagedThreadId);
        }

        private static async Task<string> GetLengthAsync()
        {
            Log4Util.Debug("GetLengthAsync()开始执行，当前线程为：" + Thread.CurrentThread.ManagedThreadId);

            var str = await GetStringAsync();

            Log4Util.Debug("GetLengthAsync()执行完毕，当前线程为：" + Thread.CurrentThread.ManagedThreadId);

            return str;
        }

        private static Task<string> GetStringAsync()
        {
            Log4Util.Debug("GetStringAsync()开始执行，当前线程为：" + Thread.CurrentThread.ManagedThreadId);
            return Task.Run(() =>
            {
                Log4Util.Debug("异步任务开始执行，当前线程为：" + Thread.CurrentThread.ManagedThreadId);

                Log4Util.Debug("线程[" + Thread.CurrentThread.ManagedThreadId + "]睡眠10s:");
                Thread.Sleep(10000); //将异步任务线程睡眠10s

                Log4Util.Debug("GetStringAsync()执行完毕，当前线程为：" + Thread.CurrentThread.ManagedThreadId);
                return "GetStringAsync()执行完毕";
            });
        }


        #endregion
    }
}