using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Waf.UnitTesting;

namespace Test.MusicManager.Domain;

[TestClass]
public abstract class DomainTest
{
    [TestInitialize]
    public void Initialize()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
        OnInitialize();
    }

    [TestCleanup]
    public void Cleanup() => OnCleanup();

    protected virtual void OnInitialize() { }

    protected virtual void OnCleanup() { }

    public static void AssertHaveEqualPropertyValues<T>(T expected, T actual, Func<PropertyInfo, bool>? predicate = null)
    {
        var objectType = typeof(T);
        var properties = objectType.GetProperties();
        predicate ??= (p => true);

        foreach (var property in properties.Where(predicate))
        {
            if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                var expectedCollection = ((IEnumerable)property.GetValue(expected)!).Cast<object>();
                var actualCollection = ((IEnumerable)property.GetValue(actual)!).Cast<object>();
                AssertHelper.SequenceEqual(expectedCollection, actualCollection);
            }
            else
            {
                Assert.AreEqual(property.GetValue(expected), property.GetValue(actual), "Property name: " + property.Name);
            }
        }
    }
}
