namespace BeautySaloon.Library
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
