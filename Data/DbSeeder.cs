using Microsoft.AspNetCore.Identity;
using cem.Models;
using Microsoft.EntityFrameworkCore;

namespace cem.Data;

public static class DbSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Creazione dei ruoli se non esistono
        var roles = new[] { "Admin", "CondominiumManager" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Creazione dell'utente admin se non esiste
        var adminEmail = "admin@cem.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "System",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Creazione di gestori di condominio di esempio
        var managers = new[]
        {
            new { Email = "manager1@cem.com", FirstName = "John", LastName = "Doe", Password = "Manager123!" },
            new { Email = "manager2@cem.com", FirstName = "Jane", LastName = "Smith", Password = "Manager123!" },
            new { Email = "manager3@cem.com", FirstName = "Mike", LastName = "Johnson", Password = "Manager123!" }
        };

        foreach (var manager in managers)
        {
            var managerUser = await userManager.FindByEmailAsync(manager.Email);
            if (managerUser == null)
            {
                managerUser = new ApplicationUser
                {
                    UserName = manager.Email,
                    Email = manager.Email,
                    FirstName = manager.FirstName,
                    LastName = manager.LastName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(managerUser, manager.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "CondominiumManager");
                }
            }
        }

        // Creazione di condomini di esempio
        var condominiums = new[]
        {
            new Condominium
            {
                Name = "Residenza del Sole",
                Address = "Via Roma 123",
                City = "Milano",
                Province = "MI",
                PostalCode = "20100",
                CreationDate = DateTime.UtcNow.AddYears(-2)
            },
            new Condominium
            {
                Name = "Villa Verde",
                Address = "Via Garibaldi 456",
                City = "Roma",
                Province = "RM",
                PostalCode = "00100",
                CreationDate = DateTime.UtcNow.AddYears(-1)
            },
            new Condominium
            {
                Name = "Palazzo Moderno",
                Address = "Via Dante 789",
                City = "Torino",
                Province = "TO",
                PostalCode = "10100",
                CreationDate = DateTime.UtcNow.AddMonths(-6)
            }
        };

        foreach (var condominium in condominiums)
        {
            if (!context.Condominiums.Any(c => c.Name == condominium.Name))
            {
                context.Condominiums.Add(condominium);
            }
        }

        await context.SaveChangesAsync();

        // Associazione dei condomini ai gestori
        var manager1 = await userManager.FindByEmailAsync("manager1@cem.com");
        var manager2 = await userManager.FindByEmailAsync("manager2@cem.com");
        var manager3 = await userManager.FindByEmailAsync("manager3@cem.com");

        var residenzaSole = await context.Condominiums.FirstOrDefaultAsync(c => c.Name == "Residenza del Sole");
        var villaVerde = await context.Condominiums.FirstOrDefaultAsync(c => c.Name == "Villa Verde");
        var palazzoModerno = await context.Condominiums.FirstOrDefaultAsync(c => c.Name == "Palazzo Moderno");

        if (manager1 != null && residenzaSole != null)
        {
            var existingAssociation = await context.UserCondominiums
                .FirstOrDefaultAsync(uc => uc.ManagersId == manager1.Id && uc.ManagedCondominiumsId == residenzaSole.Id);
            
            if (existingAssociation == null)
            {
                context.UserCondominiums.Add(new UserCondominium
                {
                    ManagersId = manager1.Id,
                    ManagedCondominiumsId = residenzaSole.Id
                });
            }
        }

        if (manager2 != null && villaVerde != null)
        {
            var existingAssociation = await context.UserCondominiums
                .FirstOrDefaultAsync(uc => uc.ManagersId == manager2.Id && uc.ManagedCondominiumsId == villaVerde.Id);
            
            if (existingAssociation == null)
            {
                context.UserCondominiums.Add(new UserCondominium
                {
                    ManagersId = manager2.Id,
                    ManagedCondominiumsId = villaVerde.Id
                });
            }
        }

        if (manager3 != null && palazzoModerno != null)
        {
            var existingAssociation = await context.UserCondominiums
                .FirstOrDefaultAsync(uc => uc.ManagersId == manager3.Id && uc.ManagedCondominiumsId == palazzoModerno.Id);
            
            if (existingAssociation == null)
            {
                context.UserCondominiums.Add(new UserCondominium
                {
                    ManagersId = manager3.Id,
                    ManagedCondominiumsId = palazzoModerno.Id
                });
            }
        }

        await context.SaveChangesAsync();

        // Creazione di spese di esempio
        if (manager1 != null && residenzaSole != null && adminUser != null)
        {
            var expenses = new[]
            {
                new Expense
                {
                    Description = "Pulizia scale",
                    Amount = 150.00m,
                    Date = DateTime.Now.AddDays(-5),
                    Category = ExpenseCategory.Pulizia,
                    Status = ExpenseStatus.Pending,
                    CreatedById = manager1.Id,
                    CondominiumId = residenzaSole.Id
                },
                new Expense
                {
                    Description = "Manutenzione ascensore",
                    Amount = 500.00m,
                    Date = DateTime.Now.AddDays(-3),
                    Category = ExpenseCategory.Manutenzione,
                    Status = ExpenseStatus.Approved,
                    CreatedById = manager1.Id,
                    ApprovedById = adminUser.Id,
                    ApprovedAt = DateTime.Now.AddDays(-2),
                    CondominiumId = residenzaSole.Id
                },
                new Expense
                {
                    Description = "Bolletta luce",
                    Amount = 200.00m,
                    Date = DateTime.Now.AddDays(-1),
                    Category = ExpenseCategory.Energia,
                    Status = ExpenseStatus.Rejected,
                    CreatedById = manager1.Id,
                    ApprovedById = adminUser.Id,
                    ApprovedAt = DateTime.Now,
                    CondominiumId = residenzaSole.Id
                }
            };

            foreach (var expense in expenses)
            {
                if (!context.Expenses.Any(e => e.Description == expense.Description && e.CondominiumId == expense.CondominiumId))
                {
                    context.Expenses.Add(expense);
                }
            }

            await context.SaveChangesAsync();

            // Creazione di notifiche di esempio
            if (manager1 != null && manager2 != null && manager3 != null)
            {
                var notifications = new[]
                {
                    new Notification
                    {
                        Title = "Spesa approvata",
                        Message = "La spesa per la manutenzione dell'ascensore è stata approvata",
                        Type = NotificationType.ExpenseApproved,
                        CreatedAt = DateTime.UtcNow.AddDays(-30),
                        UserId = manager1.Id,
                        User = manager1,
                        ExpenseId = expenses[1].Id
                    },
                    new Notification
                    {
                        Title = "Spesa rifiutata",
                        Message = "La spesa per la riparazione della caldaia è stata rifiutata: Preventivo troppo alto",
                        Type = NotificationType.ExpenseRejected,
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UserId = manager3.Id,
                        User = manager3,
                        ExpenseId = expenses[2].Id
                    },
                    new Notification
                    {
                        Title = "Rate in scadenza",
                        Message = "Ricorda che hai delle rate in scadenza per il mese corrente",
                        Type = NotificationType.PaymentDue,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UserId = manager2.Id,
                        User = manager2
                    }
                };

                foreach (var notification in notifications)
                {
                    if (!context.Notifications.Any(n => n.Title == notification.Title && n.UserId == notification.UserId))
                    {
                        context.Notifications.Add(notification);
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
} 