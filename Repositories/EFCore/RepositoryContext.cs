using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories.EFCore.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class RepositoryContext : IdentityDbContext<User>  //DbContext// eski hali
    {
        public RepositoryContext(DbContextOptions options) :
            base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);                           //IdentityDbContext<User>  eklediğimiz için bunu ekledik.
            //modelBuilder.ApplyConfiguration(new BookConfig());
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());   // Repository/EFCore/Config/RoleConfiguration classındaki tanımları burada context e bağladık. Bu olmazsa ordakinin bir anlamı kalmayacak.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); //IEntityTypeConfiguration ifadelerini kullananlar bu satırdaki kod ile artık hiçbir Type ifadesi için yukarıdaki iki satır kodu öyle yazmicaz. bu hepsini tanımlayacak.
        }
    }
}
