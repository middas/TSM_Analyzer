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
        public Money Money => Price;

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

        [Ignore]
        public Money Total => Money * Quantity;

        public override string ToString()
        {
            return $"{Character} - {ItemID} - {Money}";
        }

        public override bool Equals(object? obj)
        {
            if(ReferenceEquals(this, obj)) return true;
            if(obj is CharacterSaleModel csm)
            {
                return csm.Character == Character && csm.ItemID == ItemID && csm.Price == Price && 
                    csm.Quantity == Quantity && csm.Source == Source && csm.StackSize == StackSize && 
                    csm.Time == Time;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Character.GetHashCode() ^ ItemID.GetHashCode() ^ Price.GetHashCode() ^ 
                Quantity.GetHashCode() ^ Source.GetHashCode() ^ StackSize.GetHashCode() ^ 
                Time.GetHashCode();
        }
    }
}