using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace All.WebApi.MiddleWare
{
    /// <summary>
    /// 全局异常处理日志中间件
    /// </summary>
    public class ExceptionHandlerMiddleWare
    {
        //https://www.cnblogs.com/zhangxiaoyong/p/9463791.html
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlerMiddleWare> _logger;
        public ExceptionHandlerMiddleWare(RequestDelegate next, ILogger<ExceptionHandlerMiddleWare> logger)
        {
            this.next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception == null) return;
            await WriteExceptionAsync(context, exception).ConfigureAwait(false);
        }

        private async Task WriteExceptionAsync(HttpContext context, Exception exception)
        {
            //记录日志
            //NLogHelp.Error(exception.GetBaseException().ToString());
            //_logger.LogError(context.Request.GetAbsoluteUri() + "\r\n" + statusCode.ToString());
            //_logger.LogError("");
            //返回友好的提示
            var response = context.Response;

            //状态码
            if (exception is UnauthorizedAccessException) //未授权异常
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
            else if (exception is Exception)
                response.StatusCode = (int)HttpStatusCode.BadRequest;

            response.ContentType = context.Request.Headers["Accept"];

            if (response.ContentType.ToLower() == "application/xml")
            {
                await response.WriteAsync(Object2XmlString(new { msg = exception.GetBaseException().Message })).ConfigureAwait(false);
            }
            else
            {
                response.ContentType = "application/json";

                await response.WriteAsync(JsonConvert.SerializeObject(new object { }
                    //new ApiResult<dynamic>
                    //{
                    //    code = response.StatusCode,
                    //    msg = exception.GetBaseException().Message,
                    //    body = null,
                    //}
                    )
                    ).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 对象转为Xml
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static string Object2XmlString(object o)
        {
            StringWriter sw = new StringWriter();
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                serializer.Serialize(sw, o);
            }
            catch
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Dispose();
            }
            return sw.ToString();
        }
    }
}
