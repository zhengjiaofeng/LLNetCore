using System;
using System.Collections.Generic;
using System.Text;

namespace LL.Models.Users
{
   public class U_Users
    {
        /// <summary>
        /// KEY
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserAcount { get; set; }

        /// <summary>
        /// 密码(MD5加密)
        /// </summary>
        public string UserPassWord { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
