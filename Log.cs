using System;
using System.Diagnostics;

namespace OutlookReminder
{
    internal class Log
    {
        static string _source = typeof(Log).Namespace;

        public static void Write(EventLogEntryType level, string message)
        {
            try
            {
                EventLog.WriteEntry(_source, message, level);
            }
            catch (Exception)
            { }
        }

        public static void Write(EventLogEntryType level, string message, params object[] args)
        {
            try
            {
                EventLog.WriteEntry(_source, string.Format("{0}: {1}", _source, string.Format(message, args)), level);
            }
            catch (Exception)
            { }   
        }

        public static void Write(EventLogEntryType level, string message, object arg)
        {
            try
            {
                EventLog.WriteEntry(_source, string.Format("{0}: {1}", _source, string.Format(message, new[] { arg })), level);
            }
            catch (Exception)
            { }
        }
    }
}
