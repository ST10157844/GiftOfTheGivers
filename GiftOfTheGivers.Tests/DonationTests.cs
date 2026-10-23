//using GiftOfTheGivers.Models;

//namespace GiftOfTheGivers.Tests
//{
//    public class DonationTests
//    {
//        [Fact]
//        public void Donation_ShouldStoreCorrectValues()
//        {
//            var donation = new Donation
//            {
//                DonorName = "Test Donor",
//                Amount = 500,
//                Currency = "ZAR",
//                DonationType = "One-Time"
//            };

//            Assert.Equal("Test Donor", donation.DonorName);
//            Assert.Equal(500, donation.Amount);
//            Assert.Equal("ZAR", donation.Currency);
//            Assert.Equal("One-Time", donation.DonationType);
//        }
//    }
//}
using System.ComponentModel.DataAnnotations;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.Helpers;

namespace GiftOfTheGivers.Tests
{
    public class DonationTests
    {
        private static bool Validate(
            Donation donation,
            out List<ValidationResult> results)
        {
            results = new List<ValidationResult>();

            return Validator.TryValidateObject(
                donation,
                new ValidationContext(donation),
                results,
                true);
        }

        [Fact]
        public void ValidNamedDonation_ShouldPassValidation()
        {
            var donation = new Donation
            {
                DonorName = "Test Donor",
                Amount = 500,
                Currency = "ZAR",
                DonationType = "One-Time"
            };

            bool valid = Validate(donation, out _);

            Assert.True(valid);
        }

        [Fact]
        public void AnonymousDonation_ShouldPassWithoutDonorName()
        {
            var donation = new Donation
            {
                IsAnonymous = true,
                Amount = 250,
                Currency = "ZAR",
                DonationType = "One-Time"
            };

            bool valid = Validate(donation, out _);

            Assert.True(valid);
        }

        [Fact]
        public void ZeroAmount_ShouldFailValidation()
        {
            var donation = new Donation
            {
                DonorName = "Test",
                Amount = 0,
                Currency = "ZAR",
                DonationType = "One-Time"
            };

            bool valid = Validate(donation, out _);

            Assert.False(valid);
        }

        [Fact]
        public void UnsupportedCurrency_ShouldFailValidation()
        {
            var donation = new Donation
            {
                DonorName = "Test",
                Amount = 100,
                Currency = "GBP",
                DonationType = "One-Time"
            };

            bool valid = Validate(donation, out _);

            Assert.False(valid);
        }

        [Fact]
        public void InvalidDonationType_ShouldFailValidation()
        {
            var donation = new Donation
            {
                DonorName = "Test",
                Amount = 100,
                Currency = "ZAR",
                DonationType = "Invalid"
            };

            bool valid = Validate(donation, out _);

            Assert.False(valid);
        }

        [Fact]
        public void DonationFormatter_ShouldFormatZarAmount()
        {
            var result =
                DonationFormatter.FormatAmount(500, "ZAR");

            Assert.Equal("ZAR 500.00", result);
        }

        [Fact]
        public void DonationDate_ShouldBeInitialised()
        {
            var donation = new Donation();

            Assert.True(
                donation.DonationDate <= DateTime.Now);
        }
    }
}