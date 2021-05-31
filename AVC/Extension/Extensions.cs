using System.Linq;

namespace AVC.Extensions.Extensions
{
    public static class Extensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> query, int page, int limit)
        {
            if (page != 0 && limit != 0)
            {
                query = query.Skip(limit * (page - 1)).Take(limit);
            }

            return query;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            bool isNullOrEmpty = false;
            if (str == null)
            {
                isNullOrEmpty = true;
            }
            else if (str.Equals(""))
            {
                isNullOrEmpty = true;
            }
            return isNullOrEmpty;
        }
    }
}
