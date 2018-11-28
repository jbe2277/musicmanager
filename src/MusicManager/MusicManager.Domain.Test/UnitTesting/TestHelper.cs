using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Waf.UnitTesting;

namespace Test.MusicManager.Domain.UnitTesting
{
    [TestClass]
    public static class TestHelper
    {
        private static HashSet<string> tempFiles = new HashSet<string>();
        
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
        }

        public static string GetTempFileName(string extension = null)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "tmp" + Path.GetRandomFileName());
            if (!string.IsNullOrEmpty(extension)) { tempFile += extension; }
            tempFiles.Add(tempFile);
            return tempFile;
        }

        public static void AssertHaveEqualPropertyValues<T>(T expected, T actual, Func<PropertyInfo, bool> predicate = null)
        {
            var objectType = typeof(T);
            var properties = objectType.GetProperties();
            predicate = predicate ?? (p => true);

            foreach (var property in properties.Where(predicate))
            {
                if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    var expectedCollection = ((IEnumerable)property.GetValue(expected)).Cast<object>();
                    var actualCollection = ((IEnumerable)property.GetValue(actual)).Cast<object>();
                    AssertHelper.SequenceEqual(expectedCollection, actualCollection);
                }
                else
                {
                    Assert.AreEqual(property.GetValue(expected), property.GetValue(actual), "Property name: " + property.Name);
                }
            }
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            foreach (var tempFile in tempFiles)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
