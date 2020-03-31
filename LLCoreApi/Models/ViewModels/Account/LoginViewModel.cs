using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLCoreApi.Models.ViewModels.Account
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 登录token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// token 有效期
        /// </summary>
        public string tokenExpired { get; set; }

        /// <summary>
        /// refreshToken
        /// </summary>
        public string refreshToken { get; set; }

        /// <summary>
        /// refreshToken 有效期
        /// </summary>
        public string refTokenExpired { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string userAccount { get; set; }
    }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public class ReSetTokenDto
    {
        /// <summary>
        ///  用户id
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// token 令牌
        /// </summary>
        public string jwtToken { get; set; }

        /// <summary>
        /// refreshToken
        /// </summary>
        public string refreshToken { get; set; }
    }


    /// <summary>
    /// jwtTokenDto
    /// </summary>
    public class JwtTokenDto
    {
        /// <summary>
        /// 登录token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// token 有效期
        /// </summary>
        public string tokenExpired { get; set; }

    }

}
