using System;
using System.Collections.Generic;
using System.Text;

namespace All.Domain
{
    public interface UserInfo
    {
        /// <summary>
        /// 根据用户id获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetById(int id);
    }
}
