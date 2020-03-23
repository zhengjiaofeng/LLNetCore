using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLCoreApi.Common.Base.Configurations
{
    public static class CorsInit
    {
        /// <summary>
        /// 跨域配置类 指定域名策略 cors
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddCorsInit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("LLCoreApiSpecificOrigins", build =>
                {
                    /*
                     *1.WithOrigins：允许http://localhost:7777， http://192.168.0.10:7777 访问
                     *2.AllowCredentials /指定处理cookie
                     *                      
                     */
                    build.WithOrigins("http://localhost:7777", "http://192.168.0.10:7777").AllowAnyHeader()
                                .AllowAnyMethod().AllowCredentials();

                });
            });
        }
    }
}
