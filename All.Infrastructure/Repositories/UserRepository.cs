using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using All.Domain.IRepositories;
using All.Domain.Model;

namespace All.Infrastructure.Repositories
{
    public class UserRepository : IUsersRepository
    {
        public IQueryable<Users> Entities => throw new NotImplementedException();

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public int Delete(object id)
        {
            throw new NotImplementedException();
        }

        public int Delete(Users entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(IEnumerable<Users> entities)
        {
            throw new NotImplementedException();
        }

        public string GetById(int id)
        {
            return "Test";
        }

        public Users GetByKey(object key)
        {
            throw new NotImplementedException();
        }

        public int Insert(Users entity)
        {
            throw new NotImplementedException();
        }

        public int Insert(IEnumerable<Users> entities)
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public int Update(Users entity)
        {
            throw new NotImplementedException();
        }
    }
}
