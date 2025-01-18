using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Parent", Schema = "dbo")]

        public class Customer
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public long Id { get; set; }

            [Required]
            [MaxLength(500)]
            [Column("Name", TypeName = "varchar(200)")]
            public string Name { get; set; }

            [NotNull]
            [EmailAddress]
            [MaxLength(256)]
            public string Email { get; set; }

            [Phone]
            [MaxLength(15)]
            public string PhoneNumber { get; set; }

            [Required]
            [MinLength(3)]
            [MaxLength(100)]
            public string Address { get; set; }

            [Required]
            [Range(0, 120)]
            public int Age { get; set; }

            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            [NotMapped]
            [Column(TypeName = "varchar(200)")]
            public string AdditionalField { get; set; }

            [Display(Name = "Customer Since")]
            [DataType(DataType.Date)]
            [Column("CustomerSince")]
            public DateTime CustomerSince { get; set; }

            [ScaffoldColumn(false)]
            public bool IsDeleted { get; set; } // For soft delete  

            // A collection to demonstrate relationships (e.g., orders)  
            public virtual ICollection<Order> Orders { get; set; } // Assuming an Order class exists  

            // Add more fields and their annotations as necessary  
        }

        // Assuming this class exists for relationships  
        public class Order
        {
            [Key]
            public long OrderId { get; set; }

            public DateTime OrderDate { get; set; }

            [ForeignKey("CustomerId")]
            public long CustomerId { get; set; }
        }
    }
}
