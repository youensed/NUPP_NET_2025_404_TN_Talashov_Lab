using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetStore.Infrastructure.Models;

namespace PetStore.Infrastructure
{
    public class PetStoreContext : IdentityDbContext<User>
    {
        public PetStoreContext(DbContextOptions<PetStoreContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<DogModel> Dogs { get; set; } = null!;
        public DbSet<CatModel> Cats { get; set; } = null!;
        public DbSet<CustomerModel> Customers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure table names for Identity tables (optional, cleaner names)
            builder.Entity<User>().ToTable("Users");

            // Configure Pet inheritance - Table Per Hierarchy (TPH) strategy
            builder.Entity<PetModel>()
                .HasDiscriminator<string>("PetType")
                .HasValue<DogModel>("Dog")
                .HasValue<CatModel>("Cat");

            // Configure CustomerModel - store PetIds as JSON
            builder.Entity<CustomerModel>()
                .Property(c => c.PetIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Guid.Parse).ToList()
                );
        }
    }
}

