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

        //public override string ToString()
        //{
        //    return $"{ItemId} - {Time}";
        //}
    }
}