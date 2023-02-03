using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models.Store
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [DefaultValue(0)]
        public int Access { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}
