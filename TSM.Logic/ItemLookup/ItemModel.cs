using System.Xml.Serialization;

namespace TSM.Logic.ItemLookup
{
    [XmlRoot("wowhead")]
    public class ItemModel
    {
        [XmlElement("item")]
        public Item Item { get; set; }
    }

    public class Item
    {
        [XmlAttribute("id")]
        public string ItemID { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }
    }
}