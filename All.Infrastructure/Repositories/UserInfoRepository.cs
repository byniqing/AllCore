using All.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace All.Infrastructure.Repositories
{
    public class UserInfoRepository : UserInfo
    {
        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetById(int id)
        {
            return "测试";
        }
    }
}
