using LLC.Common.DB;
using LLC.IService.IServices.Test;
using Microsoft.EntityFrameworkCore;

namespace LLC.Service.Services.Test
{
    public class TestService : ITestService
    {
        /// <summary>
        /// db 上下文  须在 Startup中注入LLDbContext
        /// </summary>
        private readonly LLDbContext llDb;


        public TestService(LLDbContext _llDbContext)
        {
            llDb = _llDbContext;
        }


        public bool EditStock(int id)
        {



            var result = llDb.Database.ExecuteSqlCommand("IF EXIT(select id from good WHERE id=1 AND Quantity>0) BEGIN UPDATE good SET Quantity=Quantity-1 where Id={0} END", id);
            return result > 0;
        }
    }
}
