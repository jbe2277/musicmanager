﻿using System.IO;

namespace Waf.MusicManager.Applications.Services;

public static class MusicTitleHelper
{
    public static string GetTitleText(string? fileName, IEnumerable<string>? artists, string? title)
    {
        artists ??= [];
        var result = string.IsNullOrEmpty(title) && !artists.Any() ? Path.GetFileNameWithoutExtension(fileName) : title;
        return result ?? "";
    }
}
