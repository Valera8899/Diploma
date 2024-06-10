using Microsoft.Extensions.Hosting;

namespace Monitor_2.Models.Currency
{
    public class CurrencyPair
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<CurrentCpValue> CurrentCpValues { get; set; }
        public ICollection<ExCompany_CurrencyPair> ExCompany_CurrencyPairs { get; set; }
    }

}
