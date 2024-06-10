using System.Net;

namespace Monitor_2.Models.Shopping
{
    public class Marketplace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteUrl { get; set; }

        public ICollection<Address>? Addresses { get; set; } = default!;

        public ICollection<Lot> Lots { get; set; } = default!;
    }
}
