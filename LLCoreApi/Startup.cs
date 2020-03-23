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
            services.AddSingleton(new AppSettingUtil());
            //��ʼ������
            services.Configure<JWTSetting>(Configuration.GetSection("JWTSetting"));
            #endregion

            #region �����ʼ������

            #region ���� ָ���������� cors
            services.AddCorsInit(Configuration);
            #endregion

            #region  ����ע��

            #region MySQL DbContext ע��
            services.AddDbContext<LLDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("LLDbContext")));
            #endregion

            #endregion

            #region ���Swagger
            //���Swagger
            services.AddSwaggerInit(Configuration);
            #endregion

            #region ��ʼ��JWT���� 
            //��ʼ��JWT����
            services.AddJwtConfiguration(Configuration);
            #endregion

            #region MiniProfiler SQL���ӹ���
            
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
                ////�����쳣�����м��
                app.UseMiddleware<ExceptionMiddleware>();
                //app.UseExceptionHandler("/Error");
            }

            //ǿ��https ��ת
            app.UseHttpsRedirection();

            #region Cors
            //��������
            app.UseCors("LLCoreApiSpecificOrigins");
            #endregion

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
