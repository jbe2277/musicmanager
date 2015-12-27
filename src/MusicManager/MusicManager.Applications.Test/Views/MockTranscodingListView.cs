using System.ComponentModel.Composition;
using System.Waf.UnitTesting.Mocks;
using Waf.MusicManager.Applications.Views;

namespace Test.MusicManager.Applications.Views
{
    [Export(typeof(ITranscodingListView)), Export]
    public class MockTranscodingListView : MockView, ITranscodingListView
    {
    }
}
