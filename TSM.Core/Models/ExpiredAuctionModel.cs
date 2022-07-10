using CsvHelper.Configuration.Attributes;

namespace TSM.Core.Models
{
    public class ExpiredAuctionModel
    {
        //itemString,stackSize,quantity,player,time
        [Name("itemString")]
        public string ItemId { get; set; }

        [Name("player")]
        public string Player { get; set; }

        [Name("quantity")]
        public int Quantity { get; set; }

        [Name("stackSize")]
        public int StackSize { get; set; }

        [Ignore]
        public DateTimeOffset Time => DateTimeOffset.FromUnixTimeSeconds(TimeEpoch);

        [Name("time")]
        public long TimeEpoch { get; set; }

        [Ignore]
        public int Hash { get; set; }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is ExpiredAuctionModel eam)
            {
                return eam.Hash == Hash;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ Player.GetHashCode() ^ Quantity.GetHashCode() ^ StackSize.GetHashCode() ^ TimeEpoch.GetHashCode();
        }
    }
}