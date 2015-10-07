using ShakeToStart.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ShakeToStart.ControlPages
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        private ObservableCollection<UriItem> uriSelection = new ObservableCollection<UriItem>();

        public Home()
        {
            uriSelection = App.uriItemsAvailable;
            this.InitializeComponent();
        }

        private void btnEnable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDisable_Click(object sender, RoutedEventArgs e)
        {
            uriSelection.Add(new UriItem() { name = "Google", uri = new Uri("http://Google.com"), symbol = Symbol.Globe });
        }

        private void cbUri_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }
    }
}
