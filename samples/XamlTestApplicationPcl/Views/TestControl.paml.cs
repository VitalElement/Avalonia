using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace XamlTestApplication.Views
{
    public class TestControl : UserControl
    {
        public TestControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
