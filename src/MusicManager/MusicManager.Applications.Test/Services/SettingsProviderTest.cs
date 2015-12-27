using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.UnitTesting;
using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services
{
    [TestClass]
    public class SettingsProviderTest : ApplicationsTest
    {
        private string testSettingsPath;
        

        protected override void OnInitialize()
        {
            base.OnInitialize();
            testSettingsPath = Environment.CurrentDirectory + @"\Files\Settings\MockSettings.xml";
        }
        
        protected override void OnCleanup()
        {
            try
            {
                File.Delete(testSettingsPath);
                Directory.Delete(Path.GetDirectoryName(testSettingsPath));
            }
            catch (DirectoryNotFoundException)
            { }
            catch (FileNotFoundException)
            { }
            base.OnCleanup();
        }
        

        [TestMethod]
        public void BasicSaveAndLoadTest()
        {
            var provider = new SettingsProvider();

            var defaultSettings = provider.LoadSettings<TestSettings>(testSettingsPath);
            Assert.AreEqual("Default", defaultSettings.Test);

            var settings1 = new TestSettings() { Test = "Test1" };
            provider.SaveSettings(testSettingsPath, settings1);

            var settings2 = provider.LoadSettings<TestSettings>(testSettingsPath);

            Assert.AreNotEqual(settings1, settings2);
            Assert.AreEqual(settings1.Test, settings2.Test);
        }

        [TestMethod]
        public void MethodArgumentsTest()
        {
            var provider = new SettingsProvider();
            var settings = new TestSettings() { Test = "Test1" };

            AssertHelper.ExpectedException<ArgumentNullException>(() => provider.SaveSettings(testSettingsPath, null));
            AssertHelper.ExpectedException<ArgumentException>(() => provider.SaveSettings(null, settings));
            AssertHelper.ExpectedException<ArgumentException>(() => provider.SaveSettings("MockSettings.xml", settings));

            AssertHelper.ExpectedException<ArgumentException>(() => provider.LoadSettings<TestSettings>(null));
            AssertHelper.ExpectedException<ArgumentException>(() => provider.LoadSettings<TestSettings>("MockSettings.xml"));
        }


        [Serializable]
        private class TestSettings
        {
            public TestSettings()
            {
                Test = "Default";
            }
            
            public string Test { get; set; }
        }
    }
}
