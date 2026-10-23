
using System.Net;
using System.Net.Http.Json;
using GiftOfTheGivers.Controllers;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GiftOfTheGivers.IntegrationTests;

public class DonationControllerTests
{
    [Fact]
    public async Task ValidDonation_SavesDonation_AndReturnsTaxCertificate()
    {
        using var context = CreateContext();

        var handler = new StubHttpMessageHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new TaxCertificateViewModel
                {
                    CertificateNumber = "GOTG-TEST-001",
                    DonorName = "Test Donor",
                    Amount = 100m,
                    Currency = "ZAR",
                    DonationType = "One-Time",
                    IssuedDate = DateTime.UtcNow
                })
            }));

        var controller = CreateController(context, handler);
        var donation = CreateValidDonation();

        var result = await controller.Index(donation);

        Assert.Single(context.Donations);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("TaxCertificate", view.ViewName);

        var certificate =
            Assert.IsType<TaxCertificateViewModel>(view.Model);

        Assert.Equal("GOTG-TEST-001", certificate.CertificateNumber);
        Assert.Equal(100m, certificate.Amount);
    }

    [Fact]
    public async Task AzureFunctionUnavailable_DonationRemainsSaved_AndErrorIsShown()
    {
        using var context = CreateContext();

        var handler = new StubHttpMessageHandler(_ =>
            throw new HttpRequestException("Test Function unavailable"));

        var controller = CreateController(context, handler);
        var donation = CreateValidDonation();

        var result = await controller.Index(donation);

        Assert.Single(context.Donations);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Null(view.ViewName);
        Assert.Same(donation, view.Model);

        Assert.Contains(
            controller.ModelState.Values.SelectMany(v => v.Errors),
            error => error.ErrorMessage ==
                "Donation saved, but the local Azure Function is unavailable.");
    }

    [Fact]
    public async Task InvalidDonation_IsNotSaved()
    {
        using var context = CreateContext();

        var handler = new StubHttpMessageHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        var controller = CreateController(context, handler);
        var donation = CreateValidDonation();
        donation.Amount = 0m;

        // Simulate the model-validation error supplied by MVC.
        controller.ModelState.AddModelError(
            nameof(Donation.Amount),
            "Donation amount must be greater than zero.");

        var result = await controller.Index(donation);

        Assert.Empty(context.Donations);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(donation, view.Model);
        Assert.False(controller.ModelState.IsValid);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static Donation CreateValidDonation()
    {
        return new Donation
        {
            DonorName = "Test Donor",
            Amount = 100m,
            Currency = "ZAR",
            DonationType = "One-Time",
            IsAnonymous = false
        };
    }

    private static DonationController CreateController(
        ApplicationDbContext context,
        HttpMessageHandler handler)
    {
        var client = new HttpClient(handler);

        var factory = new StubHttpClientFactory(client);

        var settings = new Dictionary<string, string?>
        {
            ["Functions:TaxCertificateUrl"] =
                "http://localhost/api/GenerateTaxCertificate"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var controller = new DonationController(
            context,
            factory,
            configuration);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        return controller;
    }

    private sealed class StubHttpClientFactory(HttpClient client)
        : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, Task<HttpResponseMessage>> responder)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return responder(request);
        }
    }
}
