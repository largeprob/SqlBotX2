using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Core.Startups
{
    public class CorsConfig
    {
        /// <summary>
        /// 允许的域集合
        /// </summary>
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
        /// <summary>
        /// 凭证启用
        /// </summary>
        public bool AllowCredentials { get; set; }
    }


    public static class CorsExtensions
    {
        private const string PolicyName = "DefaultCorsPolicy";

        extension(IServiceCollection services)
        {
            public IServiceCollection AddDefaultCors(IConfiguration configuration)
            {
                // 1. 绑定配置对象并支持热更新
                services.Configure<CorsConfig>(configuration.GetSection("CorsConfig"));

                // 2. 注册 CORS 策略
                services.AddCors(options =>
                {
                    // 注意：这里先定义一个空的策略，具体规则由下面的中间件动态读取
                    options.AddPolicy(PolicyName, builder =>
                    {
                        // 初始化时可以设置默认值，或者留空
                    });
                });

                return services;
            }
        }

        extension(IApplicationBuilder app)
        {
            public IApplicationBuilder UseDefaultCors()
            {
                // 使用自定义中间件拦截 CORS，利用 IOptionsMonitor 实现即时感知
                app.UseCors(builder =>
                {
                    // 通过获取 IOptionsMonitor 实例
                    var monitor = app.ApplicationServices.GetRequiredService<IOptionsMonitor<CorsConfig>>();

                    // 获取当前最新的配置值（配置修改后，这里拿到的永远是最新的）
                    var config = monitor.CurrentValue;

                    if (config.AllowedOrigins.Any(x => x == "*"))
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    }
                    else
                    {
                        builder.WithOrigins(config.AllowedOrigins)
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();

                        if (config.AllowCredentials)
                        {
                            builder.AllowCredentials();
                        }
                    }
                });

                return app;
            }
        }
    }
}
