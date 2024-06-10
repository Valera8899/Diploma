namespace Monitor_2.Models.Currency
{
    public class ExCompany_CurrencyPair
    {
        public int ExchangeCompanyId { get; set; }
        public ExchangeCompany ExchangeCompany { get; set; }

        public int CurrencyPairId { get; set; }
        public CurrencyPair CurrencyPair { get; set; }
    }
}
