using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MedicalRep.Models;
using MedicalRep;
using System.IO;

namespace MedicalRep.Models
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            await SeedRolesAsync(serviceProvider);
            await SeedDoctorsAsync(serviceProvider);
            await SeedMedicalRepsAsync(serviceProvider);
            await SeedVisitsAsync(serviceProvider);
            //await SeedProductsAsync(serviceProvider);
        }

        private static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "Doctor", "MedicalRep" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedDoctorsAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var doctors = new List<(string Email, string FName, string LName, int SpecializationId, string Location, string City, string Street, string Government, string Phone)>
    {
        ("dr.Remmassmo1@zagazig.com", "Remass", "Mohamed", 1, "Zagazig Clinic 1", "Zagazig", "El Galaa Street", "Sharqia", "01056330922"),
        ("dr.NourMo1@mansoura.com", "Nour", "Mohamed", 2, "Mansoura Clinic A", "Mansoura", "Talaat Harb Street", "Dakahlia", "01046330922"),
        ("dr.Yassinmo2@zagazig.com", "Yassin", "Mohamed", 3, "Zagazig Clinic 2", "Zagazig", "Al Horreya Street", "Sharqia", "01036330922"),
        ("dr.AmrSamir3@mansoura.com", "Amr", "Samir", 4, "Mansoura Clinic B", "Mansoura", "Al Geish Street", "Dakahlia", "01026330922"),
        ("dr.AtefNossier3@zagazig.com", "Atef", "Nosser", 5, "Zagazig Clinic 3", "Zagazig", "Al Salam Street", "Sharqia", "01016330922"),

        // الأطباء الجدد (SpecializationId من 6 إلى 10)
        ("dr.MonaAli6@cairo.com", "Mona", "Ali", 6, "Cairo Clinic 1", "Cairo", "Nasr City Street", "Cairo", "01066330927"),
        ("dr.HassanOmar7@giza.com", "Hassan", "Omar", 7, "Giza Clinic", "Giza", "Pyramids Street", "Giza", "01066330925"),
        ("dr.LailaNabil8@alex.com", "Laila", "Nabil", 8, "Alex Clinic 1", "Alexandria", "Corniche Street", "Alexandria", "01066330923"),
        ("dr.KhaledSaeed9@cairo.com", "Khaled", "Saeed", 9, "Cairo Clinic 2", "Cairo", "Heliopolis Street", "Cairo", "01066330922"),
        ("dr.RanaFahmy10@ismailia.com", "Rana", "Fahmy", 10, "Ismailia Clinic", "Ismailia", "Canal Street", "Ismailia", "01066330929")
    };


            foreach (var (email, fname, lname, specId, location, city, street, government, phone) in doctors)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FName = fname,
                        LName = lname,
                        SpecializationId = specId,
                        Location = location,
                        City = city,
                        Street = street,
                        Government = government,
                        PhoneNumber = phone
                    };

                    var result = await userManager.CreateAsync(user, "Test@123");

                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, "Doctor");
                    else
                        Console.WriteLine($"❌ Failed to create doctor {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }


        }

        private static async Task SeedMedicalRepsAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var reps = new List<(string Email, string FName, string LName, string Location, string City, string Street, string Government, string Phone)>
        {
            ("rep.ahmedSalah@company.com", "Ahmed", "Mahmoud", "Mansoura", "Mansoura", "El Galaa Street", "Daqhlia", "01000000001"),
            ("rep.Aya@company.com", "Sara", "Ali", "Mansoura", "Mansoura", "El Galaa Street", "Daqhlia", "01000000001"),
            ("rep.Ashrakat@company.com", "Mohamed", "Samir", "Mansoura" , "Mansoura" , "El Galaa Street" , "Daqhlia" , "01000000001")
        };

            foreach (var (email, fname, lname, Location, City, Street, Government, PhoneNumber) in reps)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FName = fname,
                        LName = lname,
                        Location = Location,
                        City = City,
                        Street = Street,
                        Government = Government,
                        PhoneNumber = PhoneNumber

                    };

                    var result = await userManager.CreateAsync(user, "Test@123");

                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, "MedicalRep");
                    else
                        Console.WriteLine($"❌ Failed to create medical rep {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
        private static async Task SeedVisitsAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (context.Visits.Any())
                return;

            var doctors = await userManager.GetUsersInRoleAsync("Doctor");
            var reps = await userManager.GetUsersInRoleAsync("MedicalRep");

            if (!doctors.Any() || !reps.Any())
            {
                Console.WriteLine("❌ Cannot seed visits. Make sure doctors and medical reps exist.");
                return;
            }

            var visits = new List<Visit>();
            var rand = new Random();

            var statuses = new List<VisitStatus>
    {
        VisitStatus.Scheduled, VisitStatus.Scheduled, VisitStatus.Scheduled, VisitStatus.Scheduled, VisitStatus.Scheduled,
        VisitStatus.Completed, VisitStatus.Completed, VisitStatus.Completed, VisitStatus.Completed,
        VisitStatus.InProgress, VisitStatus.InProgress,
        VisitStatus.Cancelled, VisitStatus.Cancelled,
        VisitStatus.Rescheduled,
        VisitStatus.NoShow
    };

            for (int i = 0; i < 15; i++)
            {
                var doctor = doctors[rand.Next(doctors.Count)];
                var rep = reps[rand.Next(reps.Count)];

                var visitDate = i < 5
                    ? DateTime.Today
                    : i < 10
                        ? DateTime.Today.AddDays(1)
                        : DateTime.Today.AddDays(2);

                var status = statuses[i];

                visits.Add(new Visit
                {
                    DoctorId = doctor.Id,
                    MedicalRepId = rep.Id,
                    VisitDate = visitDate.AddHours(rand.Next(9, 17)),
                    VisitNotes = $"زيارة حالة: {status}",
                    Status = status,
                    ActualVisitDate = (status == VisitStatus.Completed || status == VisitStatus.InProgress)
                                      ? visitDate.AddHours(rand.Next(9, 17)) : null,
                    Feedback = status == VisitStatus.Completed ? "تمت بنجاح" : null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            context.Visits.AddRange(visits);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Seeded 15 visits with various statuses successfully.");
        }
    }
}