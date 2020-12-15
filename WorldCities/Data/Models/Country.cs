using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorldCities.Data.Models
{
    public class Country
    {
        public Country()
        {

        }

        [Key]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }    //In UTF8
        [JsonPropertyName("iso2")]
        public string ISO2 { get; set; }    //ContryCode (in ISO 3166-1 ALPHA-2 format)
        [JsonPropertyName("iso3")]
        public string ISO3 { get; set; }    //ContryCode (in ISO 3166-1 ALPHA-3 format)

        //Navigation property
        public List<City> Cities { get; set; }
    }
}
