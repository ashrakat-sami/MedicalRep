using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalRep.Migrations
{
    /// <inheritdoc />
    public partial class correctseeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Contraindications", "Description", "DosageForm", "ExpiryDate", "Image", "Indications", "Ingredients", "IsPrescriptionOnly", "Manufacturer", "Name", "Price", "Rate", "SideEffects", "Strength" },
                values: new object[,]
                {
                    { "10", 1, "Acute heart failure, cardiogenic shock.", "Beta-blocker for hypertension and heart conditions.", "Tablet", new DateTime(2025, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://th.bing.com/th/id/OIP.B_Q0EkQC8jh5KjK0ndGYcwHaHG?w=193&h=185&c=7&r=0&o=5&cb=iwc2&dpr=1.3&pid=1.7", "Hypertension, angina, heart failure.", "Metoprolol tartrate", true, "Novartis", "Betaloc (Metoprolol)", 40.00m, 4.5m, "Fatigue, dizziness, bradycardia.", "50mg" },
                    { "4", 2, "Hypersensitivity to cephalosporins.", "Third-generation cephalosporin antibiotic for serious infections.", "Injection", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://th.bing.com/th/id/OIP.yu5HYMc0tyh-yWebIPCldgAAAA?cb=iwc2&rs=1&pid=ImgDetMain", "Severe bacterial infections, meningitis, pneumonia.", "Ceftriaxone sodium", true, "Roche", "Ceftriaxone (Rocephin)", 85.00m, 4.7m, "Diarrhea, rash, pain at injection site.", "1g/vial" },
                    { "5", 1, "Severe liver impairment.", "Egyptian paracetamol for pain and fever relief.", "Tablet", new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://th.bing.com/th/id/OIP.mkMq7pl1TZqmgElVI1MgmwHaHa?cb=iwc2&rs=1&pid=ImgDetMain", "Headache, fever, mild to moderate pain.", "Paracetamol", false, "EIPICO", "Adol (Paracetamol)", 8.00m, 4.2m, "Rare at therapeutic doses.", "500mg" },
                    { "6", 3, "GI hemorrhage, mechanical obstruction.", "For nausea, vomiting, bloating and heartburn.", "Tablet", new DateTime(2025, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://th.bing.com/th/id/OIP.inCIZfWjCvKuX6NslxK66gHaHa?cb=iwc2&rs=1&pid=ImgDetMain", "Gastric motility disorders, nausea, vomiting.", "Domperidone maleate", false, "Misr Pharma", "Digest (Domperidone)", 12.50m, 4.3m, "Dry mouth, headache, diarrhea.", "10mg" },
                    { "7", 2, "Pregnancy, severe coronary artery disease.", "Improves cerebral circulation and cognitive function.", "Tablet", new DateTime(2025, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://cdn.youmed.vn/tin-tuc/wp-content/uploads/2020/04/thuoc-cavinton-5mg.jpg", "Cerebrovascular disorders, memory impairment.", "Vinpocetine", true, "Gulf Pharmaceutical Industries", "Cavinton (Vinpocetine)", 45.00m, 4.4m, "Dizziness, nausea, sleep disturbances.", "5mg" },
                    { "8", 3, "Concomitant use with rilpivirine.", "Proton pump inhibitor for acid-related disorders.", "Capsule", new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://prospecte.ro/images/omez-20-mg-omeprazol-caps-gastrorez.jpg", "GERD, peptic ulcers, Zollinger-Ellison syndrome.", "Omeprazole", false, "Dr. Reddy's", "Omez (Omeprazole)", 35.00m, 4.6m, "Headache, diarrhea, abdominal pain.", "20mg" },
                    { "9", 1, "Acute intoxication, MAOI use.", "Opioid analgesic for moderate to severe pain.", "Capsule", new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://th.bing.com/th/id/R.8952c303f57ef1212b064cd84f03ce03?rik=0TC1X2cQDWe3ZQ&pid=ImgRaw&r=0", "Moderate to severe pain management.", "Tramadol hydrochloride", true, "Minapharm", "Tramal (Tramadol)", 55.00m, 4.1m, "Nausea, dizziness, constipation.", "50mg" }
                });

            migrationBuilder.InsertData(
                table: "ProductSpecializations",
                columns: new[] { "ProductId", "SpecializationId" },
                values: new object[,]
                {
                    { "10", 1 },
                    { "4", 3 },
                    { "4", 7 },
                    { "4", 10 },
                    { "5", 1 },
                    { "5", 2 },
                    { "5", 4 },
                    { "6", 7 },
                    { "7", 2 },
                    { "7", 6 },
                    { "8", 7 },
                    { "9", 1 },
                    { "9", 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "10", 1 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "4", 3 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "4", 7 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "4", 10 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "5", 1 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "5", 2 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "5", 4 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "6", 7 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "7", 2 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "7", 6 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "8", 7 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "9", 1 });

            migrationBuilder.DeleteData(
                table: "ProductSpecializations",
                keyColumns: new[] { "ProductId", "SpecializationId" },
                keyValues: new object[] { "9", 4 });

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "10");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "6");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "7");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "8");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "9");
        }
    }
}
