using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ShakeToStart.Model
{
    /// <summary>
    /// AppConfiguration in case there is no data use this.
    /// </summary>
    class AppConfiguration
    {
        public const string X_URI = @"zune";
        public const string Y_URI = @"ms-settings-wifi";
        public const string Z_URI = @"http://facebook.com";

        public const string BACKGROUND_TASKNAME = "ShakeToStart BackgroundTask";
        public const string BACKGROUND_TAKS_ENTRYPOINT = "BackgroundTask.BackgroundTask";

        public static List<UriItem> defaultUris = new List<UriItem>()
        {
            new UriItem() { name = "Bing", uri = new Uri("http://Bing.com"), symbol = Symbol.Globe },
            new UriItem() { name = "Facebook", uri = new Uri("http://Facebook.com"), symbol = Symbol.PhoneBook },
            new UriItem() { name = "Phone Settings", uri = new Uri("ms-settings:"), symbol = Symbol.Setting},
            new UriItem() { name = "Zune", uri = new Uri("Zune:"), symbol = Symbol.Globe },
            new UriItem() { name = "Wifi", uri = new Uri("ms-settings-wifi"), symbol = Symbol.Globe },
            new UriItem() { name = "Google", uri = new Uri("http://Google.com"), symbol = Symbol.Globe }
        };
    }
}
