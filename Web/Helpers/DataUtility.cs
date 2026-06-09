namespace Web.Helpers
{
    public static class DataUtility
    {
        public static string GetInitials(string? name, string fallback)
        {
            if (string.IsNullOrWhiteSpace(name)) return fallback;

            var parts = name
                .Split(new char[] { '_', '-', '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .Take(2)
                .ToArray();

            if (parts.Length == 0) return fallback;

            return string.Concat(parts.Select(part => char.ToUpperInvariant(part[0])));
        }
    }
}
