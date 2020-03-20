using LL.IService.IServices.Users;
using LL.Models.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LL.Service.Services.Users
{
    public class UserService : IUserSevice
    {
        private readonly LLDbContext llDbContext;

        /// <summary>
        /// 根据账号获取用户信息
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public async Task<U_Users> GetUserByUserAcount(string userAccount)
        {

            return await  llDbContext.Users.FirstOrDefaultAsync(d => d.UserAcount == userAccount);
        }
    }
}
