using ShakeToStart.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class UriManager : Page
    {
        /// <summary>
        /// Observable collections for databinding.
        /// </summary>
        private ObservableCollection<UriItem> uriSelection = new ObservableCollection<UriItem>();
        private ObservableCollection<SymbolItem> symbolList = new ObservableCollection<SymbolItem>();

        public UriManager()
        {
            //fil the list with the Global Uri items
            uriSelection = App.uriItemsAvailable;
            FillSymbolList();
            this.InitializeComponent();
        }


        //TODO: fix Symbols
        private void FillSymbolList()
        {
            //57600 start of symbolicons 
            //58005 end of symbolicons
            //https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.symbol
            List<SymbolIcon> icons = new List<SymbolIcon>();
            for (int i = 57600; i <= 58005; i++)
            {
                Symbol symb = (Symbol)i;
                icons.Add(new SymbolIcon(symb));
                symbolList.Add(new SymbolItem() { symbol = symb, emptyName = "" });
            }
        }


        //TODO: check for empty and format exceptions
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SymbolItem symbolitem = (SymbolItem)cbSymbols.SelectedItem;
            try
            {
                UriItem item = new UriItem()
                {
                    name = tbName.Text,
                    uri = new Uri(tbUri.Text),
                    symbol = symbolitem.symbol
                };
                uriSelection.Add(item);
                storeUriItemsToStorage();
            }
            catch
            {
                // something is wrong with the uri here.
            }
        }

        private void storeUriItemsToStorage()
        {
            List<String> serializedUriList = new List<String>();
            foreach (UriItem item in uriSelection)
            {
                serializedUriList.Add(item.GetSerializedUriObject());
            }

            ApplicationData.Current.LocalSettings.Values["uriItems"] = serializedUriList.ToArray();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            uriSelection.Remove(e.ClickedItem as UriItem);
        }
    }
}
