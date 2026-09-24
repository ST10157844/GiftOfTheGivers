using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace GiftOfTheGivers.Functions
{
    public class GenerateTaxCertificate
    {
        private readonly ILogger<GenerateTaxCertificate> _logger;

        public GenerateTaxCertificate(
            ILogger<GenerateTaxCertificate> logger)
        {
            _logger = logger;
        }

        [Function("GenerateTaxCertificate")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                "post")]
            HttpRequestData req)
        {
            _logger.LogInformation(
                "GenerateTaxCertificate function received a request.");

            // Allows us to test the function easily in a browser.
            if (req.Method.Equals(
                "GET",
                StringComparison.OrdinalIgnoreCase))
            {
                var browserResponse =
                    req.CreateResponse(HttpStatusCode.OK);

                await browserResponse.WriteStringAsync(
                    "GenerateTaxCertificate function is running.");

                return browserResponse;
            }

            var requestBody =
                await new StreamReader(req.Body).ReadToEndAsync();

            var donation =
                JsonSerializer.Deserialize<DonationRequest>(
                    requestBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (donation == null ||
                string.IsNullOrWhiteSpace(donation.DonorName) ||
                donation.Amount <= 0)
            {
                var badResponse =
                    req.CreateResponse(HttpStatusCode.BadRequest);

                await badResponse.WriteStringAsync(
                    "Valid donor details and donation amount are required.");

                return badResponse;
            }

            string certificateNumber =
                $"GOTG-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";

            var certificate = new
            {
                CertificateNumber = certificateNumber,
                donation.DonorName,
                donation.Amount,
                donation.Currency,
                donation.DonationType,
                IssuedDate = DateTime.UtcNow
            };

            var response =
                req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(certificate);

            return response;
        }

        public class DonationRequest
        {
            public string DonorName { get; set; } = string.Empty;

            public decimal Amount { get; set; }

            public string Currency { get; set; } = string.Empty;

            public string DonationType { get; set; } = string.Empty;
        }
    }
}