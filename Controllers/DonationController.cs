using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Donation donation)
        {
            if (ModelState.IsValid)
            {
                _context.Donations.Add(donation);
                _context.SaveChanges();

                return RedirectToAction("TaxCertificate", donation);
            }

            return View(donation);
        }

        public IActionResult TaxCertificate(Donation donation)
        {
            return View(donation);
        }
    }
}