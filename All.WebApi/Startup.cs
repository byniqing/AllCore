using All.WebApi.Filter;
using All.WebApi.MiddleWare;
using All.WebApi.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace All.WebApi
{
    public class Startup
    {
        /// <summary>
        /// 名称
        /// </summary>
        private const string ApiName = "All.Core";
        private IApiVersionDescriptionProvider provider;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //全局日志记录过滤器
            //services.AddMvc(m => m.Filters.Add<ApiLogFilter>());


            //services.AddMvcCore()
            //.AddApiExplorer();

            #region swagger服务注册
            services.AddSwaggerGen(options =>
            {
                #region 多版本控制
                //遍历出全部的版本，做文档信息展示
                typeof(CustomApiVersion.Version).GetEnumNames().ToList().ForEach(version =>
                {
                    options.SwaggerDoc(version, new Info
                    {
                        // {ApiName} 定义成全局变量，方便修改
                        Version = version,
                        Title = $"{ApiName} 接口文档",
                        Description = $"{ApiName} HTTP API " + version,
                        TermsOfService = "None",
                        Contact = new Contact
                        {
                            Name = "糯米粥",
                            Email = "nsky@cnblogs.com",
                            Url = "http://www.cnblogs.com/nsky"
                        }
                    });
                    // 按相对路径排序，
                    options.OrderActionsBy(o => o.RelativePath);

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath, true);
                });
                #endregion

                #region 如果不用版本控制
                //options.SwaggerDoc("v1", new Info
                //{
                //    Title = "SwaggerApi测试",
                //    Version = "v1",
                //    Description = "接口文档描述",
                //    Contact = new Contact
                //    {
                //        Name = "糯米粥",
                //        Email = "nsky@cnblogs.com",
                //        Url = "http://www.cnblogs.com/nsky"
                //    }
                //});
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //options.IncludeXmlComments(xmlPath, true); 
                #endregion
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region 配置swaggerui信息
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                #region 如果不用版本控制
                //options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //options.RoutePrefix = string.Empty;
                #endregion

                #region 多版本控制
                typeof(CustomApiVersion.Version).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                    options.RoutePrefix = string.Empty;
                });

                //或者
                //foreach (var description in provider.ApiVersionDescriptions)
                //{
                //    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                //}
                #endregion
            });
            #endregion


            /*使用NLog*/
            //loggerFactory.AddNLog();
            //env.ConfigureNLog("NLog.config");


            //app.UseStaticFiles();

            #region Nlog
            loggerFactory.AddNLog();
            // v = Path.Combine(AppContext.BaseDirectory, "nlog.config");
            LogManager.LoadConfiguration("nlog.config");

            //app.AddNLogWeb();
            //loggerFactory.ConfigureNLog("nlog.config"); 

            #endregion

            app.UseEndpointRouting(); //必须放在UseMiddleware 之前

            #region 自定义中间件
            //全局异常处理中间件
            //app.UseMiddleware(typeof(ExceptionHandlerMiddleWare));
            //签名验证中间件
            //app.UseMiddleware<ApiSecurityMiddleWare>();
            //app.Map("/v1/Settings", build =>
            //{
            //   var ack =  build.UseMiddleware<ApiSecurityMiddleWare>();

            //});
            #endregion

            #region 自定义注册服务
            CustomServiceProvider.Instance = app.ApplicationServices;
            #endregion

            app.UseHttpsRedirection();
            app.UseMvc();


        }
    }
}
