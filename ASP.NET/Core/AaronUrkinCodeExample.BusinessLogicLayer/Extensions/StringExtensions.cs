using System;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Extensions
{
    public static class StringExtensions
    {
        public static bool Truncated(this string source, int threshold, out string output, string suffix = null)
        {
            output = null;

            if (!string.IsNullOrEmpty(source) && source.Length > threshold)
            {
                int nextSpace = source.LastIndexOf(" ", threshold, StringComparison.Ordinal);

                output = $"{source.Substring(0, (nextSpace > 0) ? nextSpace : threshold).Trim()}{suffix}";
            }

            return !string.IsNullOrEmpty(output);
        }
    }
}
