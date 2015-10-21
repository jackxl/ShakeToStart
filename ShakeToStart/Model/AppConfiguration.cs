using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakeToStart.Model
{
    /// <summary>
    /// AppConfiguration in case there is no data use this.
    /// </summary>
    class AppConfiguration
    {
        public const double SHAKETHRESHOLD = 3.0;
        public const int INTERVAL_WHEN_AWAKE = 100;
        public const int INTERVAL_WHEN_ASLEEP = 3000;

        public const string X_URI = @"zune";
        public const string Y_URI = @"ms-settings-wifi";
        public const string Z_URI = @"http://facebook.com";

        public const string BACKGROUND_TASKNAME = "ShakeToStart BackgroundTask";
        public const string BACKGROUND_TAKS_ENTRYPOINT = "BackgroundTask.BackgroundTask";
    }
}
