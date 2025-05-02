using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneratedValues.Model
{
    public class Product
    {
        [Key]
        public int Id { get; set; } // Identity column  

        [Required]
        public string Name { get; set; }

        // Custom value not using identity  
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CustomValue { get; set; } = "Default";

        // Remove Identity from this column  
        public int IdentityColumn { get; set; }

        // Default Value  
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAdd { get; set; }

        // Number of Orders  
        public int OrderNumber { get; set; }

        // Computed Column  
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string ComputedCol { get; set; }
    }
}
