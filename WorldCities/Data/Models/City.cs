using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCities.Data.Models
{
    public class City
    {
        public City()
        {

        }

        [Key]
        [Required]
        public int Id { get; set; } //PK
        public string Name { get; set; }    //In UTF8
        public string  Name_ASCII { get; set; }

        [Column(TypeName = "decimal(7,4)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(7,4)")]
        public decimal Longitude { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }  //FK

        //Navigation property
        public Country Country { get; set; }
    }
}
