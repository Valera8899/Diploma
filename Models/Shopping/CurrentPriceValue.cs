using System.ComponentModel.DataAnnotations;

namespace Monitor_2.Models.Shopping
{
    public class CurrentPriceValue
    {
        public int Id { get; set; } //Primary key

        [DataType(DataType.Date)]
        public DateTime RecordingDate { get; set; }
        public decimal Price { get; set; }

        public int LotId { get; set; }
        public Lot Lot { get; set; } = default!;
    }
}
