using System.ComponentModel.DataAnnotations;

namespace Shop.Models.Store
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ImgName { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}