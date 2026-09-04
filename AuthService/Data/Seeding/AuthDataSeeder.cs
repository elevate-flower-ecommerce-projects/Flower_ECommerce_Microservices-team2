using AuthService.Common.Enums;
using AuthService.Common.Helpers;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Seeding;

public static class AuthDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AuthDbContext>>();

        var maxRetries = 10;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                logger.LogInformation("Attempting to apply AuthDb database migrations (Attempt {Attempt}/{MaxRetries})...", i + 1, maxRetries);
                
                // If tables like 'Roles' already exist in the database from prior setup, ensure EF Migrations History is synchronized so MigrateAsync doesn't try to recreate existing tables.
                try
                {
                    await context.Database.ExecuteSqlRawAsync(@"
                        IF OBJECT_ID(N'[Roles]', N'U') IS NOT NULL
                        BEGIN
                            IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
                            BEGIN
                                CREATE TABLE [__EFMigrationsHistory] (
                                    [MigrationId] nvarchar(150) NOT NULL,
                                    [ProductVersion] nvarchar(32) NOT NULL,
                                    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                                );
                            END;
                            IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260828154048_Create AuthDatabase')
                            BEGIN
                                INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                                VALUES ('20260828154048_Create AuthDatabase', '10.0.11');
                            END;
                        END;
                    ");
                }
                catch
                {
                    // Ignore if database does not exist yet or connection is still warming up
                }

                await context.Database.MigrateAsync();

                // Ensure GeneratedCode column is long enough to store 64-char reset tokens
                try
                {
                    await context.Database.ExecuteSqlRawAsync(@"
                        IF OBJECT_ID(N'[OtpVerificationCodes]', N'U') IS NOT NULL
                        BEGIN
                            ALTER TABLE [OtpVerificationCodes] ALTER COLUMN [GeneratedCode] nvarchar(128) NOT NULL;
                        END;
                    ");
                }
                catch
                {
                    // Ignore if already adjusted
                }

                logger.LogInformation("AuthDb database migration completed successfully.");
                break;
            }
            catch (Exception ex)
            {
                if (i == maxRetries - 1)
                {
                    logger.LogError(ex, "Failed to migrate AuthDb database after {MaxRetries} attempts.", maxRetries);
                    throw;
                }
                logger.LogWarning("AuthDb database migration failed: {Message}. Retrying in 3 seconds...", ex.Message);
                await Task.Delay(3000);
            }
        }

        try
        {

            // Ensure Roles exist
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { Id = (long)PersonTypeEnum.Customer, Name = nameof(PersonTypeEnum.Customer), PersonType = PersonTypeEnum.Customer },
                    new Role { Id = (long)PersonTypeEnum.Driver, Name = nameof(PersonTypeEnum.Driver), PersonType = PersonTypeEnum.Driver },
                    new Role { Id = (long)PersonTypeEnum.Admin, Name = nameof(PersonTypeEnum.Admin), PersonType = PersonTypeEnum.Admin }
                );
                await context.SaveChangesAsync();
            }

            // Ensure Statuses exist
            if (!await context.Statuses.AnyAsync())
            {
                context.Statuses.AddRange(
                    new Status { Id = (long)DriverStatus.Pending, Name = nameof(DriverStatus.Pending), Description = "Application submitted and awaiting review." },
                    new Status { Id = (long)DriverStatus.Approved, Name = nameof(DriverStatus.Approved), Description = "Driver application approved and active." },
                    new Status { Id = (long)DriverStatus.Rejected, Name = nameof(DriverStatus.Rejected), Description = "Driver application was rejected." },
                    new Status { Id = (long)DriverStatus.Suspended, Name = nameof(DriverStatus.Suspended), Description = "Driver account has been suspended." }
                );
                await context.SaveChangesAsync();
            }

            // Check if users already seeded
            if (await context.Users.AnyAsync())
            {
                return;
            }

            logger.LogInformation("Seeding Auth database with initial Admins, Customers, and Drivers...");

            var adminPassword = PasswordHasher.Hash("Admin@123!");
            var customerPassword = PasswordHasher.Hash("Customer@123!");
            var driverPassword = PasswordHasher.Hash("Driver@123!");

            // 1. Seed Admin Users
            var adminUsers = new List<User>
            {
                new User
                {
                    Id = 1001,
                    FullName = "System Administrator",
                    Email = "admin@flower.local",
                    PhoneNumber = "01000000001",
                    Gender = Gender.Male,
                    Password = adminPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 1002,
                    FullName = "Super Administrator",
                    Email = "superadmin@flower.local",
                    PhoneNumber = "01000000002",
                    Gender = Gender.Female,
                    Password = adminPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 1003,
                    FullName = "Store Manager",
                    Email = "manager@flower.local",
                    PhoneNumber = "01000000003",
                    Gender = Gender.Male,
                    Password = adminPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            // 2. Seed Customer Users
            var customerUsers = new List<User>
            {
                new User
                {
                    Id = 2001,
                    FullName = "Layla Hassan",
                    Email = "customer1@flower.local",
                    PhoneNumber = "01111111101",
                    Gender = Gender.Female,
                    Password = customerPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2002,
                    FullName = "Mohamed Ali",
                    Email = "customer2@flower.local",
                    PhoneNumber = "01111111102",
                    Gender = Gender.Male,
                    Password = customerPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2003,
                    FullName = "Sarah Smith",
                    Email = "customer3@flower.local",
                    PhoneNumber = "01111111103",
                    Gender = Gender.Female,
                    Password = customerPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2004,
                    FullName = "John Doe",
                    Email = "john.doe@gmail.com",
                    PhoneNumber = "01111111104",
                    Gender = Gender.Male,
                    Password = customerPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2005,
                    FullName = "Omar Khaled",
                    Email = "omar.khaled@gmail.com",
                    PhoneNumber = "01111111105",
                    Gender = Gender.Male,
                    Password = customerPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2006,
                    FullName = "Mona Zaki",
                    Email = "mona.zaki@gmail.com",
                    PhoneNumber = "01111111106",
                    Gender = Gender.Female,
                    Password = customerPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2007,
                    FullName = "Youssef Ahmed",
                    Email = "youssef.ahmed@gmail.com",
                    PhoneNumber = "01111111107",
                    Gender = Gender.Male,
                    Password = customerPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2008,
                    FullName = "Nour Elshamy",
                    Email = "nour.elshamy@gmail.com",
                    PhoneNumber = "01111111108",
                    Gender = Gender.Female,
                    Password = customerPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            // 3. Seed Driver Users
            var driverUsers = new List<User>
            {
                new User
                {
                    Id = 3001,
                    FullName = "Ahmed Driver",
                    Email = "driver1@flower.local",
                    PhoneNumber = "01222222201",
                    Gender = Gender.Male,
                    Password = driverPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 3002,
                    FullName = "Karim Driver",
                    Email = "driver2@flower.local",
                    PhoneNumber = "01222222202",
                    Gender = Gender.Male,
                    Password = driverPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 3003,
                    FullName = "Tarek Driver",
                    Email = "driver3@flower.local",
                    PhoneNumber = "01222222203",
                    Gender = Gender.Male,
                    Password = driverPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 3004,
                    FullName = "Mostafa Driver",
                    Email = "driver4@flower.local",
                    PhoneNumber = "01222222204",
                    Gender = Gender.Male,
                    Password = driverPassword,
                    IsEmailConfirmed = true,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(adminUsers);
            context.Users.AddRange(customerUsers);
            context.Users.AddRange(driverUsers);
            await context.SaveChangesAsync();

            // 4. Assign User Roles
            var userRoles = new List<UserRole>();
            foreach (var admin in adminUsers)
            {
                userRoles.Add(new UserRole { UserId = admin.Id, RoleId = (long)PersonTypeEnum.Admin });
            }
            foreach (var customer in customerUsers)
            {
                userRoles.Add(new UserRole { UserId = customer.Id, RoleId = (long)PersonTypeEnum.Customer });
            }
            foreach (var driver in driverUsers)
            {
                userRoles.Add(new UserRole { UserId = driver.Id, RoleId = (long)PersonTypeEnum.Driver });
            }

            context.UserRoles.AddRange(userRoles);
            await context.SaveChangesAsync();

            // 5. Seed Driver Profiles
            var driverProfiles = new List<DriverUser>
            {
                new DriverUser
                {
                    Id = 3001,
                    UserId = 3001,
                    NationalId = "29501011234567",
                    VehicleType = "Motorcycle",
                    VehiclePlate = "ABC-1234",
                    StatusId = (long)DriverStatus.Approved,
                    CreatedAt = DateTime.UtcNow
                },
                new DriverUser
                {
                    Id = 3002,
                    UserId = 3002,
                    NationalId = "29602022345678",
                    VehicleType = "Van",
                    VehiclePlate = "XYZ-5678",
                    StatusId = (long)DriverStatus.Approved,
                    CreatedAt = DateTime.UtcNow
                },
                new DriverUser
                {
                    Id = 3003,
                    UserId = 3003,
                    NationalId = "29703033456789",
                    VehicleType = "Sedan",
                    VehiclePlate = "CAR-9012",
                    StatusId = (long)DriverStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                },
                new DriverUser
                {
                    Id = 3004,
                    UserId = 3004,
                    NationalId = "29804044567890",
                    VehicleType = "Bicycle",
                    VehiclePlate = "BIKE-3456",
                    StatusId = (long)DriverStatus.Rejected,
                    RejectionReason = "Invalid license documentation.",
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.DriverUsers.AddRange(driverProfiles);
            await context.SaveChangesAsync();

            logger.LogInformation("Auth database successfully seeded with {AdminCount} Admins, {CustomerCount} Customers, and {DriverCount} Drivers.",
                adminUsers.Count, customerUsers.Count, driverUsers.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the Auth database.");
        }
    }
}
