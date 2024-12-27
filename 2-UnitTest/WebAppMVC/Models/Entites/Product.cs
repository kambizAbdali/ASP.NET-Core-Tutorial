using System.ComponentModel.DataAnnotations;

namespace WebAppMVC.Models.Entites
{
    public class Product
    {
        
        public long Id { get; set; }
        [Required(ErrorMessage = "The Name is required.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Price { get; set; }
    }
}