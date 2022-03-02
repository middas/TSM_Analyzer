using CsvHelper.Configuration.Attributes;

namespace TSM.Core.Models
{
    public class CancelledAuctionModel
    {
        //itemString,stackSize,quantity,player,time
        [Name("itemString")]
        public string ItemId { get; set; }

        [Name("player")]
        public string PlayerName { get; set; }

        [Name("quantity")]
        public int Quantity { get; set; }

        [Name("stackSize")]
        public int StackSize { get; set; }

        [Ignore]
        public DateTimeOffset Time => DateTimeOffset.FromUnixTimeSeconds(TimeEpoch);

        [Name("time")]
        public long TimeEpoch { get; set; }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is CancelledAuctionModel cam)
            {
                return cam.ItemId == ItemId && cam.StackSize == StackSize && cam.PlayerName == PlayerName && cam.Quantity == Quantity && cam.TimeEpoch == TimeEpoch;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ StackSize.GetHashCode() ^ PlayerName.GetHashCode() ^ Quantity.GetHashCode() ^ TimeEpoch.GetHashCode();
        }
    }
}