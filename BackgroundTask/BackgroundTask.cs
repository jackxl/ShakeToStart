// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.Devices.Background;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Storage;

namespace BackgroundTask
{
    public sealed class BackgroundTask : IBackgroundTask, IDisposable
    {
        private Accelerometer Accelerometer;
        private BackgroundTaskDeferral Deferral;
        private ulong SampleCount;

        bool appHasLaunched = false;
        uint awakeTime = (uint) BackgroundTaskConfiguration.INTERVAL_WHEN_AWAKE;
        uint sleepTime = (uint) BackgroundTaskConfiguration.INTERVAL_WHEN_ASLEEP;
        double shakeThreshold = BackgroundTaskConfiguration.SHAKETHRESHOLD;

        double[] measurementsX = new double[5];
        double[] measurementsY = new double[5];
        double[] measurementsZ = new double[5];

        /// <summary> 
        /// Background task entry point.
        /// </summary> 
        /// <param name="taskInstance"></param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Accelerometer = Accelerometer.GetDefault();

            if (null != Accelerometer)
            {
                SampleCount = 0;

                Init();

                // Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
                setReportInterval(false);

                // Subscribe to accelerometer ReadingChanged events.
                Accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);

                //The Accelerometer has a ShakenEvent. This has been tested and found not be working as desired on our dev device (lumia 930)
                //Accelerometer.Shaken += new TypedEventHandler<Accelerometer, AccelerometerShakenEventArgs>(Shaken); //not using

                // Take a deferral that is released when the task is completed.
                Deferral = taskInstance.GetDeferral();

                // Get notified when the task is canceled.
                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

