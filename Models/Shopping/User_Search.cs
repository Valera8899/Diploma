namespace Monitor_2.Models.Shopping
{
    public class User_Search
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public int SearchId { get; set; }
        public Search Search { get; set; } = default!;
    }
}
