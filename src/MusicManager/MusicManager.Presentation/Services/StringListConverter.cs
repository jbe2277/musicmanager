using System.Globalization;

namespace Waf.MusicManager.Presentation.Services;

public static class StringListConverter
{
    public static string ToString(IEnumerable<string>? list, string? separator = null) => string.Join(GetSeparator(separator), list ?? Array.Empty<string>());

    public static IEnumerable<string> FromString(string text, string? separator = null)
    {
        return (text ?? "").Split(new[] { GetSeparator(separator).Trim(' ') }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim(' ')).Where(x => !string.IsNullOrEmpty(x)).ToArray();
    }

    private static string GetSeparator(string? separator) => !string.IsNullOrEmpty(separator) ? separator : CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ";
}
