using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.UnitTesting;
using Waf.MusicManager.Applications.DataModels;

namespace Test.MusicManager.Applications.DataModels
{
    [TestClass]
    public class SearchFilterDataModelTest : ApplicationsTest
    {
        [TestMethod]
        public void YearFilterTest()
        {
            var dataModel = new SearchFilterDataModel();

            Assert.IsNull(dataModel.FromYearFilter);
            Assert.IsNull(dataModel.ToYearFilter);

            AssertHelper.PropertyChangedEvent(dataModel, x => x.FromYearFilter, () => dataModel.FromYearFilter = "1990");
            Assert.AreEqual("1990", dataModel.FromYearFilter);
            AssertHelper.PropertyChangedEvent(dataModel, x => x.ToYearFilter, () => dataModel.ToYearFilter = "2000");
            Assert.AreEqual("2000", dataModel.ToYearFilter);

            dataModel.FromYearFilter = "";
            dataModel.ToYearFilter = "";
            Assert.IsNull(dataModel.FromYearFilter);
            Assert.IsNull(dataModel.ToYearFilter);
        }

        [TestMethod]
        public void DisplaySearchFilterTest()
        {
            var dataModel = new SearchFilterDataModel();

            Assert.AreEqual("Search", dataModel.DisplaySearchFilter);

            AssertHelper.PropertyChangedEvent(dataModel, x => x.DisplaySearchFilter, () => dataModel.ArtistFilter = "Culture");
            Assert.AreEqual("A filter is set", dataModel.DisplaySearchFilter);

            AssertHelper.PropertyChangedEvent(dataModel, x => x.DisplaySearchFilter, () => dataModel.UserSearchFilter = "01 Culture");
            Assert.AreEqual("01 Culture; A filter is set", dataModel.DisplaySearchFilter);

            AssertHelper.PropertyChangedEvent(dataModel, x => x.DisplaySearchFilter, () => dataModel.ArtistFilter = "");
            Assert.AreEqual("01 Culture", dataModel.DisplaySearchFilter);

            dataModel.Clear();
            Assert.AreEqual("Search", dataModel.DisplaySearchFilter);
        }

        [TestMethod]
        public void ApplicationSearchFilterTest()
        {
            var dataModel = new SearchFilterDataModel();

            AssertHelper.PropertyChangedEvent(dataModel, x => x.ApplicationSearchFilter, () => dataModel.ArtistFilter = "Culture");
            Assert.AreEqual("System.Music.Artist:~=\"Culture\"", dataModel.ApplicationSearchFilter);

            AssertHelper.PropertyChangedEvent(dataModel, x => x.ApplicationSearchFilter, () => dataModel.TitleFilter = "Mr");
            Assert.AreEqual("System.Music.Artist:~=\"Culture\" AND System.Title:~=\"Mr\"", dataModel.ApplicationSearchFilter);

            dataModel.Clear();
            Assert.AreEqual("", dataModel.ApplicationSearchFilter);

            AssertHelper.PropertyChangedEvent(dataModel, x => x.ApplicationSearchFilter, () => dataModel.GenreFilter = new[] { "Pop", "Rock" });
            Assert.AreEqual("System.Music.Genre:\"Pop\" OR System.Music.Genre:\"Rock\"", dataModel.ApplicationSearchFilter);

            dataModel.Clear();

            AssertHelper.PropertyChangedEvent(dataModel, x => x.ApplicationSearchFilter, () => dataModel.RatingFilterOperator = FilterOperator.GreaterThanOrEqual);
            AssertHelper.PropertyChangedEvent(dataModel, x => x.ApplicationSearchFilter, () => dataModel.RatingFilter = 75);
            Assert.AreEqual("System.Rating:>=75", dataModel.ApplicationSearchFilter);
            dataModel.RatingFilterOperator = FilterOperator.Ignore;
            Assert.AreEqual("", dataModel.ApplicationSearchFilter);
            dataModel.RatingFilterOperator = FilterOperator.LessThanOrEqual;
            Assert.AreEqual("System.Rating:<=75", dataModel.ApplicationSearchFilter);

            dataModel.Clear();

            Assert.AreEqual(FilterOperator.Ignore, dataModel.RatingFilterOperator);
            AssertHelper.PropertyChangedEvent(dataModel, x => x.RatingFilterOperator, () => dataModel.RatingFilter = 75);
            Assert.AreEqual(FilterOperator.GreaterThanOrEqual, dataModel.RatingFilterOperator);
            Assert.AreEqual("System.Rating:>=75", dataModel.ApplicationSearchFilter);
            
            dataModel.Clear();

            AssertHelper.PropertyChangedEvent(dataModel, x => x.ApplicationSearchFilter, () => dataModel.FromYearFilter = "1990");
            Assert.AreEqual("System.Media.Year:>=1990", dataModel.ApplicationSearchFilter);
            AssertHelper.PropertyChangedEvent(dataModel, x => x.ApplicationSearchFilter, () => dataModel.ToYearFilter = "2000");
            Assert.AreEqual("System.Media.Year:>=1990 <=2000", dataModel.ApplicationSearchFilter);
            dataModel.FromYearFilter = null;
            Assert.AreEqual("System.Media.Year:<=2000", dataModel.ApplicationSearchFilter);
        }
    }
}
