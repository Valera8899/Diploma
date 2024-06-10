using Monitor_2.Models.Currency;
using Monitor_2.Models.Shopping;

namespace Monitor_2.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string AddressLine { get; set; } = default!;

        public int? MarketplaceId { get; set; }
        public Marketplace? Marketplace { get; set; }

        public int? ExchangeCompanyId { get; set; }
        public ExchangeCompany? ExchangeCompany { get; set; }
    }
}
