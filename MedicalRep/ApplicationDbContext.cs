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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser table
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.FName).HasMaxLength(100);
                entity.Property(e => e.LName).HasMaxLength(100);
                entity.Property(e => e.City).HasMaxLength(200);
                entity.Property(e => e.Street).HasMaxLength(200);
                entity.Property(e => e.Government).HasMaxLength(100);

                // Relationship with Specialization (Doctor-Specialization)
                entity.HasOne(e => e.Specialization)
                    .WithMany(s => s.Doctors)
                    .HasForeignKey(e => e.SpecializationId)
                    .OnDelete(DeleteBehavior.SetNull);

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

        }
    }
}
