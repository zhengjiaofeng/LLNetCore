using Autofac;
using LLC.Common.DB;
using LLCoreApi.Common.Base;
using LLCoreApi.Common.Base.Configurations;
using LLCoreApi.Common.Base.Middlewares.ExceptionMiddleware;
using LLCoreApi.Models.Base;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
namespace LLCoreApi
{
    public class Startup
    {
        private readonly ILoggerRepository repository;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            #region log����
            repository = LogManager.CreateRepository("CommonLog");

            XmlConfigurator.Configure(repository, new FileInfo("log4.config"));//�����ļ�·�������Զ���
            BasicConfigurator.Configure(repository);
            #endregion
        }

        public IConfiguration Configuration { get; }


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

            #region  ����ע��

            #region MySQL DbContext ע��
            services.AddDbContext<LLDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("LLDbContext")));
            #endregion

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
                LLC.Common.LogHeleper.Log4Util.Debug(xmlFile.ToString());
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });
            #endregion

            #region jwt
            //��ʼ��JWT����
            services.AddJwtConfiguration(Configuration);
            #endregion

            #region MiniProfiler SQL���ӹ���
            services.AddMiniProfilerInit(Configuration);
            #endregion

            //services.AddControllers();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                ////�����쳣�����м��
                app.UseMiddleware<ExceptionMiddleware>();
                //app.UseExceptionHandler("/Error");
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
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("LLCoreApi.index.html");
                c.RoutePrefix = string.Empty;
            });
            #endregion


            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            //MiniProfiler
            app.UseMiniProfiler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }

        /// <summary>
        /// autofac ע��
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;

            var servicesDllFile = Path.Combine(basePath, "LLC.Service.dll");

            if (!(File.Exists(servicesDllFile)))
            {
                throw new Exception("service.dll ��ʧ����Ϊ��Ŀ�����ˣ�������ҪLLC.Service��F6���룬��F5���У����� bin �ļ��У���������");
            }

            // ��ȡ Service.dll ���򼯷��񣬲�ע��

            builder.RegisterAssemblyTypes(Assembly.LoadFile(servicesDllFile))//ֱ�Ӳ��ü����ļ��ķ���
                                                                             //.PropertiesAutowired()//��ʼ����ע��
                                                                             .Where(t => t.Name.EndsWith("Service"))
                       .AsImplementedInterfaces()//��ʾע������ͣ��Խӿڵķ�ʽע�᲻����IDisposable�ӿ�
                                                 // .EnableInterfaceInterceptors()//����Autofac.Extras.DynamicProxy,ʹ�ýӿڵ�����������ʹ������ [Attribute] ע��ʱ��ע����������ע�ᵽ�ӿ�(Interface)�ϻ���ʵ����(Implement)�ϡ�ʹ��ע�ᵽ�ӿ��Ϸ�ʽ�����е�ʵ���඼��Ӧ�õ���������
                       .InstancePerLifetimeScope();//��Ϊÿһ����������ô���һ����һ�Ĺ����ʵ��


        }
    }
}
