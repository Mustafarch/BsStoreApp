using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore.Config
{
    public class CategoryConfig : IEntityTypeConfiguration<Category> //kalıtım aldığımız interface config bir metodun kullanılmasını implemente edilmesini zorunlu kılacak. 
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.CategoryId); //Category Id nin Primery Key olduğunu belirttik.
            builder.Property(c => c.CategoryName).IsRequired(); // Category Name nin boş geçilmeyeceğini belirttik.

            builder.HasData(
                new Category()
                {
                    CategoryId = 1,
                    CategoryName = "Computer"
                },
                new Category()
                {
                    CategoryId = 2,
                    CategoryName = "Network"
                },
                new Category()
                {
                    CategoryId = 3,
                    CategoryName = "Database"
                }
            );
        }
    }
}
