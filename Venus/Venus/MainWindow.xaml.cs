using AdonisUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Venus.ViewModels;

namespace Venus
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisWindow
    {
        private MainWindow window { set; get; }
        private MainViewModel model { set; get; }

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            window = this;

            model = new MainViewModel(window, "Core");
        }

        private async void AdonisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await model.EventLoad();
        }
    }
}
