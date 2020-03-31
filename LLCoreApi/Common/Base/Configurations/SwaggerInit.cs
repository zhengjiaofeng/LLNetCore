using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LLCoreApi.Common.Base.Configurations
{
    public static class SwaggerInit
    {
        /// <summary>
        /// Swagger配置类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddSwaggerInit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LLCoreAPI", Version = "v1", Contact = new OpenApiContact { Name = "llcode", Email = "llcode.com" } });

                // add security definitions (Swagger UI 上添加 Authorization 按钮 )
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Bearer 授权 \"Authorization:     Bearer+空格+token\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });

                //Swagger 添加token
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme
                        {
                        Reference=new OpenApiReference(){
                        Id="Bearer",
                        Type=ReferenceType.SecurityScheme
                        }
                        },Array.Empty<string>()
                    }
                });

                //接口描述
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                //LLC.Common.LogHeleper.Log4Util.Debug(xmlFile.ToString());
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
