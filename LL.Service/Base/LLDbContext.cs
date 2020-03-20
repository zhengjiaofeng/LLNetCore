using LL.Models.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace LL.Service
{
    public class LLDbContext : DbContext
    {

        public LLDbContext(DbContextOptions<LLDbContext> options)
       : base(options)
        {

        }

        public DbSet<U_Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder OptionsBuilder)
        {
            OptionsBuilder.UseMySQL(ConfigurationManager.ConnectionStrings["LLDbContext"].ConnectionString);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //指定表
            modelBuilder.Entity<U_Users>().ToTable("U_Users");

            // ef core 并发字段 要添加 rowVerSion字段
            //            modelBuilder.Entity<LL_Users>()
            //.Property(p => p.RowVersion).IsConcurrencyToken();
            base.OnModelCreating(modelBuilder);
        }
    }
}
