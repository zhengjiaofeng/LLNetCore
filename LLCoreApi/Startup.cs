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

            #region ��ʼ������
            //��ʼ������
            services.Configure<JWTSetting>(Configuration.GetSection("JWTSetting"));
            #endregion

            #region ���� ָ���������� cors

            services.AddCors(options =>
            {
                options.AddPolicy("LLCoreApiSpecificOrigins", build =>
                {
                    /*
                     *1.WithOrigins������http://localhost:7777�� http://192.168.0.10:7777 ����
                     *2.AllowCredentials /ָ������cookie
                     *                      
                     */
                    build.WithOrigins("http://localhost:7777", "http://192.168.0.10:7777").AllowAnyHeader()
                                .AllowAnyMethod().AllowCredentials();

                });
            });
            #endregion

            #region ���Swagger

            //���Swagger.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LLCoreAPI", Version = "v1", Contact = new OpenApiContact { Name = "llcode", Email = "llcode.com" } });

                // add security definitions (Swagger UI ����� Authorization ��ť )
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Bearer ��Ȩ \"Authorization:     Bearer+�ո�+token\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });

                //Swagger ���token
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




                //�ӿ�����
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });
            #endregion

            #region jwt
            //��ʼ��JWT����
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
            //ǿ��https ��ת
            app.UseHttpsRedirection();
            //��������
            app.UseCors("LLCoreApiSpecificOrigins");

            #region ����Swagger
            //����Swagger
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
