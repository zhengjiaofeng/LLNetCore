using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLCoreApi.Models.Base
{
    /// <summary>
    /// Setting类
    /// </summary>
    public class AppSetting
    {
    }

    /// <summary>
    /// JWTSetting
    /// </summary>
    public class JWTSetting
    {
        /// <summary>
        /// 颁发机构
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 颁发给谁
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 超时时长
        /// </summary>
        public int ExpiresTime { get; set; }
    }
}
