using Microsoft.EntityFrameworkCore;
using cem.Data.Models;

namespace cem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Condominium> Condominiums { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurazioni per le proprietà e le relazioni
            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ManagedCondominiums)
                .WithMany(c => c.Managers);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.ApprovedBy)
                .WithMany()
                .HasForeignKey(e => e.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Expense)
                .WithMany()
                .HasForeignKey(n => n.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Aggiungi i dati di seed per i condomini
            modelBuilder.Entity<Condominium>().HasData(
                new Condominium
                {
                    Id = 1,
                    Name = "Condominio A",
                    Address = "Via Roma 1",
                    City = "Milano",
                    Province = "MI",
                    PostalCode = "20100",
                    CreationDate = new DateTime(2023, 10, 1) // Valore statico
                },
                new Condominium
                {
                    Id = 2,
                    Name = "Condominio B",
                    Address = "Via Dante 2",
                    City = "Torino",
                    Province = "TO",
                    PostalCode = "10100",
                    CreationDate = new DateTime(2023, 10, 1) // Valore statico
                }
            );

            // Aggiungi i dati di seed per gli utenti
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashed_password_here", // Usa un hash reale
                    Role = UserRole.Admin,
                    FirstName = "Admin",
                    LastName = "User",
                    CreatedAt = new DateTime(2023, 10, 1) // Valore statico
                },
                new User
                {
                    Id = 2,
                    Username = "manager1",
                    Email = "manager1@example.com",
                    PasswordHash = "hashed_password_here", // Usa un hash reale
                    Role = UserRole.CondominiumManager,
                    FirstName = "Manager",
                    LastName = "One",
                    CreatedAt = new DateTime(2023, 10, 1) // Valore statico
                }
            );

            // Aggiungi i dati di seed per le spese
            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = 1,
                    Description = "Pulizia scale",
                    Amount = 100.50m,
                    Date = new DateTime(2023, 10, 1), // Valore statico
                    Type = "Manutenzione",
                    Status = ExpenseStatus.Pending,
                    CondominiumId = 1,
                    CreatedById = 2,
                    CreatedAt = new DateTime(2023, 10, 1), // Valore statico
                    Condominium = null, // Inizializza con null (o con un oggetto valido se necessario)
                    CreatedBy = null    // Inizializza con null (o con un oggetto valido se necessario)
                }
            );

            // Aggiungi i dati di seed per le notifiche
            modelBuilder.Entity<Notification>().HasData(
                new Notification
                {
                    Id = 1,
                    Title = "Nuova spesa",
                    Message = "Una nuova spesa è stata creata.",
                    Type = NotificationType.ExpenseApproved,
                    UserId = 2,
                    ExpenseId = 1,
                    CreatedAt = new DateTime(2023, 10, 1), // Valore statico
                    User = null,    // Inizializza con null (o con un oggetto valido se necessario)
                    Expense = null  // Inizializza con null (o con un oggetto valido se necessario)
                }
            );
        }
    }
}