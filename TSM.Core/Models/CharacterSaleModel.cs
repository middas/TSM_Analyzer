using CsvHelper.Configuration.Attributes;

namespace TSM.Core.Models
{
    public class CharacterSaleModel
    {
        [Name("player")]
        public string Character { get; set; }

        [Name("itemString")]
        public string ItemID { get; set; }

        [Ignore]
        public Money Money => new(Price);

        [Name("price")]
        public long Price { get; set; }

        [Name("quantity")]
        public int Quantity { get; set; }

        [Name("source")]
        public string Source { get; set; }

        [Name("stackSize")]
        public int StackSize { get; set; }

        [Name("time")]
        public long Time { get; set; }

        [Ignore]
        public DateTimeOffset TimeOfSale => DateTimeOffset.FromUnixTimeSeconds(Time);

        public override string ToString()
        {
            return $"{Character} - {ItemID} - {Money}";
        }
    }
}