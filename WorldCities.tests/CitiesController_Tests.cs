using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using WorldCities.Controllers;
using WorldCities.Data;
using WorldCities.Data.Models;
using Xunit;

namespace WorldCities.tests
{
    public class CitiesController_Tests
    {
        //Test the GetCity() method
        [Fact]
        public async void GetCity()
        {
            //Arrange - define the required assets
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WorldCities")
                .Options;

            var storeOptions = Options.Create(new OperationalStoreOptions());

            using (var context = new ApplicationDbContext(options, storeOptions))
            {
                context.Add(new City()
                {
                    Id = 1,
                    CountryId = 1,
                    Latitude = 1,
                    Longitude = 1,
                    Name = "TestCity1"
                });
                context.SaveChanges();
            }
            City city_existing = null;
            City city_nonExisting = null;

            //Act - invoke the testing subject's behavior
            using (var context = new ApplicationDbContext(options, storeOptions))
            {
                var controller = new CitiesController(context);
                city_existing = (await controller.GetCity(1)).Value;
                city_nonExisting = (await controller.GetCity(2)).Value;
            }

            //Assert - verify that conditions are met by evaluating the behavior's return value or measuring it against some user-defnied rules
            Assert.True(city_existing != null && city_nonExisting == null);
        }
    }
}
