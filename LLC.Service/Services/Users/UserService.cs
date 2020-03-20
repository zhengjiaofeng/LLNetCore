using LLC.Common.DB;
using LLC.Common.Tool.MD5;
using LLC.IService.IServices.Users;
using LLC.Models.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLC.Service.Services.Users
{
    public class UserService : IUserSevice
    {
        /// <summary>
        /// db 上下文  须在 Startup中注入LLDbContext
        /// </summary>
        private readonly LLDbContext llDb;


        public UserService(LLDbContext _llDbContext)
        {
            llDb = _llDbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  int AddLLUserInit()
        {
            var result = -1;

            try
            {
                if (llDb.Users.Count()==0)
                {
                    List<U_Users> list = new List<U_Users>() { new U_Users { UserAcount = "zjf", UserPassWord = Md5Encry.Encry("123456"), CreateTime = DateTime.Now }, new U_Users { UserAcount = "ll", UserPassWord = Md5Encry.Encry("123456"), CreateTime = DateTime.Now } };
                    llDb.Users.AddRange(list);
                    result = llDb.SaveChanges();
                }
            }
            catch (Exception ex)
            { 
                
            }
           
            
            return result;
            
        }

        /// <summary>
        /// 根据账号获取用户信息
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public async Task<U_Users> GetUserByUserAcount(string userAccount)
        {
            return await llDb.Users.FirstOrDefaultAsync(d => d.UserAcount == userAccount);
        }


    }
}
