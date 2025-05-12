using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MedicalRep.Models
{
    public class Product
        {
            [Key]
            public string Id { get; set; }= Guid.NewGuid().ToString();

       
            public string? Name { get; set; }

    
            [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5")]
            public decimal? Rate { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal? Price { get; set; }

    

    
            public string? DosageForm { get; set; }

            public string? Strength { get; set; }

          
            public DateTime? ExpiryDate { get; set; }


            public string? Indications { get; set; }

          
            public string? SideEffects { get; set; }

           
            public string? Contraindications { get; set; }

       
            public string? Ingredients { get; set; }

        public bool IsPrescriptionOnly { get; set; } = false;

            public string? Description { get; set; }
            public string? Manufacturer { get; set; }
            public string? Image { get; set; }


        
            public int? CategoryId { get; set; }
            public  Category? Category { get; set; }

         public virtual ICollection<Review> Reviews { set; get; } = new HashSet<Review>();
        public virtual ICollection<ProductSpecialization> ProductSpecializations { get; set; } = new HashSet<ProductSpecialization>();
    }

    public class ProductSpecialization
    {
        public int SpecializationId { get; set; }
        public string ProductId { get; set; }

        public Specialization Specialization { get; set; }
        public Product Product { get; set; }
    }
}

    
