using System.Net.Http.Json;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;

namespace GiftOfTheGivers.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DonationController(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Donation donation)
        {
            if (!ModelState.IsValid)
            {
                return View(donation);
            }

            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();

            var functionUrl =
                _configuration["Functions:TaxCertificateUrl"];

            if (string.IsNullOrWhiteSpace(functionUrl))
            {
                ModelState.AddModelError(
                    "",
                    "Tax certificate function URL is not configured.");

                return View(donation);
            }

            var functionRequest = new
            {
                donorName = donation.DonorName,
                amount = donation.Amount,
                currency = donation.Currency,
                donationType = donation.DonationType
            };

            try
            {
                var response =
                    await _httpClient.PostAsJsonAsync(
                        functionUrl,
                        functionRequest);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(
                        "",
                        "Donation saved, but the tax certificate could not be generated.");

                    return View(donation);
                }

                var certificate =
                    await response.Content
                        .ReadFromJsonAsync<TaxCertificateViewModel>();

                if (certificate == null)
                {
                    ModelState.AddModelError(
                        "",
                        "The tax certificate response was empty.");

                    return View(donation);
                }

                return View("TaxCertificate", certificate);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "",
                    "Donation saved, but the local Azure Function is unavailable.");

                return View(donation);
            }
        }
    }
}