using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace All.WebApi
{
    /// <summary>
    /// 自定义服务容器实例
    /// </summary>
    public class CustomServiceProvider
    {
        /*
         https://blog.csdn.net/shiershilian/article/details/80876054
         https://www.bbsmax.com/A/rV57bj09JP/
         https://www.cnblogs.com/xishuai/p/asp-net-core-ioc-di-get-service.html
             */
        public static IServiceProvider Instance { get; set; }
    }
}
