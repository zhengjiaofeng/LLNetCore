using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL.Service.Base
{
    public class UnitOfWork : IDisposable
    {
        private readonly LLDbContext dbContext;
        private bool disposed = false;
        public UnitOfWork(LLDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public bool SaveChage()
        {
            return dbContext.SaveChanges() > 0;
        }

        public async Task<bool> SaveChageAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class => dbContext.Set<TEntity>().FromSql(sql, parameters);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // dispose the db context.
                    dbContext.Dispose();
                }
            }
            disposed = true;
        }
    }
}
