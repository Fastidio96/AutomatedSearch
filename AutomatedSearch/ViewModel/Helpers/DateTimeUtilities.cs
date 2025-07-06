using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedSearch.ViewModel.Helpers
{
    public class DateTimeUtilities
    {
        public static bool HasElapsed(DateTime now, DateTime startTime, TimeSpan diff)
        {
            return (now - startTime) > diff;
        }

        public static bool HasElapsed(DateTime now, ref DateTime lastCall, TimeSpan diff)
        {
            bool res = (now - lastCall) > diff;
            lastCall = res ? now : lastCall; // Update only when lastCall when datime has elapsed

            return res;
        }
    }
}
