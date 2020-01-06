using LLCoreApi.Common.Base;
using LLCoreApi.Models.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace LLCoreApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region 初始化配置
            //初始化配置
            services.Configure<JWTSetting>(Configuration.GetSection("JWTSetting"));
            #endregion

            #region 跨域 指定域名策略 cors

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
            #endregion

            #region 添加Swagger

            //添加Swagger.
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
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });
            #endregion

            #region jwt
            //初始化JWT配置
            services.AddJwtConfiguration(Configuration);
            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //强制https 跳转
            app.UseHttpsRedirection();
            //跨域请求
            app.UseCors("LLCoreApiSpecificOrigins");

            #region 配置Swagger
            //配置Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LLCoreAPI");
                c.RoutePrefix = string.Empty;
            });
            #endregion

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
