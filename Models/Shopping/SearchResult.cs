namespace Monitor_2.Models.Shopping
{
    public class SearchResult
    {
        public int Id { get; set; }
        public int LotsCount { get; set; }
        public DateTime ReceivingDate { get; set; }

        public int SearchId { get; set; }
        public Search Search { get; set; } = default!;

        public ICollection<SearchResult_Lot> SearchResultLots { get; set; } = default!;
    }

}
