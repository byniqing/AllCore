using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using All.WebApi.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace All.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        [HttpGet]
        [UnAuthApi(true, Message = null)]
        [AllowAnonymous]
        public void show()
        {

        }
    }
}