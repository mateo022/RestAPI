using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestAPIBackendWebService.DataAccess.Models.EntitiesConfiguration;
using RestAPIBackendWebService.Domain.Auth.Entities;

namespace RestAPIBackendWebService.DataAccess
{
    public class RestAPIDbContext: IdentityDbContext<CustomIdentityUser, CustomIdentityRole, string>
    {
        public RestAPIDbContext()
        {
        }

        public RestAPIDbContext(DbContextOptions<RestAPIDbContext> options)
       : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            CustomIdentityEntitiesConfiguration.Configure(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:ApplicationDB");
        }

    }
}
