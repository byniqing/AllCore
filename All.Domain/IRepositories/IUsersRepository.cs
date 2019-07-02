using All.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace All.Domain.IRepositories
{
    public interface IUsersRepository : IRepository<Users>
    {
        /// <summary>
        /// 根据用户id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetById(int id);
    }
}
