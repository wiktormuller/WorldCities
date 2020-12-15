﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldCities.Data
{
    public class CityDTO
    {
        public CityDTO()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Name_ASCII { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
