using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Volunteer> Volunteers { get; set; }

        public DbSet<ReliefUpdate> ReliefUpdates { get; set; }

        public DbSet<Donation> Donations { get; set; }
    }


}
