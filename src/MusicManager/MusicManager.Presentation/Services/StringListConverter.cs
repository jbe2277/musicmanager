using System.Globalization;

namespace Waf.MusicManager.Presentation.Services;

public static class StringListConverter
{
    public static string ToString(IEnumerable<string>? list, string? separator = null) => string.Join(GetSeparator(separator), list ?? []);

    public static IReadOnlyList<string> FromString(string text, string? separator = null)
    {
        return (text ?? "").Split([GetSeparator(separator).Trim(' ')], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim(' ')).Where(x => !string.IsNullOrEmpty(x)).ToArray();
    }

    private static string GetSeparator(string? separator) => !string.IsNullOrEmpty(separator) ? separator : CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ";
}
