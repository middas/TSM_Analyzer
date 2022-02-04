using CsvHelper.Configuration.Attributes;

namespace TSM.Core.Models
{
    public class AuctionBuyModel
    {
        //itemString,stackSize,quantity,price,otherPlayer,player,time,source
        [Name("itemString")]
        public string ItemId { get; set; }

        [Ignore]
        public Money Money => new(Price);

        [Name("otherPlayer")]
        public string OtherPlayer { get; set; }

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
    }
}