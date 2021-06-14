using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = MD5.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(this string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public class AuthorizeRolesAttribute : AuthorizeAttribute
        {
            public AuthorizeRolesAttribute(params string[] roles) : base()
            {
                Roles = string.Join(",", roles);
            }
        }



        
    }
}
