namespace GiftOfTheGivers.Helpers
{
    public static class DonationFormatter
    {
        public static string FormatAmount(decimal amount, string currency)
        {
            return $"{currency} {amount:N2}";
        }
    }
}