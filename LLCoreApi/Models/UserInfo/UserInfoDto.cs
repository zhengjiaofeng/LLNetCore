using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLCoreApi.Models.UserInfo
{

    /// <summary>
    /// 用户请求dto
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set;}


        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
    }
}
