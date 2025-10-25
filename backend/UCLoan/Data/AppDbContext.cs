using Microsoft.EntityFrameworkCore;
using UCLoan.Entities;

namespace UCLoan.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) 
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Queue> Queues { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        // Configurações específicas do modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fila
            modelBuilder.Entity<Queue>(entity =>
            {
                entity.HasKey(q => q.Id);

                entity.Property(q => q.UserId)
                     .IsRequired();

                entity.Property(q => q.Priority)
                     .IsRequired();
                entity.Property(q => q.Position)
                     .IsRequired();
                entity.Property(q => q.EnqueuedAt)
                     .IsRequired();

                entity.Property(q => q.Status)
                    .HasConversion<string>()
                    .IsRequired();

                entity.HasIndex(q => new { q.Priority, q.Position });
            });

            // Registro de Atividades
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.Property(e => e.Action)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.OccurredAt)
                      .IsRequired();

                entity.Property(e => e.UserId)
                      .IsRequired();

                entity.Property(e => e.DetailsJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

                entity.HasIndex(e => new { e.UserId, e.OccurredAt }).IsUnique();
            });

            // Usuário
            modelBuilder.Entity<User>(entity =>
            {
               entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(u => u.CPF)
                      .IsRequired()
                      .HasMaxLength(14);

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.CreatedAt)
                      .IsRequired();

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.CPF).IsUnique();
            });

            // Equipamento
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PhysicalStatus)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(e => e.LoanStatus)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(e => e.Description)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.HasOne(e => e.Model)
                      .WithMany(m => m.Equipments)
                      .HasForeignKey(e => e.ModelId)
                      .IsRequired();
            });

            // Modelo
            modelBuilder.Entity<Model>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Name)
                        .IsRequired()
                        .HasMaxLength(100);

                entity.Property(m => m.Manufacturer)
                        .IsRequired()
                        .HasMaxLength(100);

                entity.HasMany(m => m.Equipments);

            });

            // Empréstimo
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.UserId)
                      .IsRequired();

                entity.Property(l => l.EquipmentId)
                      .IsRequired();

                entity.Property(l => l.LoanDate)
                      .IsRequired();

                entity.Property(l => l.ReturnDate)
                      .IsRequired(false);

                entity.Property(l => l.Status)
                      .HasConversion<string>()
                      .IsRequired();

                entity.HasIndex(l => new { l.UserId, l.EquipmentId });
            });
        }
    }
}
