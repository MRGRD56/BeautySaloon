using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class Text
    {
        public static bool IsMatch(this string original, string query)
        {
            return string.IsNullOrWhiteSpace(query) ||
                original.ToLower().Contains(query.ToLower().Trim());
        }
    }
}
