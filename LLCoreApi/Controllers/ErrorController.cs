using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LLCoreApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]//表示 再Swagger文档中排除此类
    public class ErrorController : Controller
    {
        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        [Route("/Error")]
   
        public IActionResult Index()
        {
            return View();
        }
    }
}