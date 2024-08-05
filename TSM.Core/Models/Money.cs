namespace TSM.Core.Models
{
    public class Money : IComparable<Money>, IComparable
    {
        public Money(long totalCopper)
        {
            this.TotalCopper = totalCopper;

            Gold = (int)(totalCopper / 10000L);
            Silver = (int)((totalCopper - (Gold * 10000L)) / 100L);
            Copper = (int)(totalCopper - (Gold * 10000L) - (Silver * 100));
        }

        public int Copper { get; set; } = 0;

        public int Gold { get; set; } = 0;

        public int Silver { get; set; } = 0;

        public long TotalCopper { get; }

        public static Money operator -(Money a, Money b)
        {
            return a.TotalCopper - b.TotalCopper;
        }

        public static Money operator -(Money a)
        {
            return -a.TotalCopper;
        }

        public static Money operator +(Money a, Money b)
        {
            return a.TotalCopper + b.TotalCopper;
        }

        public static Money operator *(Money a, int b)
        {
            return a.TotalCopper * b;
        }

        public static implicit operator Money(long copper)
        {
            return new(copper);
        }

        public static implicit operator Money(double copper)
        {
            return new((long)copper);
        }

        public int CompareTo(Money? other)
        {
            return other == null ? 0 : TotalCopper.CompareTo(other.TotalCopper);
        }

        public int CompareTo(object? obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj is Money m ? CompareTo(m) : 0;
        }

        public override string ToString()
        {
            return $"{$"{(TotalCopper < 0 ? "-" : string.Empty)}{Math.Abs(Gold):n0}"}g{Math.Abs(Silver)}s{Math.Abs(Copper)}c";
        }
    }
}