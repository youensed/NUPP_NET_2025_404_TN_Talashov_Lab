using Microsoft.EntityFrameworkCore;
using PetStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Internal;

namespace PetStore.Infrastructure
{
    public class PetStoreContext : DbContext
    {
        private readonly string _connectionString;

       public PetStoreContext(string connectionString)
    {
        _connectionString = connectionString;
    }

   
    public PetStoreContext()
    {
    }
    

        // DbSet properties for each entity
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<PetModel> Pets { get; set; }
        public DbSet<DogModel> Dogs { get; set; }
        public DbSet<CatModel> Cats { get; set; }
        public DbSet<VaccineModel> Vaccines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use connection string if provided via constructor
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    optionsBuilder.UseNpgsql(_connectionString);
                }
                else
                {
                    // Default connection string for PostgreSQL
                    optionsBuilder.UseNpgsql("Host=localhost;Database=PetStoreDB;Username=postgres;Password=b27g12dan");
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Table-per-Type (TPT) inheritance for Pet hierarchy
            modelBuilder.Entity<PetModel>()
                .ToTable("Pets")
                .HasKey(p => p.Id);

            modelBuilder.Entity<DogModel>()
                .ToTable("Dogs");

            modelBuilder.Entity<CatModel>()
                .ToTable("Cats");

            // Configure CustomerModel
            modelBuilder.Entity<CustomerModel>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(c => c.Age)
                    .IsRequired();
            });

            // Configure PetModel
            modelBuilder.Entity<PetModel>(entity =>
            {
                entity.Property(p => p.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(p => p.Age)
                    .IsRequired();
                entity.Property(p => p.PetType)
                    .HasMaxLength(50);

                // One-to-Many: Customer -> Pets (One Customer can have many Pets)
                entity.HasOne(p => p.Owner)
                    .WithMany(c => c.Pets)
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure DogModel
            modelBuilder.Entity<DogModel>(entity =>
            {
                entity.Property(d => d.Breed)
                    .HasMaxLength(100);
            });

            // Configure CatModel
            modelBuilder.Entity<CatModel>(entity =>
            {
                entity.Property(c => c.Color)
                    .HasMaxLength(50);
            });

            // Configure VaccineModel
            modelBuilder.Entity<VaccineModel>(entity =>
            {
                entity.ToTable("Vaccines");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(v => v.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(v => v.Description)
                    .HasMaxLength(500);
            });

            // Configure Many-to-Many relationship: Pet <-> Vaccine
            modelBuilder.Entity<PetModel>()
                .HasMany(p => p.Vaccines)
                .WithMany(v => v.Pets)
                .UsingEntity<Dictionary<string, object>>(
                    "PetVaccine",
                    j => j
                        .HasOne<VaccineModel>()
                        .WithMany()
                        .HasForeignKey("VaccineId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<PetModel>()
                        .WithMany()
                        .HasForeignKey("PetId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.ToTable("PetVaccines");
                        j.HasKey("PetId", "VaccineId");
                    });

            // Add indexes for better query performance
            modelBuilder.Entity<PetModel>()
                .HasIndex(p => p.Name);

            modelBuilder.Entity<CustomerModel>()
                .HasIndex(c => c.Name);

            modelBuilder.Entity<VaccineModel>()
                .HasIndex(v => v.Name);
        }
    }
}

