namespace Monitor_2.Models.Shopping
{
    public class Search
    {
        public int Id { get; set; }
        public string SearchQuery { get; set; } = default!;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public ICollection<KeyWord> KeyWords { get; set; } = new List<KeyWord>();
        public ICollection<SearchResult> SearchResults { get; set; } = new List<SearchResult>();
        public ICollection<User_Search> UserSearches { get; set; } = new List<User_Search>();
    }
}
