
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
        public DbSet<ProductSpecialization> ProductSpecializations { get; set; }


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

            });
            // Configure Visits table
            modelBuilder.Entity<Visit>(entity =>
            {
                entity.Property(v => v.VisitNotes).HasMaxLength(1000);

                // Relationship with Doctor
                entity.HasOne(v => v.Doctor)
                    .WithMany(d => d.DoctorVisits)
                    .HasForeignKey(v => v.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with MedicalRep
                entity.HasOne(v => v.MedicalRep)
                    .WithMany(m => m.MedicalRepVisits)
                    .HasForeignKey(v => v.MedicalRepId)
                    .OnDelete(DeleteBehavior.Restrict);
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

            modelBuilder.Entity<ProductSpecialization>()
            .HasKey(ps => new { ps.ProductId, ps.SpecializationId });

            modelBuilder.Entity<ProductSpecialization>()
             .HasOne(ps => ps.Product)
             .WithMany(p => p.ProductSpecializations)
             .HasForeignKey(ps => ps.ProductId);

            modelBuilder.Entity<ProductSpecialization>()
              .HasOne(ps => ps.Specialization)
              .WithMany(s => s.ProductSpecializations)
              .HasForeignKey(ps => ps.SpecializationId);

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
                },
                new Product
                {
                    Id = "4",
                    Name = "Ceftriaxone (Rocephin)",
                    Rate = 4.7M,
                    Image = "https://th.bing.com/th/id/OIP.yu5HYMc0tyh-yWebIPCldgAAAA?cb=iwc2&rs=1&pid=ImgDetMain",
                    Description = "Third-generation cephalosporin antibiotic for serious infections.",
                    Manufacturer = "Roche",
                    Price = 85.00M,
                    CategoryId = 2, // Antibiotics
                    DosageForm = "Injection",
                    Strength = "1g/vial",
                    ExpiryDate = DateTime.Parse("2025-11-30"),
                    Indications = "Severe bacterial infections, meningitis, pneumonia.",
                    SideEffects = "Diarrhea, rash, pain at injection site.",
                    Contraindications = "Hypersensitivity to cephalosporins.",
                    Ingredients = "Ceftriaxone sodium",
                    IsPrescriptionOnly = true
                },
                new Product
                {
                    Id = "5",
                    Name = "Adol (Paracetamol)",
                    Rate = 4.2M,
                    Image = "https://th.bing.com/th/id/OIP.mkMq7pl1TZqmgElVI1MgmwHaHa?cb=iwc2&rs=1&pid=ImgDetMain",
                    Description = "Egyptian paracetamol for pain and fever relief.",
                    Manufacturer = "EIPICO",
                    Price = 8.00M,
                    CategoryId = 1, // Pain Relievers
                    DosageForm = "Tablet",
                    Strength = "500mg",
                    ExpiryDate = DateTime.Parse("2026-01-31"),
                    Indications = "Headache, fever, mild to moderate pain.",
                    SideEffects = "Rare at therapeutic doses.",
                    Contraindications = "Severe liver impairment.",
                    Ingredients = "Paracetamol",
                    IsPrescriptionOnly = false
                },
                new Product
                {
                    Id = "6",
                    Name = "Digest (Domperidone)",
                    Rate = 4.3M,
                    Image = "https://th.bing.com/th/id/OIP.inCIZfWjCvKuX6NslxK66gHaHa?cb=iwc2&rs=1&pid=ImgDetMain",
                    Description = "For nausea, vomiting, bloating and heartburn.",
                    Manufacturer = "Misr Pharma",
                    Price = 12.50M,
                    CategoryId = 3, // Antacids
                    DosageForm = "Tablet",
                    Strength = "10mg",
                    ExpiryDate = DateTime.Parse("2025-08-15"),
                    Indications = "Gastric motility disorders, nausea, vomiting.",
                    SideEffects = "Dry mouth, headache, diarrhea.",
                    Contraindications = "GI hemorrhage, mechanical obstruction.",
                    Ingredients = "Domperidone maleate",
                    IsPrescriptionOnly = false
                },
                new Product
                {
                    Id = "7",
                    Name = "Cavinton (Vinpocetine)",
                    Rate = 4.4M,
                    Image = "https://cdn.youmed.vn/tin-tuc/wp-content/uploads/2020/04/thuoc-cavinton-5mg.jpg",
                    Description = "Improves cerebral circulation and cognitive function.",
                    Manufacturer = "Gulf Pharmaceutical Industries",
                    Price = 45.00M,
                    CategoryId = 2, // Neurology-related
                    DosageForm = "Tablet",
                    Strength = "5mg",
                    ExpiryDate = DateTime.Parse("2025-09-30"),
                    Indications = "Cerebrovascular disorders, memory impairment.",
                    SideEffects = "Dizziness, nausea, sleep disturbances.",
                    Contraindications = "Pregnancy, severe coronary artery disease.",
                    Ingredients = "Vinpocetine",
                    IsPrescriptionOnly = true
                },
                new Product
                {
                    Id = "8",
                    Name = "Omez (Omeprazole)",
                    Rate = 4.6M,
                    Image = "https://prospecte.ro/images/omez-20-mg-omeprazol-caps-gastrorez.jpg",
                    Description = "Proton pump inhibitor for acid-related disorders.",
                    Manufacturer = "Dr. Reddy's",
                    Price = 35.00M,
                    CategoryId = 3, // Antacids
                    DosageForm = "Capsule",
                    Strength = "20mg",
                    ExpiryDate = DateTime.Parse("2025-10-31"),
                    Indications = "GERD, peptic ulcers, Zollinger-Ellison syndrome.",
                    SideEffects = "Headache, diarrhea, abdominal pain.",
                    Contraindications = "Concomitant use with rilpivirine.",
                    Ingredients = "Omeprazole",
                    IsPrescriptionOnly = false
                },
                new Product
                {
                    Id = "9",
                    Name = "Tramal (Tramadol)",
                    Rate = 4.1M,
                    Image = "https://th.bing.com/th/id/R.8952c303f57ef1212b064cd84f03ce03?rik=0TC1X2cQDWe3ZQ&pid=ImgRaw&r=0",
                    Description = "Opioid analgesic for moderate to severe pain.",
                    Manufacturer = "Minapharm",
                    Price = 55.00M,
                    CategoryId = 1, // Pain Relievers
                    DosageForm = "Capsule",
                    Strength = "50mg",
                    ExpiryDate = DateTime.Parse("2025-12-31"),
                    Indications = "Moderate to severe pain management.",
                    SideEffects = "Nausea, dizziness, constipation.",
                    Contraindications = "Acute intoxication, MAOI use.",
                    Ingredients = "Tramadol hydrochloride",
                    IsPrescriptionOnly = true
                },
                new Product
                {
                    Id = "10",
                    Name = "Betaloc (Metoprolol)",
                    Rate = 4.5M,
                    Image = "https://th.bing.com/th/id/OIP.B_Q0EkQC8jh5KjK0ndGYcwHaHG?w=193&h=185&c=7&r=0&o=5&cb=iwc2&dpr=1.3&pid=1.7",
                    Description = "Beta-blocker for hypertension and heart conditions.",
                    Manufacturer = "Novartis",
                    Price = 40.00M,
                    CategoryId = 1, // Cardiology
                    DosageForm = "Tablet",
                    Strength = "50mg",
                    ExpiryDate = DateTime.Parse("2025-07-31"),
                    Indications = "Hypertension, angina, heart failure.",
                    SideEffects = "Fatigue, dizziness, bradycardia.",
                    Contraindications = "Acute heart failure, cardiogenic shock.",
                    Ingredients = "Metoprolol tartrate",
                    IsPrescriptionOnly = true
                }
            );





            // إضافة بيانات ProductSpecialization
            modelBuilder.Entity<ProductSpecialization>().HasData(
                 // Panadol (Id: 1) - مناسب لعدة تخصصات
                 new ProductSpecialization { ProductId = "1", SpecializationId = 1 }, // Cardiology
                new ProductSpecialization { ProductId = "1", SpecializationId = 2 }, // Neurology
                new ProductSpecialization { ProductId = "1", SpecializationId = 4 }, // Orthopedics

                // Amoxicillin (Id: 2) - مضاد حيوي واسع المدى
                new ProductSpecialization { ProductId = "2", SpecializationId = 3 }, // Pediatrics
                new ProductSpecialization { ProductId = "2", SpecializationId = 7 }, // Gastroenterology
                new ProductSpecialization { ProductId = "2", SpecializationId = 10 }, // Urology

                // Tums (Id: 3) - مضاد للحموضة
                new ProductSpecialization { ProductId = "3", SpecializationId = 7 }, // Gastroenterology
                new ProductSpecialization { ProductId = "3", SpecializationId = 8 },//Gynecology

                             // Ceftriaxone
                new ProductSpecialization { ProductId = "4", SpecializationId = 3 }, // Pediatrics
                new ProductSpecialization { ProductId = "4", SpecializationId = 7 }, // Gastroenterology
                new ProductSpecialization { ProductId = "4", SpecializationId = 10 }, // Urology

                // Adol
                new ProductSpecialization { ProductId = "5", SpecializationId = 1 }, // Cardiology
                new ProductSpecialization { ProductId = "5", SpecializationId = 2 }, // Neurology
                new ProductSpecialization { ProductId = "5", SpecializationId = 4 }, // Orthopedics

                // Digest
                new ProductSpecialization { ProductId = "6", SpecializationId = 7 }, // Gastroenterology

                // Cavinton
                new ProductSpecialization { ProductId = "7", SpecializationId = 2 }, // Neurology
                new ProductSpecialization { ProductId = "7", SpecializationId = 6 }, // Psychiatry

                // Omez
                new ProductSpecialization { ProductId = "8", SpecializationId = 7 }, // Gastroenterology

                // Tramal
                new ProductSpecialization { ProductId = "9", SpecializationId = 1 }, // Cardiology
                new ProductSpecialization { ProductId = "9", SpecializationId = 4 }, // Orthopedics

                // Betaloc
                new ProductSpecialization { ProductId = "10", SpecializationId = 1 } // Cardiology

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
