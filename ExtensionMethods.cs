using System;

namespace OutlookReminder
{
    static class ExtensionMethods
    {
        public static int ToInt32(this double d)
        {
            return Convert.ToInt32(d);
        }
    }
}
