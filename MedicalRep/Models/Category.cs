using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalRep.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public  virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
