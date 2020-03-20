using LL.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LL.IService.IServices.Users
{
    public interface IUserSevice
    {
        /// <summary>
        /// 根据账号获取用户信息
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        Task<U_Users> GetUserByUserAcount(string userAccount);
    }
}
