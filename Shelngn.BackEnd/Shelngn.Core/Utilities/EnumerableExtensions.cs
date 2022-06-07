namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static bool All<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            int i = 0;
            foreach (var item in source)
            {
                if (!predicate(item, i++))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
