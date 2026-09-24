using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Tests
{
    public class DonationTests
    {
        [Fact]
        public void Donation_ShouldStoreCorrectValues()
        {
            var donation = new Donation
            {
                DonorName = "Test Donor",
                Amount = 500,
                Currency = "ZAR",
                DonationType = "One-Time"
            };

            Assert.Equal("Test Donor", donation.DonorName);
            Assert.Equal(500, donation.Amount);
            Assert.Equal("ZAR", donation.Currency);
            Assert.Equal("One-Time", donation.DonationType);
        }
    }
}