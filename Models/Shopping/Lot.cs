namespace Monitor_2.Models.Shopping
{
    public class Lot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; } // Дозволено NULL значення
        public string Url { get; set; }
        public string PhotoUrl { get; set; }
        public List<string>? FoundKeywords { get; set; }//

        public ICollection<User_Lot> UserLots { get; set; } = default!;
        public ICollection<SearchResult_Lot> SearchResultLots { get; set; } = default!;
        public ICollection<CurrentPriceValue> CurrentPriceValues { get; set; } = default!;

        public int MarketplaceId { get; set; }
        public Marketplace Marketplace { get; set; } = default!;
    }
}
