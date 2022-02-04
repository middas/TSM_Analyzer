namespace TSM.Core.Models
{
    public class CharacterSaleModel
    {
        public Character Character { get; set; }

        public int Count { get; set; }

        public string ItemID { get; set; }

        public string ItemName { get; set; }

        public Money SoldValue { get; set; }

        public DateTimeOffset TimeOfSale { get; set; }

        public override string ToString()
        {
            return $"{Character.Name} - {ItemName} - {SoldValue}";
        }
    }
}