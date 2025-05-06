using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalRep.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
   table: "Products",
   columns: new[] { "Id", "CategoryId", "Contraindications", "Description", "DosageForm", "ExpiryDate", "Image", "Indications", "Ingredients", "IsPrescriptionOnly", "Manufacturer", "Name", "Price", "Rate", "SideEffects", "Strength" },
   values: new object[,]
   {
                { "4", 1, "Liver disease, alcohol use.", "Analgesic and antipyretic used to reduce pain and fever.", "Tablet", new DateTime(2025, 12, 31), "/images/products/Cetal.jpeg", "Headaches, fever, flu.", "Paracetamol", false, "El Nasr Pharmaceutical", "Cetal", 4.00m, 4.4m, "Liver toxicity if overdosed.", "500mg" },
                { "5", 2, "Allergy to penicillins.", "Broad-spectrum antibiotic for bacterial infections.", "Capsule", new DateTime(2026, 1, 15), "/images/products/Augmantine.jpeg", "Respiratory, urinary, and skin infections.", "Amoxicillin + Clavulanic acid", true, "Hikma Pharma", "Augmentin", 18.00m, 4.7m, "Nausea, diarrhea, rash.", "625mg" },
                { "6", 3, "Severe kidney disease.", "Antacid for heartburn and indigestion relief.", "Tablet", new DateTime(2025, 9, 30), "/images/products/Gaviscon.jpeg", "Acid reflux, indigestion.", "Alginic acid + Sodium bicarbonate", false, "Delta Pharma", "Gaviscon", 6.50m, 4.0m, "Bloating, nausea.", "500mg" },
                { "7", 4, "Heart conditions, high blood pressure.", "Bronchodilator for asthma symptoms.", "Inhaler", new DateTime(2026, 2, 1), "/images/products/Ventolex.jpeg", "Bronchial asthma, wheezing.", "Salbutamol", true, "Medical Union Pharmaceuticals", "Ventolex", 15.00m, 4.3m, "Tremors, nervousness.", "100mcg/dose" },
                { "8", 5, "Severe liver disease.", "Antihistamine for allergy symptoms.", "Tablet", new DateTime(2025, 11, 1), "/images/products/Lorinase.jpeg", "Allergic rhinitis, hay fever.", "Loratadine + Pseudoephedrine", false, "Global Napi Pharmaceuticals", "Lorinase", 9.00m, 4.2m, "Dry mouth, dizziness.", "10mg" },
                { "9", 6, "Glaucoma, urinary retention.", "Antispasmodic used for stomach cramps and IBS.", "Tablet", new DateTime(2025, 8, 20), "/images/products/Spasmoproct.jpeg", "Abdominal cramps, IBS.", "Hyoscine Butylbromide", true, "Amoun Pharmaceutical Co.", "Spasmoproct", 7.50m, 4.1m, "Dry mouth, blurred vision.", "10mg" }
   });



        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
