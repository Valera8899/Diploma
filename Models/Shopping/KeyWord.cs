namespace Monitor_2.Models.Shopping
{
    public class KeyWord
    {
        public int Id { get; set; }
        public string Word { get; set; } = default!;
        public int SearchId { get; set; }
        public Search Search { get; set; } = default!;
    }
}
