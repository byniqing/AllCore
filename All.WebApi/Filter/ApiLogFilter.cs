using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace All.WebApi.Filter
{
    /// <summary>
    /// 全局过滤器记录消息
    /// </summary>
    public class ApiLogFilter : ActionFilterAttribute
    {
        //private readonly Appsettings _appsettings;
        //public ApiLogFilter(IOptionsSnapshot<Appsettings> options)
        //{
        //    _appsettings = options.Value;
        //}
        /// <summary>
        /// 参数
        /// </summary>
        private string ActionArguments { get; set; }
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch Stopwatch { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        [Obsolete("弃用")]
        private string LogFlag { get; set; }

        /// <summary>
        /// 是否开启日志打印
        /// </summary>
        private bool IsSwitch
        {
            get => true;//;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //context.Result = new BadRequestObjectResult(new { data = 0, code = 0, msg = "0" });
            base.OnActionExecuting(context);
            if (IsSwitch)
            {
                ActionArguments = Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments);
                Stopwatch = new Stopwatch();
                Stopwatch.Start();
            }
        }

        /// <summary>
        /// OnActionExecuted 在执行操作方法之后由 core 框架调用
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            if (IsSwitch)
            {
                Stopwatch.Stop();

                //var response = context.HttpContext.Response = context.Response ?? new HttpResponseMessage();

                //https://blog.csdn.net/confused_kitten/article/details/80915104
                //var ab = new { code = 90 }.ToJson();
                //context.HttpContext.Response.StatusCode = 401;
                //context.HttpContext.Response.Headers.Add("My-Header", "WebApiFrame-Header");
                //context.HttpContext.Response.WriteAsync(ab);
                //object responseBody = null;

                #region MyRegion
                var headers = context.HttpContext.Request.Headers;
                //检测是否包含'Authorization'请求头，如果不包含返回context进行下一个中间件，用于访问不需要认证的API
                //if (!headers.ContainsKey("Authorization"))
                //{
                //    // context.HttpContext.Request.Bod
                //    responseBody = new ApiResult { };

                //}
                //var tokenStr = headers["Authorization"].ToString().Replace("Bearer ", "");
                //if (tokenStr.Contains("Bearer"))
                //{
                //    tokenStr = tokenStr.Split("Bearer ")[1];

                //    var tokenModel = JwtHelper.SerializeJwt(tokenStr);
                //    if (tokenModel != null && tokenModel.Uid > 0)
                //    {
                //        var userinfo = new UserBll().GetById(tokenModel.Uid);
                //        if (userinfo != null)
                //        {

                //        }
                //    }
                //} 
                #endregion

                HttpRequest request = context.HttpContext.Request;
                var method = request.Method;
                //var headrs = request.Headers;
                var controllerName = context.RouteData.Values["controller"].ToString();
                var actionName = context.RouteData.Values["action"].ToString();
                var phone = headers["phone"].ToString();
                string url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
                string arg = ActionArguments;
                dynamic result = null;

                string res = "Action发生了异常";

                /*
                 如果Action异常，在异常中间件中捕获异常并返回了
                 那么这里还没到异常中间件，所以context.Result会为空
                 */
                if (context.Result != null)
                {
                    result = context.Result.GetType().Name == "EmptyResult" ? new { Value = "无返回结果" } : context.Result as dynamic;
                    try
                    {
                        if (result != null)
                        {
                            res = Newtonsoft.Json.JsonConvert.SerializeObject(result.Value);
                        }
                    }
                    catch (System.Exception)
                    {
                        res = "数据无法序列化";
                    }
                }
                else if (context.Exception is Exception)
                {
                    res += $"==>异常信息：{context.Exception.GetBaseException().Message}";
                }

                //记录日志
                //NLogHelp.Info(
                //$"手机：{phone} \r\n " +
                //$"控制器：{controllerName} \r\n " +
                //$"方法：{actionName} \r\n " +
                //$"方式：{method} \r\n " +
                //$"地址：{url} \r\n " +
                //$"参数：{arg}\r\n " +
                //$"结果：{res}\r\n " +
                //$"耗时：{Stopwatch.Elapsed.TotalSeconds} 秒（指控制器内对应方法执行完毕的时间）\r\n");

                //  ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),

                #region old
                //string rawRequest = string.Empty;
                //using (var stream = new StreamReader(request.Body))
                //{

                //    //Stream stream = request.Body;
                //    //char[] buffer = new char[request.ContentLength.Value];
                //    //stream.Read(buffer, 0, buffer.Length);
                //    //rawRequest = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(buffer));
                //    //JObject rss = JObject.Parse(rawRequest);
                //    //var appid = (string)rss.GetValue("appid");
                //    //var uuid = (string)rss.GetValue("uuid"); //guid
                //    var method = request.Method;
                //    var path = request.Path.Value;
                //    var host = request.Host;
                //    string url = context.HttpContext.Request.Host + context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;

                //    if (request.HasFormContentType) //是否可以通过Form获取参数
                //    {
                //        var ab = request.ReadFormAsync().Result;
                //        if (request.Form.Keys != null && request.Form.Keys.Count > 0)
                //        {
                //            var formtring = string.Empty;
                //            request.Form.Keys.ToList().ForEach(f =>
                //            {
                //                formtring += $"{f}={ request.Form[f]}&";
                //            });
                //            formtring = formtring.Substring(path.LastIndexOf("&") + 1); //干掉最后一个&
                //        }
                //    }
                //    else if (method.ToLower() == "post") //通过body获取
                //    {
                //        request.EnableRewind();
                //        request.Body.Position = 0;
                //        var content = stream.ReadToEnd();
                //    }
                //} 
                #endregion
            }
        }
    }
}
