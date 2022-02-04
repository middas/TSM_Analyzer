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
    }
}