using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask
{
    public static class BackgroundTaskConfiguration
    {
        //public static readonly double SHAKETHRESHOLD = 3.0;
        //public static readonly int INTERVAL_WHEN_AWAKE = 100;
        //public static readonly int INTERVAL_WHEN_ASLEEP = 3000;

        public static double SHAKETHRESHOLD
        {
            get
            {
                return 3.0;
            }
        }

        public static int INTERVAL_WHEN_AWAKE
        {
            get
            {
                return 100;
            }
        }

        public static int INTERVAL_WHEN_ASLEEP
        {
            get
            {
                return 3000;
            }
        }
    }
}
