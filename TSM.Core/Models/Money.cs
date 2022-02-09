﻿namespace TSM.Core.Models
{
    public class Money
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

        public static Money operator +(Money a, Money b) => new(a.TotalCopper + b.totalCopper);

        public override string ToString()
        {
            return $"{$"{Gold:n0}"}g{Silver}s{Copper}c";
        }
    }
}