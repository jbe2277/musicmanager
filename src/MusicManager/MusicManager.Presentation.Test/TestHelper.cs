using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.MusicManager.Presentation;

[TestClass]
public static class TestHelper
{
    private static readonly HashSet<string> tempFiles = new();
        
    public static string GetTempFileName(string? extension = null)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), "tmp" + Path.GetRandomFileName());
        if (!string.IsNullOrEmpty(extension)) tempFile += extension;
        tempFiles.Add(tempFile);
        return tempFile;
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        foreach (var x in tempFiles.Where(y => File.Exists(y))) { File.Delete(x); }
    }
}
