using Insurance.Data.Repository.Entities;
using Microsoft.EntityFrameworkCore;
namespace Insurance.Data.Repositories
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SurchargeEntity> Surcharges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity mappings
            modelBuilder.Entity<SurchargeEntity>(entity =>
            {
                entity.ToTable("Surcharge"); // Map entity to correct table name
                entity.HasKey(e => e.ProductTypeId);
                entity.Property(e => e.Surcharge)
                    .IsRequired()
                    .HasColumnType("decimal(18, 2)"); // Specify column type

                // Ensure ProductTypeId is unique
                entity.HasIndex(e => e.ProductTypeId)
                    .IsUnique();
            });
        }

    }

}
