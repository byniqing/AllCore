using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace All.WebApi.Filter
{
    /// <summary>
    /// 全局过滤器或者中间件过滤某个Action
    /// 目的：如果是授权后不需要验证签名，才加这个
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class UnAuthApi : Attribute
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 是否授权
        /// </summary>
        public bool IsAuth { get; set; }
        public UnAuthApi(bool isAuth, string message = null)
        {
            Message = message;
            IsAuth = isAuth;
        }
    }
}
