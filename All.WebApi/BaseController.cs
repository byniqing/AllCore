using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace All.WebApi
{
    public class BaseController : Controller
    {
        //protected IMyService _myService;
        public BaseController()
        {
            /*
             在BaseController的构造方法里通过ServiceLocator.Instance获取IHttpContextAccessor，再使用IHttpContextAccessor获取注册的自定义服务。这里不直接使用CustomServiceProvider.Instance.GetService方法获取自定义服务是，因为如果使用Scoped方式注册服务的话这里获取服务会报错。
*/
            var cus = CustomServiceProvider.Instance.GetService<IHttpContextAccessor>();
            //_myService = hca.HttpContext.RequestServices.GetService<IMyService>(); 
            var context = cus.HttpContext;
            //获取session
            var s = context.Session;
        }
    }
}