                // Store a setting so that the app knows that the task is running.
                ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"] = true;
            }
        }

        private void Init()
        {

            // setting the interval from the userdata settings
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("intervalWhenAwake"))
            {
                uint val = Convert.ToUInt32(ApplicationData.Current.LocalSettings.Values["intervalWhenAwake"]);
                if (val > 10)
                    awakeTime = val;
            }

            // setting the sleepTime attribute from the userdata settings
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("intervalWhenAsleep"))
            {
                uint val = Convert.ToUInt32(ApplicationData.Current.LocalSettings.Values["intervalWhenAsleep"]);
                if (val > 50)
                    sleepTime = val;
            }

            // setting shakeThreshold attribute from the userdata settings
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("shakeThreshold"))
            {
                double val = Convert.ToDouble(ApplicationData.Current.LocalSettings.Values["shakeThreshold"]);
                if (val > 2.0)
                    shakeThreshold = val;
            }
        }

        private void setReportInterval(bool sleepyState)
        {

            uint minReportIntervalMsecs = Accelerometer.MinimumReportInterval;
            //default interval is 50 ms. when in sleepyState change it to 3 sec. (50 ms and 3 sec are defaults.. can be changed with usersettings from the gui)
            uint interval = awakeTime;

            if (sleepyState)
            {
                interval = sleepTime;
            }

            Accelerometer.ReportInterval = minReportIntervalMsecs > interval ? minReportIntervalMsecs : interval;
        }

        /// <summary> 
        /// Called when the background task is canceled by the app or by the system.
        /// </summary> 
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            ApplicationData.Current.LocalSettings.Values["TaskCancelationReason"] = reason.ToString();
            ApplicationData.Current.LocalSettings.Values["SampleCount"] = SampleCount;
            ApplicationData.Current.LocalSettings.Values["IsBackgroundTaskActive"] = false;

            if (null != Accelerometer)
            {
                Accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                Accelerometer.ReportInterval = 0;
            }

            // Complete the background task (this raises the OnCompleted event on the corresponding BackgroundTaskRegistration).
            Deferral.Complete();
        }

        /// <summary>
        /// Frees resources held by this background task.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// This is the event handler for acceleroemter ReadingChanged events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            SampleCount++;

            // if appHasLaunched it means the report interval was set to sleepTime. this only is true when the sleeptime is over and the first readingchanged event has arrived
            // when this is the case the appHaslaunched can be reset to false.
            if (appHasLaunched)
                appHasLaunched = false;

            // Save the sample count if the foreground app is visible.
            bool appVisible = true; //(bool)ApplicationData.Current.LocalSettings.Values["IsAppVisible"];
            if (appVisible)
            {
                AccelerometerReading reading = e.Reading;

                saveReadings(reading);

                if (SampleCount > 5)
                    ApplicationData.Current.LocalSettings.Values["SampleCount"] = SampleCount;

                ApplicationData.Current.LocalSettings.Values["xval"] = String.Format("{0,5:0.00}", reading.AccelerationX);
                ApplicationData.Current.LocalSettings.Values["yval"] = String.Format("{0,5:0.00}", reading.AccelerationY);
                ApplicationData.Current.LocalSettings.Values["zval"] = String.Format("{0,5:0.00}", reading.AccelerationZ);
            }

            // once in every 5 readingChanged check if the phone has been shaken.
            if ((SampleCount % 5) == 0)
            {
                appHasLaunched = checkIfPhoneHasBeenShaken();
            }
            
            // when checkIfPhoneHasBeenShaken has been run it returns a bool. this bool tels if it has launched an app or not.
            if(appHasLaunched)
            {
                //when an app has been lauched the reportinterval gets set to sleepyState.
                setReportInterval(true); //the report interval is now set to a long time. (3 sec default) we wont be able to start a new app during this time.
            }
            else
            {
                setReportInterval(false); // the report interval is set to the standard interval (50ms default).
            }
        }

        private bool checkIfPhoneHasBeenShaken()
        {
            //function result
            bool result = false;

            //check and store if any of the directions has been shaken.
            bool x = IsShakenX();
            bool y = IsShakenY();
            bool z = IsShakenZ();

            //for debugging purpose.
            string debugString_hasbeenShaken = "";

            string uri = "";

            //The next set of if statements checks the bools. only act if there is a shake in 1 exclusive direction
            if (x && !y && !z)
            {
                uri = (string)ApplicationData.Current.LocalSettings.Values["xUri"];
                debugString_hasbeenShaken += "HASBEENSHAKEN! X axel";
            }

            if (!x && y && !z)
            {
                uri = (string)ApplicationData.Current.LocalSettings.Values["yUri"];
                debugString_hasbeenShaken += "HASBEENSHAKEN! Y axel";
            }

            if (!x && !y && z)
            {
                uri = (string)ApplicationData.Current.LocalSettings.Values["zUri"];
                debugString_hasbeenShaken += "HASBEENSHAKEN! Z axel";
            }

            //launch the uri that has been set. if it has been set.
            if (!string.IsNullOrEmpty(uri))
            {
                //not awaiting this method because we are not waiting for a response.
                Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
                // result is true. an app has been launched (or atleast the attempt has been made)
                result = true;
            }

            return result;
        }

        private bool IsShakenX()
        {
            bool result = false;
            double lowest = measurementsX[0];
            double highest = measurementsX[0];

            for (int i = 1; i < 5; i++)
            {
                if (measurementsX[i] < lowest)
                    lowest = measurementsX[i];

                if (measurementsX[i] > highest)
                    highest = measurementsX[i];
            }
            double delta = Math.Abs(lowest - highest);
            double threshold = shakeThreshold;

            if (delta >= threshold)
                result = true;

            return result;
        }


        private bool IsShakenY()
        {
            bool result = false;
            double lowest = measurementsY[0];
            double highest = measurementsY[0];

            for (int i = 1; i < 5; i++)
            {
                if (measurementsY[i] < lowest)
                    lowest = measurementsY[i];

                if (measurementsY[i] > highest)
                    highest = measurementsY[i];
            }
            double delta = Math.Abs(lowest - highest);
            double threshold = shakeThreshold;

            if (delta >= threshold)
                result = true;

            return result;
        }


        private bool IsShakenZ()
        {
            bool result = false;
            double lowest = measurementsZ[0];
            double highest = measurementsZ[0];

            for (int i = 1; i < 5; i++)
            {
                if (measurementsZ[i] < lowest)
                    lowest = measurementsZ[i];

                if (measurementsZ[i] > highest)
                    highest = measurementsZ[i];
            }
            double delta = Math.Abs(lowest - highest);
            double threshold = shakeThreshold;

            if (delta >= threshold)
                result = true;

            return result;
        }

        private void saveReadings(AccelerometerReading reading)
        {
            int i = (int)(SampleCount % 5);

            measurementsX[i] = Convert.ToDouble( String.Format("{0,5:0.00}", reading.AccelerationX) );
            measurementsY[i] = Convert.ToDouble( String.Format("{0,5:0.00}", reading.AccelerationY) );
            measurementsZ[i] = Convert.ToDouble( String.Format("{0,5:0.00}", reading.AccelerationZ) );
        }
    }
}
