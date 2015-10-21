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
            SetPlaceholderText();
        }

        /// <summary>
        /// Set the placeholder text so the user doesn't see a empty screen
        /// </summary>
        private void SetPlaceholderText()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("shakeThreshold"))
            {
                tbThreshold.PlaceholderText = ApplicationData.Current.LocalSettings.Values["shakeThreshold"].ToString();
            }
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("intervalWhenAsleep"))
            {
                tbSleepTime.PlaceholderText = ApplicationData.Current.LocalSettings.Values["intervalWhenAsleep"].ToString();
            }
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("intervalWhenAwake"))
            {
                tbMeasurementInterval.PlaceholderText = ApplicationData.Current.LocalSettings.Values["intervalWhenAwake"].ToString();
            }
        }

        /// <summary>
        /// Apply the advanced settings to local settings value.
        /// If a settings box is empty it doesn't add it to the local settings value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbThreshold.Text))
                ApplicationData.Current.LocalSettings.Values["shakeThreshold"] = tbThreshold.Text;
            if (!string.IsNullOrEmpty(tbSleepTime.Text))
                ApplicationData.Current.LocalSettings.Values["intervalWhenAsleep"] = tbSleepTime.Text;
            if (!string.IsNullOrEmpty(tbMeasurementInterval.Text))
                ApplicationData.Current.LocalSettings.Values["intervalWhenAwake"] = tbMeasurementInterval.Text;
        }

        /// <summary>
        /// Reset the local settings value to default.
        /// For a user who may mess up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["shakeThreshold"] = BackgroundTask.BackgroundTaskConfiguration.SHAKETHRESHOLD.ToString();
            ApplicationData.Current.LocalSettings.Values["intervalWhenAsleep"] = BackgroundTask.BackgroundTaskConfiguration.INTERVAL_WHEN_ASLEEP.ToString();
            ApplicationData.Current.LocalSettings.Values["intervalWhenAwake"] = BackgroundTask.BackgroundTaskConfiguration.INTERVAL_WHEN_AWAKE.ToString();
        }


    }
}
