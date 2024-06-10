using Microsoft.AspNetCore.Mvc;
using Monitor_2.Data;
using Monitor_2.Models.Shopping;
using System;

namespace Monitor_2.Controllers
{
    public class SeedShoppingDataController : Controller
    {
        private readonly Monitor_2Context _context;

        public SeedShoppingDataController(Monitor_2Context context)
        {
            _context = context;
        }

        public IActionResult SeedData()
        {
            try
            {
                var promMarketplace = new Marketplace
                {
                    Name = "prom",
                    SiteUrl = "https://prom.ua/ua/",
                    Addresses = null
                };

                var rozetkaMarketplace = new Marketplace
                {
                    Name = "ROZETKA",
                    //SiteUrl = "https://rozetka.com.ua",
                    //Addresses = new System.Collections.Generic.List<string>
                    //{
                    //    "проспект Повітряних Сил, 56, Київ",
                    //    "вулиця Васильківська, 34, Київ"
                    //}
                };

                _context.Marketplaces.AddRange(promMarketplace, rozetkaMarketplace);
                _context.SaveChanges();

                return Ok("Data seeded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
