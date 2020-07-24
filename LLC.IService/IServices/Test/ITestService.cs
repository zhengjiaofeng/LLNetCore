using System;
using System.Collections.Generic;
using System.Text;

namespace LLC.IService.IServices.Test
{
  public  interface ITestService
    {
        /// <summary>
        /// 减库存
        /// </summary>
        /// <param name="id">商品id</param>
        /// <returns></returns>
        public bool EditStock(int id);
    }
}
