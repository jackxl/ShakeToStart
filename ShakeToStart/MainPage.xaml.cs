using ShakeToStart.ControlPages;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ShakeToStart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        /// <summary>
        /// Object required for databinding. Made private so it can't be changed
        /// </summary>
        private ObservableCollection<NavigationItem> _NavigationLinks = new ObservableCollection<NavigationItem>()
        {
            new NavigationItem() {name = "Home", symbol = Symbol.Home, pageFrame = typeof(Home) },
            new NavigationItem() {name = "UriManager", symbol = Symbol.Globe, pageFrame = typeof(UriManager) },
            new NavigationItem() {name = "Advanced Seddings", symbol = Symbol.Setting, pageFrame = typeof(AdvancedSettings)},
        };

        /// <summary>
        /// Allowes access for databinding, used c# 6.0 lampda expression to make it shorter
        /// </summary>
        public ObservableCollection<NavigationItem> navigationLinks => _NavigationLinks;
        public MainPage()
        {
            this.InitializeComponent();
            this.MainFrame.Navigate(typeof(Home));
        }

        #region EVENTS
        /// <summary>
        /// Event for clicking an item in the ListView.
        /// For now it only changes the background color for one specific item.
        /// </summary>
        private void svItemList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as NavigationItem;
            if (item?.pageFrame != null)
            {
                this.MainFrame.Navigate((e.ClickedItem as NavigationItem).pageFrame);
            }
            else
            {
                this.MainFrame.Navigate(typeof(Home));
            }
        }

        private void btHamburger_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }
        #endregion
    }
}
