using Microsoft.EntityFrameworkCore;
using cem.Models;

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
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ManagedCondominiums)
                .WithMany(c => c.Managers)
                .UsingEntity(j => j.ToTable("UserCondominiums"));

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

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

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Master" },
                new Role { Id = 2, Name = "Manager" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "master",
                    Email = "master@cem.com",
                    PasswordHash = "hashed_password",
                    FirstName = "Master",
                    LastName = "Supremo",
                    RoleId = 1,
                    CreatedAt = new DateTime(2025, 10, 1)
                },
                new User
                {
                    Id = 2,
                    Username = "manager",
                    Email = "manager@cem.com",
                    PasswordHash = "hashed_password",
                    FirstName = "Manager",
                    LastName = "Basic",
                    RoleId = 2,
                    CreatedAt = new DateTime(2025, 10, 1)
                }
            );

            modelBuilder.Entity<Condominium>().HasData(
                new Condominium
                {
                    Id = 1,
                    Name = "Condominio A",
                    Address = "Via Roma 1",
                    City = "Milano",
                    Province = "MI",
                    PostalCode = "20100",
                    CreationDate = new DateTime(2025, 10, 1)
                },
                new Condominium
                {
                    Id = 2,
                    Name = "Condominio B",
                    Address = "Via Dante 2",
                    City = "Torino",
                    Province = "TO",
                    PostalCode = "10100",
                    CreationDate = new DateTime(2025, 10, 1)
                }
            );

            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = 1,
                    Description = "Pulizia scale",
                    Amount = 100.50m,
                    Date = new DateTime(2025, 10, 1),
                    Type = "Manutenzione",
                    Status = ExpenseStatus.Pending,
                    CondominiumId = 1,
                    CreatedById = 2,
                    CreatedAt = new DateTime(2025, 10, 1),
                    Condominium = null!,
                    CreatedBy = null!
                }
            );

            modelBuilder.Entity<Notification>().HasData(
                new Notification
                {
                    Id = 1,
                    Title = "Nuova spesa",
                    Message = "Una nuova spesa è stata creata.",
                    Type = NotificationType.ExpenseApproved,
                    UserId = 2,
                    ExpenseId = 1,
                    CreatedAt = new DateTime(2025, 10, 1),
                    User = null!,
                    Expense = null!
                }
            );
        }
    }
}