using GiftOfTheGivers.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ReliefUpdates()
        {
            return View();
        }

        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Volunteers()
        {
            var volunteers = _context.Volunteers
                                     .OrderByDescending(v => v.DateRegistered)
                                     .ToList();

            return View(volunteers);
        }
    }
}