using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display the volunteer form
        public IActionResult Index()
        {
            return View();
        }

        // Save volunteer to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                _context.Volunteers.Add(volunteer);
                _context.SaveChanges();

                return RedirectToAction("Confirmation");
            }

            return View(volunteer);
        }

        // Confirmation page
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}