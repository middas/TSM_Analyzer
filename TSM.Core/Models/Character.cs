namespace TSM.Core.Models
{
    public enum Faction
    {
        Alliance,
        Horde
    }

    public class Character
    {
        public Character(string name, Faction faction, string realm)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Faction = faction;
            Realm = realm ?? throw new ArgumentNullException(nameof(realm));
        }

        public Dictionary<string, int> AuctionItems { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> BagItems { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> BankItems { get; set; } = new Dictionary<string, int>();

        public string Class { get; set; }

        public Faction Faction { get; set; }

        public DateTimeOffset GoldLogLastUpdate { get; set; }

        public Dictionary<string, int> MailItems { get; set; } = new Dictionary<string, int>();

        public Money Money { get; set; }

        public string Name { get; set; }

        public Dictionary<string, int> ReagentItems { get; set; } = new Dictionary<string, int>();

        public string Realm { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is Character c)
            {
                return c.Name == Name && c.Realm == Realm && c.Faction == Faction;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Realm.GetHashCode() ^ Faction.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} - {Faction} - {Realm}";
        }
    }
}