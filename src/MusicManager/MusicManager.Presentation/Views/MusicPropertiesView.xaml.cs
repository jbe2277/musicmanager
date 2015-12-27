using System.ComponentModel.Composition;
using System.Windows.Controls;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(IMusicPropertiesView))]
    public partial class MusicPropertiesView : UserControl, IMusicPropertiesView
    {
        public MusicPropertiesView()
        {
            InitializeComponent();
        }
    }
}
