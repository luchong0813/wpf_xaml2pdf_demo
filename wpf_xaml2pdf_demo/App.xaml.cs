using wpf_xaml2pdf_demo.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace wpf_xaml2pdf_demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
