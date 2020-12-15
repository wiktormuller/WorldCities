﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldCities.Data;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using WorldCities.Data.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace WorldCities.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public SeedController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            // NOTE: This method has been updated on 2020.09.13.
            // The new version is more efficient than the code described in the book's Chapter 4.
            // ref.: https://github.com/PacktPublishing/ASP.NET-Core-3-and-Angular-9-Third-Edition/issues/15

            var path = Path.Combine(
                _env.ContentRootPath,
                String.Format("Data/Source/worldcities.xlsx"));

            using (var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read))
            {
                using (var ep = new ExcelPackage(stream))
                {
                    // get the first worksheet

                    var ws = ep.Workbook.Worksheets[0];

                    // initialize the record counters
                    var nCountries = 0;
                    var nCities = 0;

                    #region Import all Countries
                    // create a list containing all the countries already existing
                    // into the Database (it will be empty on first run).
                    var lstCountries = _context.Countries.ToList();

                    // iterates through all rows, skipping the first one
                    for (int nRow = 2;
                        nRow <= ws.Dimension.End.Row;
                        nRow++)
                    {
                        var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];
                        var name = row[nRow, 5].GetValue<string>();

                        // does this country already exist in the database?
                        if (lstCountries.Where(c => c.Name == name).Count() == 0)
                        {
                            // create the Country entity and fill it with xlsx data
                            var country = new Country();
                            country.Name = name;
                            country.ISO2 = row[nRow, 6].GetValue<string>();
                            country.ISO3 = row[nRow, 7].GetValue<string>();

                            // add the new country to the DB context
                            _context.Countries.Add(country);

                            // store the country to retrieve its Id later on
                            lstCountries.Add(country);

                            // increment the counter
                            nCountries++;
                        }
                    }

                    // save all the countries into the Database
                    if (nCountries > 0) await _context.SaveChangesAsync();
                    #endregion

                    #region Import all Cities
                    // create a list containing all the cities already existing
                    // into the Database (it will be empty on first run).
                    var lstCities = _context.Cities.ToList();

                    // iterates through all rows, skipping the first one
                    for (int nRow = 2;
                        nRow <= ws.Dimension.End.Row;
                        nRow++)
                    {
                        var row = ws.Cells[nRow, 1, nRow, ws.Dimension.End.Column];

                        var name = row[nRow, 1].GetValue<string>();
                        var name_ASCII = row[nRow, 2].GetValue<string>();
                        var countryName = row[nRow, 5].GetValue<string>();
                        var lat = row[nRow, 3].GetValue<decimal>();
                        var lon = row[nRow, 4].GetValue<decimal>();
                        // retrieve country and countryId
                        var country = lstCountries.Where(c => c.Name == countryName)
                            .FirstOrDefault();
                        var countryId = country.Id;

                        // does this city already exist in the database?
                        if (lstCities.Where(
                            c => c.Name == name
                            && c.Latitude == lat
                            && c.Longitude == lon
                            && c.CountryId == countryId
                        ).Count() == 0)
                        {
                            // create the City entity and fill it with xlsx data
                            var city = new City();
                            city.Name = name;
                            city.Name_ASCII = name_ASCII;
                            city.Latitude = lat;
                            city.Longitude = lon;
                            city.CountryId = countryId;

                            // add the new city to the DB context
                            _context.Cities.Add(city);

                            // increment the counter
                            nCities++;
                        }
                    }

                    // save all the cities into the Database
                    if (nCities > 0) await _context.SaveChangesAsync();
                    #endregion

                    return new JsonResult(new
                    {
                        Cities = nCities,
                        Countries = nCountries
                    });
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            // setup the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            // create the default roles (if they doesn't exist yet)
            if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
                await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));

            if (await _roleManager.FindByNameAsync(role_Administrator) == null)
                await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

            // create a list to track the newly added users
            var addedUserList = new List<ApplicationUser>();

            // check if the admin user already exist
            var email_Admin = "admin@email.com";
            if (await _userManager.FindByNameAsync(email_Admin) == null)
            {
                // create a new admin ApplicationUser account
                var user_Admin = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_Admin,
                    Email = email_Admin,
                };

                // insert the admin user into the DB
                await _userManager.CreateAsync(user_Admin, "MySecr3t$");

                // assign the "RegisteredUser" and "Administrator" roles
                await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
                await _userManager.AddToRoleAsync(user_Admin, role_Administrator);

                // confirm the e-mail and remove lockout
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;

                // add the admin user to the added users list
                addedUserList.Add(user_Admin);
            }

            // check if the standard user already exist
            var email_User = "user@email.com";
            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                // create a new standard ApplicationUser account
                var user_User = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_User,
                    Email = email_User
                };

                // insert the standard user into the DB
                await _userManager.CreateAsync(user_User, "MySecr3t$");

                // assign the "RegisteredUser" role
                await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

                // confirm the e-mail and remove lockout
                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled = false;

                // add the standard user to the added users list
                addedUserList.Add(user_User);
            }

            // if we added at least one user, persist the changes into the DB
            if (addedUserList.Count > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                Count = addedUserList.Count,
                Users = addedUserList
            });
        }
    }
}