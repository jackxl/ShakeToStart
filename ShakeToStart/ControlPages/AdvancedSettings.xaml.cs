using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ShakeToStart.Model;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ShakeToStart.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdvancedSettings : Page
    {
        public AdvancedSettings()
        {
            this.InitializeComponent();
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbThreshold.Text))
                ApplicationData.Current.LocalSettings.Values["shakeThreshold"] = tbThreshold.Text;
            if (!string.IsNullOrEmpty(tbSleepTime.Text))
                ApplicationData.Current.LocalSettings.Values["intervalWhenAsleep"] = tbSleepTime.Text;
            if (!string.IsNullOrEmpty(tbMeasurementInterval.Text))
                ApplicationData.Current.LocalSettings.Values["intervalWhenAwake"] = tbMeasurementInterval.Text;
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["shakeThreshold"] = BackgroundTask.BackgroundTaskConfiguration.SHAKETHRESHOLD.ToString();
            ApplicationData.Current.LocalSettings.Values["intervalWhenAsleep"] = BackgroundTask.BackgroundTaskConfiguration.INTERVAL_WHEN_ASLEEP.ToString();
            ApplicationData.Current.LocalSettings.Values["intervalWhenAwake"] = BackgroundTask.BackgroundTaskConfiguration.INTERVAL_WHEN_AWAKE.ToString();
        }


    }
}
