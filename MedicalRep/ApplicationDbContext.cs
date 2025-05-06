
namespace MedicalRep
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Category> Categories { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Remove the default ASP.NET Identity table names
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "Users"); });
             modelBuilder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles"); });
            modelBuilder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable(name: "UserRoles"); });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable(name: "UserClaims"); });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable(name: "UserLogins"); });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable(name: "RoleClaims"); });
            modelBuilder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable(name: "UserTokens"); });


            // Configure ApplicationUser table
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");

                // Use Identity's primary key (no need to explicitly define)
                //entity.HasKey(e => e.Id);

                // Configure string properties to have max length
                entity.Property(e => e.FName).HasMaxLength(100);
                entity.Property(e => e.LName).HasMaxLength(100);
                entity.Property(e => e.City).HasMaxLength(200);
                entity.Property(e => e.Street).HasMaxLength(200);
                entity.Property(e => e.Government).HasMaxLength(100);

                // Relationship with Specialization (Doctor-Specialization)
                entity.HasOne(e => e.Specialization)
                    .WithMany(s => s.Doctors)
                    .HasForeignKey(e => e.SpecializationId)
                    .OnDelete(DeleteBehavior.SetNull); // When a Specialization is deleted, set the SpecializationId to null

                // Many-to-many relationship for Visits (MedicalRep - Doctor)
                entity.HasMany(e => e.Visits)
                    .WithOne(v => v.User)  // Assuming 'User' is a 'MedicalRep' or 'Doctor' based on the Visit
                    .HasForeignKey(v => v.MedicalRepId); // Use 'MedicalRepId' to reference the Medical Representative
            });
            //Configure Visits table.
            modelBuilder.Entity<Visit>(entity =>
            {
                entity.ToTable("Visits");

                entity.HasKey(v => v.Id);

                entity.Property(v => v.VisitNotes).HasMaxLength(1000).IsRequired();

                // Foreign Keys for DoctorId and MedicalRepId (Many-to-one relationships)
                entity.HasOne(v => v.User)  // Link 'Visit' to 'ApplicationUser' (MedicalRep or Doctor)
                    .WithMany()  // Assuming a MedicalRep or Doctor can have many Visits
                    .HasForeignKey(v => v.MedicalRepId); // Use MedicalRepId as foreign key
            });


            // Configure Specialization table
            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.ToTable("Specializations");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            });

            // Configure Product table
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)").IsRequired();
                entity.Property(e => e.Image).HasMaxLength(500);
            });

            // Configure Review table
        modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Reviews");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReviewDate).HasColumnType("datetime").IsRequired();
                entity.Property(e => e.Comment).HasMaxLength(1000).IsRequired();
                // Foreign Key for ProductId
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Review>()
                .HasOne(r => r.Doctor)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);



            //Data seeding
            modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Pain Relievers" },
            new Category { Id = 2, Name = "Antibiotics" },
            new Category { Id = 3, Name = "Antacids" },
            new Category { Id = 4, Name = "Bronchodilators" },
            new Category { Id = 5, Name = "Antihistamines" },
            new Category { Id = 6, Name = "Antispasmodics" }
            );


            modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = "1",
                Name = "Panadol",
                Rate = 4.5M,
                Image = "https://abclive1.s3.amazonaws.com/2c2edf5e-aa5c-4940-b5c6-6eb1ac32a016/productimage/5099627630375___XL.jpg",
                Description = "Used to relieve pain and reduce fever.",
                Manufacturer = "Panadol Inc.",
                Price = 6.00M,
                CategoryId = 1, // Pain Relievers
                DosageForm = "Tablet",
                Strength = "500mg",
                ExpiryDate = DateTime.Parse("2025-12-31"),
                Indications = "Used for headaches, body aches, and fever.",
                SideEffects = "Nausea, dizziness, stomach upset.",
                Contraindications = "Liver disease, alcohol consumption.",
                Ingredients = "Paracetamol",
                IsPrescriptionOnly = false
            },
            new Product
            {
                Id = "2",
                Name = "Amoxicillin",
                Rate = 5.0M,
                Image = "https://th.bing.com/th/id/OIP.mzw9ErLwuoZUehYkv18gbgHaFX?w=254&h=183&c=7&r=0&o=5&dpr=1.3&pid=1.7",
                Description = "Antibiotic used to treat bacterial infections.",
                Manufacturer = "Amoxicillin Ltd.",
                Price = 12.00M,
                CategoryId = 2, // Antibiotics
                DosageForm = "Capsule",
                Strength = "250mg",
                ExpiryDate = DateTime.Parse("2025-06-30"),
                Indications = "Used for respiratory and urinary tract infections.",
                SideEffects = "Rash, diarrhea, nausea.",
                Contraindications = "Allergy to penicillin.",
                Ingredients = "Amoxicillin",
                IsPrescriptionOnly = true
            },
            new Product
            {
                Id = "3",
                Name = "Tums",
                Rate = 4.0M,
                Image = "https://th.bing.com/th/id/OIP.k7jkYQfLMGsV5yMr_mG6kwHaHa?w=188&h=188&c=7&r=0&o=5&dpr=1.3&pid=1.7",
                Description = "Antacid used to relieve heartburn and indigestion.",
                Manufacturer = "Tums Inc.",
                Price = 3.50M,
                CategoryId = 3, // Antacids
                DosageForm = "Chewable Tablet",
                Strength = "500mg",
                ExpiryDate = DateTime.Parse("2024-05-30"),
                Indications = "Relieves heartburn, acid indigestion, and upset stomach.",
                SideEffects = "Constipation, nausea.",
                Contraindications = "Kidney disease.",
                Ingredients = "Calcium Carbonate",
                IsPrescriptionOnly = false
            }

            );
            modelBuilder.Entity<Specialization>().HasData(
           new Specialization { Id = 1, Name = "Cardiology", Description = "Specialized in heart diseases and conditions." },
           new Specialization { Id = 2, Name = "Neurology", Description = "Specialized in brain and nervous system disorders." },
           new Specialization { Id = 3, Name = "Pediatrics", Description = "Specialized in the care of infants, children, and adolescents." },
           new Specialization { Id = 4, Name = "Orthopedics", Description = "Specialized in musculoskeletal conditions." },
           new Specialization { Id = 5, Name = "Dermatology", Description = "Specialized in skin, hair, and nail conditions." },
           new Specialization { Id = 6, Name = "Psychiatry", Description = "Specialized in mental health and emotional disorders." },
           new Specialization { Id = 7, Name = "Gastroenterology", Description = "Specialized in the digestive system and its disorders." },
           new Specialization { Id = 8, Name = "Gynecology", Description = "Specialized in female reproductive health." },
           new Specialization { Id = 9, Name = "Ophthalmology", Description = "Specialized in eye conditions and surgery." },
           new Specialization { Id = 10, Name = "Urology", Description = "Specialized in urinary tract and male reproductive organs." }
       );


         
        }



    }
}
