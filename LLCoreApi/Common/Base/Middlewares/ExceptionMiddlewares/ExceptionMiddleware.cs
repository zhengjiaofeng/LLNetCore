using LLC.Common.LogHeleper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLCoreApi.Common.Base.Middlewares.ExceptionMiddleware
{
    /// <summary>
    /// 异常捕获中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await WirteException(ex);

            }
        }


        /// <summary>
        /// 写入日常信息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private Task WirteException(Exception exception)
        {
            Log4Util.Error(exception.ToString());
            return Task.CompletedTask;
        }


    }
}
