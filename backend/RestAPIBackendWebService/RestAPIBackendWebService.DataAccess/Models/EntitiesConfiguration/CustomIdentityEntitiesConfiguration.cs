
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestAPIBackendWebService.Domain.Auth.Entities;

namespace RestAPIBackendWebService.DataAccess.Models.EntitiesConfiguration
{
    public static class CustomIdentityEntitiesConfiguration
    {
        public static void Configure(ModelBuilder builder)
        {

            //Remove user name unique index
            builder.Entity<CustomIdentityUser>(b =>
            {
                var index = b.HasIndex(u => new { u.NormalizedUserName }).Metadata;
                b.Metadata.RemoveIndex((index.Properties));
            });

            var userId = Guid.NewGuid().ToString();
            var superAdminRolId = Guid.NewGuid().ToString();

            builder.Entity<CustomIdentityUser>().HasData
                (
                    new CustomIdentityUser
                    {
                        Id = userId,
                        UserName = "Mateo Ceballos",
                        NormalizedUserName = "MATEO CEBALLOS",
                        Email = "mateoceballos022@gmail.com",
                        NormalizedEmail = "MATEOCEBALLOS022@GMAIL.COM",
                        EmailConfirmed = true,
                        PasswordHash = new PasswordHasher<CustomIdentityUser>().HashPassword(null, "u5v4t$kk8JdSYrw7epIN3&&^wRjbWJ)m"),
                        PhoneNumber = "3147894195",
                        PhoneNumberIndicator = "57",
                        TwoFactorCode = "2210"
                    }
                );

            builder.Entity<CustomIdentityRole>().HasData
                (
                    new CustomIdentityRole
                    {
                        Id = superAdminRolId,
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
      
                    new CustomIdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "User",
                        NormalizedName = "USER"
                    }
                );

            builder.Entity<IdentityUserRole<string>>().HasData
                (
                    new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                    {
                        RoleId = superAdminRolId,
                        UserId = userId
                    }
                );
        }
    }
}
