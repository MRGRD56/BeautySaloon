using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BeautySaloon.Library
{
    public static class Validation
    {
        public static bool IsValidName(string name) => Regex.IsMatch(name, @"^[\wа-яё\-\s]{0,50}$", RegexOptions.IgnoreCase);

        public static bool IsValidEmail(string email) => Regex.IsMatch(email, @"^[^@\s]+@[^@\.\s]+\.[^@\.\s]+$");

        public static bool IsValidPhone(string phone) => Regex.IsMatch(phone, @"^[\d\+\-\(\)\s]*$");
    }
}
