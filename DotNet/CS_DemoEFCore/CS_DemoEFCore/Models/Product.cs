using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS_DemoEFCore.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("Id")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } =string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}
