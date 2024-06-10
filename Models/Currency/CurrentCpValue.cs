using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitor_2.Models.Currency
{
    public class CurrentCpValue
    {
        public int Id { get; set; } //Primary key

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; } //переназвати RecordingDate
        public decimal BuyRate { get; set; }
        public decimal SellRate { get; set; }

        public int CurrencyPairId { get; set; }
        public CurrencyPair CurrencyPair { get; set; }

        public int ExchangeCompanyId { get; set; }
        public ExchangeCompany ExchangeCompany { get; set; }

    }
}
