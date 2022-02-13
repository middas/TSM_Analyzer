namespace TSM.Core.Models
{
    public class Money : IComparable<Money>, IComparable
    {
        private readonly long totalCopper;

        public Money(long totalCopper)
        {
            this.totalCopper = totalCopper;

            Gold = (int)(totalCopper / 10000L);
            Silver = (int)((totalCopper - (Gold * 10000L)) / 100L);
            Copper = (int)(totalCopper - (Gold * 10000L) - (Silver * 100));
        }

        public int Copper { get; set; } = 0;

        public int Gold { get; set; } = 0;

        public int Silver { get; set; } = 0;

        public long TotalCopper => totalCopper;

        public static Money operator -(Money a, Money b) => new(a.TotalCopper - b.TotalCopper);

        public static Money operator -(Money a) => new(-a.totalCopper);

        public static Money operator +(Money a, Money b) => new(a.TotalCopper + b.totalCopper);

        public int CompareTo(Money? other)
        {
            if (other == null) return 0;
            return TotalCopper.CompareTo(other.TotalCopper);
        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 0;
            if (obj is Money m) return CompareTo(m);
            return 0;
        }

        public override string ToString()
        {
            return $"{$"{Gold:n0}"}g{Silver}s{Copper}c";
        }
    }
}