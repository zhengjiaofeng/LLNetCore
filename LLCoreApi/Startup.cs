using Autofac;
using LLC.Common.DB;
using LLC.Common.Tool.Configs;
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
            services.AddSingleton(new AppSettingUtil());
            //初始化配置
            services.Configure<JWTSetting>(Configuration.GetSection("JWTSetting"));
            #endregion

            #region 组件初始化配置

            #region 跨域 指定域名策略 cors
            services.AddCorsInit(Configuration);
            #endregion

            #region  依赖注入

            #region MySQL DbContext 注入
            services.AddDbContext<LLDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("LLDbContext")));
            #endregion

            #endregion

            #region 添加Swagger
            //添加Swagger
            services.AddSwaggerInit(Configuration);
            #endregion

            #region 初始化JWT配置 
            //初始化JWT配置
            services.AddJwtConfiguration(Configuration);
            #endregion

            #region MiniProfiler SQL监视工具
            
            services.AddMiniProfilerInit(Configuration);
            #endregion

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

            #region Cors
            //跨域请求
            app.UseCors("LLCoreApiSpecificOrigins");
            #endregion

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
