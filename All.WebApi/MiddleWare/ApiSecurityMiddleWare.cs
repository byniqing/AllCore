using All.WebApi.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace All.WebApi.MiddleWare
{
    /// <summary>
    /// Api签名验证中间件
    /// </summary>
    public class ApiSecurityMiddleWare
    {
        private readonly RequestDelegate next;
        private static bool isValid = false;

        public static Endpoint GetEndpoint(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Features.Get<IEndpointFeature>()?.Endpoint;
        }


        public ApiSecurityMiddleWare(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = GetEndpoint(context)?.Metadata.GetMetadata<UnAuthApi>();
            if (endpoint != null) //不需要验证权限和签名
            {
                //var msg1 = endpoint.Message;
                //var attruibutes = GetEndpoint(context)?.Metadata.OfType<UnAuthApi>();
                await next(context);
                //await Task.CompletedTask;
                return;
            }

            var headers = context.Request.Headers;
            //var Authorization = headers["Authorization"].ToString();
            var refresh_token = headers["refresh_token"].ToString();

            /*注意：
             当access_token过期，需要刷新token。判断是否携带refresh_token
             如果携带refresh_token说明是刷新token，此时需要验证签名
          
             * 1：已授权，则验证签名
             * 2：如果是刷新token，则验证签名
             */
            //已经授权，验证签名
            bool isAuth = context.Request.HttpContext.User.Identity.IsAuthenticated;
            if (isAuth || !string.IsNullOrWhiteSpace(refresh_token))
            {
                await ValidToken(context);
            }

            //未授权，则不验证签名
            /*
              1：200+未授权，next
              2：200+已授权+签名通过 next
             */
            if (context.Response.StatusCode == StatusCodes.Status200OK && (!isAuth || isValid))
                await next(context);
        }
        private static async Task ValidToken(HttpContext context)
        {

            var headers = context.Request.Headers;

            //当前时间戳
            long currTimeStamp = 0;// Utils.GetTimeStamp();
            //设置过期时间（毫秒）滑动时间
            long expiredSeconds = 60 * 1000; //1分钟内有效

            var appid = headers["appid"].ToString();
            var guid = headers["guid"].ToString();
            var timestamp = headers["timestamp"].ToString();
            var signature = headers["signature"].ToString();

            var app = "";// new AppSecurityBll().GetApp(appid);
            var appkey = app == null ? "" : "";// app.Appkey;
            var array = new[] { appid, guid, appkey, timestamp };
            Array.Sort(array);
            var newSignature = "";// Utils.MD5(string.Join("", array));

            //时间戳相等，并且时间戳在有效范围内，并且appid不能为空
            isValid = (newSignature == signature && long.Parse(timestamp) + expiredSeconds >= currTimeStamp && !string.IsNullOrWhiteSpace(appid));
            var format = $"appid==》{appid}，guid==>{guid},timestamp==>{timestamp},signature==>{signature}";
            //NLogHelp.Info($"签名验证信息是：{format}");

            if (!isValid)
            {
                //var result = new ApiResult<dynamic>
                //{
                //    code = (int)ResultCode.SignatureNotLicit,
                //    msg = ResultCode.SignatureNotLicit.GetDescription()

                //}.ToJson();

                context.Response.ContentType = "application/json";
                //context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync(null); //.ConfigureAwait(false);
                return;
            }
        }
    }
}
