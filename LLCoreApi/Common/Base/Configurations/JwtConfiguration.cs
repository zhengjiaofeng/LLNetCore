using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace LLCoreApi.Common.Base
{

    /// <summary>
    /// JWT配置类
    /// </summary>
    public static class JwtConfiguration
    {
        /// <summary>
        /// 初始化JWT配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            #region jwt
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.Audience = configuration["JWTSetting:Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(configuration["JWTSetting:SecretKey"])),

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    //Token颁发机构
                    ValidIssuer = configuration["JWTSetting:Issuer"],

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    //颁发给谁
                    ValidAudience = configuration["JWTSetting:Audience"],

                    //验证token 有效期
                    ValidateLifetime = true,

                    // If you want to allow a certain amount of clock drift, set that here
                    ClockSkew = TimeSpan.Zero

                };
            });
            #endregion
        }
    }
}
