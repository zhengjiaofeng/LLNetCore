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

    /// <summary>
    /// jtw解析用户信息
    /// </summary>
    public class  UserInfoJwtDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string UserAcount { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }



    }
}
