namespace Monitor_2.Models.Shopping
{
    public class SearchResult_Lot
    {
        public int SearchResultId { get; set; }
        public SearchResult SearchResult { get; set; } = default!;

        public int LotId { get; set; }
        public Lot Lot { get; set; } = default!;
    }

}
