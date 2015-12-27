using System.ComponentModel.Composition;
using System.Waf.UnitTesting.Mocks;
using Waf.MusicManager.Applications.Views;

namespace Test.MusicManager.Applications.Views
{
    [Export(typeof(IManagerView))]
    public class MockManagerView : MockView, IManagerView
    {
    }
}
