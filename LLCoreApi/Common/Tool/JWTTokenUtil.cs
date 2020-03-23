using LLC.Common.LogHeleper;
using LLC.Common.Tool.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LLCoreApi.Common.Tool
{
    /// <summary>
    /// JWTToken
    /// </summary>
    public class JWTTokenUtil
    {
        private static IConfiguration Configuration { get; set; }


        /// <summary>
        /// 获取token
        /// </summary>
        public static string GetToken(List<Claim> claimList)
        {
            string result = "";

            try
            {
             
                string SecretKey = AppSettingUtil.GetSectionValue("JWTSetting:SecretKey");
                int ExpiresTime = Int32.Parse(AppSettingUtil.GetSectionValue("JWTSetting:ExpiresTime"));
                string Issuer = AppSettingUtil.GetSectionValue("JWTSetting:Issuer");
                string Audience = AppSettingUtil.GetSectionValue("JWTSetting:Audience");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
                //签名证书(秘钥，加密算法)
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                //.NET Core’s JwtSecurityToken class takes on the heavy lifting and actual
                var expiresTime = ExpiresTime;
                var token = new JwtSecurityToken(
                  issuer: Issuer,
                  audience: Audience,
                  //超时时长
                  expires: DateTime.Now.AddMinutes(ExpiresTime),
                  //签名
                  signingCredentials: creds,
                  //用户信息
                  claims: claimList);

                result = new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Log4Util.Error("GetToken：error," + ex.ToString() + ",\n StackTrace：" + ex.StackTrace);
            }

            return result;

        }

        /// <summary>
        /// 解析jwtToken
        /// </summary>
        /// <param name="jwtStr"></param>
        public static void SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

            object sid;
            object name;
            jwtToken.Payload.TryGetValue(ClaimTypes.Sid, out sid);
            jwtToken.Payload.TryGetValue(ClaimTypes.Name, out name);
            
        }
    }
}
