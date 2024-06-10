namespace Monitor_2.Models.Shopping
{
    public class User_Lot
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public int LotId { get; set; }
        public Lot Lot { get; set; } = default!;
    }
}
