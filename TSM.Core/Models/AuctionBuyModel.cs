using CsvHelper.Configuration.Attributes;

namespace TSM.Core.Models
{
    public class AuctionBuyModel
    {
        //itemString,stackSize,quantity,price,otherPlayer,player,time,source
        [Name("itemString")]
        public string ItemId { get; set; }

        [Ignore]
        public Money Money => Price;

        [Name("otherPlayer")]
        public string OtherPlayer { get; set; }

        [Name("player")]
        public string Player { get; set; }

        [Name("price")]
        public long Price { get; set; }

        [Name("quantity")]
        public int Quantity { get; set; }

        [Name("source")]
        public string Source { get; set; }

        [Name("stackSize")]
        public int StackSize { get; set; }

        [Ignore]
        public DateTimeOffset Time => DateTimeOffset.FromUnixTimeSeconds(TimeEpoch);

        [Name("time")]
        public long TimeEpoch { get; set; }

        [Ignore]
        public Money Total => Money * Quantity;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is AuctionBuyModel abm)
            {
                return abm.ItemId == ItemId && abm.StackSize == StackSize && abm.Price == Price &&
                    abm.Quantity == Quantity && abm.OtherPlayer == OtherPlayer && abm.Time == Time
                    && abm.Player == Player && abm.Source == Source;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ StackSize.GetHashCode() ^ OtherPlayer.GetHashCode() ^ Time.GetHashCode() ^ Player.GetHashCode()
                ^ Price.GetHashCode() ^ Quantity.GetHashCode() ^ Source.GetHashCode();
        }
    }
}