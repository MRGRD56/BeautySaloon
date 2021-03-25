namespace BeautySaloon.Library
{
    public static class Text
    {
        /// <summary>
        /// Определяет, соответствует ли строка <paramref name="original"/> поисковому запросу <paramref name="query"/>.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static bool IsMatch(this string original, string query)
        {
            return string.IsNullOrWhiteSpace(query) ||
                original.ToLower().Contains(query.ToLower().Trim());
        }
    }
}
