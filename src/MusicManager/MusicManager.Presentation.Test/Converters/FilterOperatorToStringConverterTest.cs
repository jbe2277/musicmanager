using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters
{
    [TestClass]
    public class FilterOperatorToStringConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            var converter = new FilterOperatorToStringConverter();
            
            Assert.IsNull(converter.Convert(null, null, null, null));
            Assert.AreEqual("", converter.Convert(FilterOperator.Ignore, null, null, null));
            Assert.AreEqual("<=", converter.Convert(FilterOperator.LessThanOrEqual, null, null, null));
            Assert.AreEqual(">=", converter.Convert(FilterOperator.GreaterThanOrEqual, null, null, null));
        }
    }
}
