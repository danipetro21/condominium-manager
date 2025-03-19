using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using cem.Models;

namespace cem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Condominium> Condominiums { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserCondominium> UserCondominiums { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserCondominium>()
                .HasKey(uc => new { uc.ManagersId, uc.ManagedCondominiumsId });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ManagedCondominiums)
                .WithMany(c => c.Managers)
                .UsingEntity<UserCondominium>();

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.CreatedBy)
                .WithMany(u => u.CreatedExpenses)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.ApprovedBy)
                .WithMany(u => u.ApprovedExpenses)
                .HasForeignKey(e => e.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Expense)
                .WithMany()
                .HasForeignKey(n => n.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}