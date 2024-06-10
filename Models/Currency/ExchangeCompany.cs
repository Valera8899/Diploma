namespace Monitor_2.Models.Currency
{
    public class ExchangeCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteUrl { get; set; }

        public ICollection<Address> Addresses { get; set; } = default!;

        public ICollection<ExCompany_CurrencyPair> ExCompany_CurrencyPairs { get; set; } = default!;
        public ICollection<CurrentCpValue> CurrentCpValues { get; set; } = default!;
    }
}
