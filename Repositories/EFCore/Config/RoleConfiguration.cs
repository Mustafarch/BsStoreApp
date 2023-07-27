using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore.Config
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                  new IdentityRole
                    {
                         Name = "User",               //Normal kullanıcılar.Mesela alışveriş sitesi ise o sitede üye olup alışveriş yapan kullancılara karşılık gelsin.
                         NormalizedName = "USER"
                    },
                    new IdentityRole
                    {
                        Name = "Editor",              //belirli alanları düzenleyen kullanıcılar.
                        NormalizedName = "EDITOR"
                    },
                    new IdentityRole
                    {
                        Name = "Admin",                //Sayfanın yönetiminden sorumlu olan kullanıcı.
                        NormalizedName = "ADMIN"
                    }
                );
        }
    }
}
