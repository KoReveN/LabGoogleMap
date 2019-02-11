using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Address
    {
        //public int ID { get; set; } 
        [Key]
        [ForeignKey("Place")]
        public int PlaceId { get; set; } 
        public int CountryId { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int PostalCode { get; set; }
        public string Street { get; set; }
        public int StreetNumber { get; set; }
        public string Remarks { get; set; }
        public Place Place { get; set; }
    }
}
