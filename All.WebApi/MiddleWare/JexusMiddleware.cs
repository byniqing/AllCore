using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace All.WebApi.MiddleWare
{
    /// <summary>
    /// Jexus Server Middleware
    /// </summary>
    public class JexusMiddleware
    {
        readonly RequestDelegate _next;
        public JexusMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<IISOptions> options)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var headers = httpContext.Request.Headers;

            try
            {
                if (headers != null && headers.ContainsKey("X-Original-For"))
                {
                    var ipaddAdndPort = headers["X-Original-For"].ToArray()[0];
                    var dot = ipaddAdndPort.IndexOf(":", StringComparison.Ordinal);
                    var ip = ipaddAdndPort;
                    var port = 0;
                    if (dot > 0)
                    {
                        ip = ipaddAdndPort.Substring(0, dot);
                        port = int.Parse(ipaddAdndPort.Substring(dot + 1));
                    }

                    httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip);
                    if (port != 0) httpContext.Connection.RemotePort = port;
                }
            }
            finally
            {
                await _next(httpContext);
            }
        }
    }

    /// <summary>
    /// Extensions
    /// </summary>
    public static class WebHostBuilder
    {
        /// <summary>
        /// 启用JexusIntegration中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseJexusIntegration(this IWebHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // 检查是否已经加载过了
            if (builder.GetSetting(nameof(UseJexusIntegration)) != null)
            {
                return builder;
            }

            // 设置已加载标记，防止重复加载
            builder.UseSetting(nameof(UseJexusIntegration), true.ToString());

            // 添加configure处理
            builder.ConfigureServices(services =>
            {
                /*
                 依赖注入：需要命名空间：using Microsoft.Extensions.DependencyInjection;
                 */
                services.AddSingleton<IStartupFilter>(new JwsSetupFilter());
            });

            return builder;
        }
    }

    class JwsSetupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<JexusMiddleware>();
                next(app);
            };
        }
    }
}
