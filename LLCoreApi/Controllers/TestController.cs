using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LLC.IService.IServices.Test;
using LLCoreApi.Models.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LLCoreApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestService iTestService;

        public TestController(ITestService _iTestService)
        {
            iTestService = _iTestService;
        }
        [HttpGet]
        public JsonResult Stock()
        {
            ResponeResult result = new ResponeResult();
            bool editResult=  iTestService.EditStock(1);
            result.state = "200";
            result.data = editResult;

            //返回json
            return new JsonResult(result);
        }
    }
}