using TSM.Core.Models;

namespace TSM.Core.Extensions
{
    public static class MoneyExtensions
    {
        public static Money Sum(this IEnumerable<Money> money, Func<Money, Money> selector)
        {
            return Sum(Enumerable.Select(money, selector));
        }

        public static Money Sum(this IEnumerable<Money> money)
        {
            Money result = 0;
            foreach (Money moneyItem in money)
            {
                result += moneyItem;
            }

            return result;
        }
    }
}