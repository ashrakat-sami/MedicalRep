namespace MedicalRep.Models
{
    public class Product
    {
        [Key]
        

        public string Id { get; set; }
        public string Name { get; set; }

        [Range(1,5)]
        public decimal Rate { get; set; }
        public string Image { get; set; }

        public string Description { get; set; }

        public string Manufacturer { get; set; }

        public decimal Price { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
