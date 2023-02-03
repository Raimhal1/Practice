using System.ComponentModel.DataAnnotations;

namespace Shop.Models.Store
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; }

        [Required] 
        public string Price { get; set; }

        [Required] 
        public string Color { get; set; }

        [Required] 
        public string Sizes { get; set; }

        [Required] 
        public string Description { get; set; }

        [Required]
        public string ImgName { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}