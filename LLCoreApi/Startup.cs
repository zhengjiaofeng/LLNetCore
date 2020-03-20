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
            #region log配置
            repository = LogManager.CreateRepository("CommonLog");

            XmlConfigurator.Configure(repository, new FileInfo("log4.config"));//配置文件路径可以自定义
            BasicConfigurator.Configure(repository);
            #endregion
        }

        public IConfiguration Configuration { get; }


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

            #region  依赖注入

            #region MySQL DbContext 注入
            services.AddDbContext<LLDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("LLDbContext")));
            #endregion

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
                LLC.Common.LogHeleper.Log4Util.Debug(xmlFile.ToString());
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });
            #endregion

            #region jwt
            //初始化JWT配置
            services.AddJwtConfiguration(Configuration);
            #endregion

            #region MiniProfiler SQL监视工具
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
                ////生产异常捕获中间件
                app.UseMiddleware<ExceptionMiddleware>();
                //app.UseExceptionHandler("/Error");
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
        /// autofac 注入
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;

            var servicesDllFile = Path.Combine(basePath, "LLC.Service.dll");

            if (!(File.Exists(servicesDllFile)))
            {
                throw new Exception("service.dll 丢失，因为项目解耦了，所以需要LLC.Service先F6编译，再F5运行，请检查 bin 文件夹，并拷贝。");
            }

            // 获取 Service.dll 程序集服务，并注册

            builder.RegisterAssemblyTypes(Assembly.LoadFile(servicesDllFile))//直接采用加载文件的方法
                                                                             //.PropertiesAutowired()//开始属性注入
                                                                             .Where(t => t.Name.EndsWith("Service"))
                       .AsImplementedInterfaces()//表示注册的类型，以接口的方式注册不包括IDisposable接口
                                                 // .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy,使用接口的拦截器，在使用特性 [Attribute] 注册时，注册拦截器可注册到接口(Interface)上或其实现类(Implement)上。使用注册到接口上方式，所有的实现类都能应用到拦截器。
                       .InstancePerLifetimeScope();//即为每一个依赖或调用创建一个单一的共享的实例


        }
    }
}
