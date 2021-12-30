using System.ComponentModel.Composition;
using System.Waf.UnitTesting.Mocks;
using Waf.MusicManager.Applications.Views;

namespace Test.MusicManager.Applications.Views;

[Export, Export(typeof(IMusicPropertiesView))]
public class MockMusicPropertiesView : MockView, IMusicPropertiesView
{
}
