using System.ComponentModel.Composition.Primitives;

namespace Waf.MusicManager.Applications;

public static class UnitTestMetadata
{
    public const string Name = "UnitTest";

    public const string Data = "Data";

    public static bool IsContained(ComposablePartDefinition definition, object? value = null)
    {
        return value == null
            ? definition.Metadata.ContainsKey(Name)
            : definition.Metadata.ContainsKey(Name) && definition.Metadata[Name].Equals(value);
    }
}
