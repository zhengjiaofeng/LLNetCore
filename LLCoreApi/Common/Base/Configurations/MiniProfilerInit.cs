using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LLCoreApi.Common.Base.Configurations
{
    public static class MiniProfilerInit
    {
        /// <summary>
        /// MiniProfiler配置类 初始加载MiniProfiler 使用"/profiler/results"来访问分析报告
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddMiniProfilerInit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMiniProfiler(options =>
            {
                //这里是配置了MiniProfiler的路由基础路径，默认的路径是/mini-profiler-resources
                //按照当前配置，你可以使用"/profiler/results"来访问分析报告
                options.RouteBasePath = "/profiler";
            }).AddEntityFramework();
        }
    }
}
