using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
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
//    private ObservableCollection<String> UriCollection 


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        public static String TASKNAME = "ShakeToStart BackgroundTask";
        public static String TASKENTRYPOINT = "BackgroundTask.BackgroundTask";

        private Accelerometer Accelerometer;
        private DeviceUseTrigger _deviceUseTrigger;

        // Used to register the device use background task
        private BackgroundTaskRegistration _deviceUseBackgroundTaskRegistration;

        public Home()
        {
            this.InitializeComponent();

            Accelerometer = Accelerometer.GetDefault();
            if (null != Accelerometer)
            {
                // Save trigger so that we may start the background task later.
                // Only one instance of the trigger can exist at a time. Since the trigger does not implement
                // IDisposable, it may still be in memory when a new trigger is created.
                _deviceUseTrigger = new DeviceUseTrigger();
            }
            else
            {
                this.NotifyUser("No accelerometer found", NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // If the background task is active, start the refresh timer and activate the "Disable" button.
            // The "IsBackgroundTaskActive" state is set by the background task.
            bool isBackgroundTaskActive =
                ApplicationData.Current.LocalSettings.Values.ContainsKey("IsBackgroundTaskActive") &&
                (bool)ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"];

            btnEnable.IsEnabled = !isBackgroundTaskActive;
            btnDisable.IsEnabled = isBackgroundTaskActive;
        }

        private async void btnEnable_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["shakeThreshold"] = 3.0;
            ApplicationData.Current.LocalSettings.Values["xUri"] = @"zune:";
            ApplicationData.Current.LocalSettings.Values["yUri"] = @"ms-settings-wifi:";
            ApplicationData.Current.LocalSettings.Values["zUri"] = @"http://facebook.com";

            ApplicationData.Current.LocalSettings.Values["intervalWhenAwake"] = "100";
            ApplicationData.Current.LocalSettings.Values["intervalWhenAsleep"] = "3000";

            if (null != Accelerometer)
            {
                // Make sure this app is allowed to run background tasks.
                // RequestAccessAsync must be called on the UI thread.
                BackgroundAccessStatus accessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                if ((BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity == accessStatus) ||
                    (BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity == accessStatus))
                {
                    if (await StartSensorBackgroundTaskAsync(Accelerometer.DeviceId))
                    {
                        btnEnable.IsEnabled = false;
                        btnDisable.IsEnabled = true;
                    }
                }
                else
                {
                    NotifyUser("Background tasks may be disabled for this app", NotifyType.ErrorMessage);
                }
            }
            else
            {
                NotifyUser("No accelerometer found", NotifyType.StatusMessage);
            }
        }

        private void btnDisable_Click(object sender, RoutedEventArgs e)
        {
            btnEnable.IsEnabled = true;
            btnDisable.IsEnabled = false;

            if (null != _deviceUseBackgroundTaskRegistration)
            {
                // Cancel and unregister the background task from the current app session.
                _deviceUseBackgroundTaskRegistration.Unregister(true);
                _deviceUseBackgroundTaskRegistration = null;
            }
            else
            {
                // Cancel and unregister the background task from the previous app session.
                FindAndCancelExistingBackgroundTask();
            }

            NotifyUser("Background task was canceled", NotifyType.StatusMessage);
        }

        /// <summary>
        /// Starts the sensor background task.
        /// </summary>
        /// <param name="deviceId">Device Id for the sensor to be used by the task.</param>
        /// <param name="e"></param>
        /// <returns>True if the task is started successfully.</returns>
        private async Task<bool> StartSensorBackgroundTaskAsync(String deviceId)
        {
            bool started = false;

            // Make sure only 1 task is running.
            FindAndCancelExistingBackgroundTask();

            // Register the background task.
            var backgroundTaskBuilder = new BackgroundTaskBuilder()
            {
                Name = TASKNAME,
                TaskEntryPoint = TASKENTRYPOINT
            };

            backgroundTaskBuilder.SetTrigger(_deviceUseTrigger);
            _deviceUseBackgroundTaskRegistration = backgroundTaskBuilder.Register();

            // Make sure we're notified when the task completes or if there is an update.
            _deviceUseBackgroundTaskRegistration.Completed += new BackgroundTaskCompletedEventHandler(OnBackgroundTaskCompleted);

            try
            {
                // Request a DeviceUse task to use the accelerometer.
                DeviceTriggerResult deviceTriggerResult = await _deviceUseTrigger.RequestAsync(deviceId);

                switch (deviceTriggerResult)
                {
                    case DeviceTriggerResult.Allowed:
                        NotifyUser("Background task started", NotifyType.StatusMessage);
                        started = true;
                        break;

                    case DeviceTriggerResult.LowBattery:
                        NotifyUser("Insufficient battery to run the background task", NotifyType.ErrorMessage);
                        break;

                    case DeviceTriggerResult.DeniedBySystem:
                        // The system can deny a task request if the system-wide DeviceUse task limit is reached.
                        NotifyUser("The system has denied the background task request", NotifyType.ErrorMessage);
                        break;

                    default:
                        NotifyUser("Could not start the background task: " + deviceTriggerResult, NotifyType.ErrorMessage);
                        break;
                }
            }
            catch (InvalidOperationException)
            {
                // If toggling quickly between 'Disable' and 'Enable', the previous task
                // could still be in the process of cleaning up.
                NotifyUser("A previous background task is still running, please wait for it to exit", NotifyType.ErrorMessage);
                FindAndCancelExistingBackgroundTask();
            }
            catch(Exception e)
            {
                NotifyUser("ex", NotifyType.StatusMessage);
                FindAndCancelExistingBackgroundTask();
            }

            return started;
        }


        /// <summary>
        /// Finds a previously registered background task for this scenario and cancels it (if present)
        /// </summary>
        private void FindAndCancelExistingBackgroundTask()
        {
            foreach (var backgroundTask in BackgroundTaskRegistration.AllTasks.Values)
            {
                if (backgroundTask.Name == TASKNAME)
                {
                    ((BackgroundTaskRegistration)backgroundTask).Unregister(true);
                    break;
                }
            }
        }

        /// <summary>
        /// This is the background task completion handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnBackgroundTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            // Dispatch to the UI thread to display the output.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // An exception may be thrown if an error occurs in the background task.
                try
                {
                    e.CheckResult();
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TaskCancelationReason"))
                    {
                        string cancelationReason = (string)ApplicationData.Current.LocalSettings.Values["TaskCancelationReason"];
                        NotifyUser("Background task was stopped, reason: " + cancelationReason, NotifyType.StatusMessage);
                    }
                }
                catch (Exception ex)
                {
                    NotifyUser("Exception in background task: " + ex.Message, NotifyType.ErrorMessage);
                }
            });

            // Unregister the background task and let the remaining task finish until completion.
            if (null != _deviceUseBackgroundTaskRegistration)
            {
                _deviceUseBackgroundTaskRegistration.Unregister(false);
                _deviceUseBackgroundTaskRegistration = null;
            }
        }

        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };

        public void NotifyUser(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }
            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
