using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FjordNestPro.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [Required, MaxLength(255)]
        public required string StreetName { get; set; }

        [Required, MaxLength(255)]
        public required string City { get; set; }

        [Required, MaxLength(10)]
        public required string Postcode { get; set; }

        public ICollection<Property>? Properties { get; set; }
    }
}

