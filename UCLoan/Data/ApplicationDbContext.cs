using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UCLoan.Models;

namespace UCLoan.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanHistory> LoanHistories { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<EquipmentModel> EquipmentModels { get; set; }
        public DbSet<EquipmentHistory> EquipmentHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(e => e.CPF).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<EquipmentHistory>()
                .HasOne(eh => eh.ChangedBy)
                .WithMany(u => u.ChangedHistories)
                .HasForeignKey(eh => eh.ChangedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EquipmentHistory>()
                .HasOne(eh => eh.LastUser)
                .WithMany(u => u.LastUserHistories)
                .HasForeignKey(eh => eh.LastUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
