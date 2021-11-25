using MyUtils;
using System;
using System.IO;

namespace protean.Infrastructure
{
    public class Utils
    {
        #region Logging

        /// <summary>
        /// Log supplied text to error log.
        /// </summary>
        /// <param name="str">Text to log as a String</param>
        public static void WriteErrorLog(string str)
        {
            var logPath = String.Format(@"{0}GRCC Portal Errors\", EnvironmentSettings.AppSettings("Logs"));

            // Create separate directory if one does not exist.
            if (!Directory.Exists(logPath)) { Directory.CreateDirectory(logPath); }

            using (StreamWriter writer = new StreamWriter(String.Format(@"{0}{1:yyyyMMdd}_Error.log", logPath, DateTime.Now), true))
            {
                writer.WriteLine(str);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write given string to a log file.
        /// </summary>
        /// <param name="str">Message as a String</param>
        /// <remarks>This will only write if debug is set to true.</remarks>
        public static void WriteDebugLog(string str)
        {
            if (Convert.ToBoolean(EnvironmentSettings.AppSettings("DebugMode")))
            {
                var logPath = string.Format(@"{0}GRCC Portal Debug\", EnvironmentSettings.AppSettings("Logs"));

                // Create a separate directory if one does not exist
                if (!Directory.Exists(logPath)) { Directory.CreateDirectory(logPath); }

                using (StreamWriter writer = new StreamWriter(string.Format(@"{0}{1:yyyyMMdd}_Debug.log", logPath, DateTime.Now), true))
                {
                    writer.WriteLine(DateTime.Now.ToString() + " - " + str);
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Write given string to a log file for missing files
        /// </summary>
        /// <param name="str">Message as a String</param>
        public static void Write404ErrorLog(string str)
        {
            var logPath = string.Format(@"{0}GRCC Portal Errors\", EnvironmentSettings.AppSettings("Logs"));

            // Create a separate directory if one does not exist
            if (!Directory.Exists(logPath)) { Directory.CreateDirectory(logPath); }

            using (StreamWriter writer = new StreamWriter(string.Format(@"{0}{1:yyyyMMdd}_404_Error.log", logPath, DateTime.Now), true))
            {
                writer.WriteLine(str);
                writer.Flush();
            }
        }

        #endregion Logging

        /// <summary>
        /// Get build date for project
        /// This ONLY works if the assembly was built using VS.NET and the assembly version attribute is set to something like the below.
        /// The asterisk (*) is the important part, as if present, VS.NET generates both the build and revision numbers automatically.
        /// <Assembly: AssemblyVersion("1.0.*")>
        /// Note for app the version is set by opening the 'My Project' file and clicking on the 'assembly information' button.
        /// An alternative method is to simply read the last time the file was written, using something similar to:
        /// Return System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly.Location)
        /// </summary>
        /// <returns>build as a String</returns>
        public static DateTime GetBuildDate()
        {  
            //Build dates start from 01/01/2000
            var result = DateTime.Parse("1/1/2000");

            //Retrieve the version information from the assembly from which this code is being executed
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            //Add the number of days (build)
            result = result.AddDays(version.Build);

            //Add the number of seconds since midnight (revision) multiplied by 2
            result = result.AddSeconds(version.Revision * 2);

            //If we're currently in daylight saving time add an extra hour
            if (TimeZone.IsDaylightSavingTime(System.DateTime.Now, TimeZone.CurrentTimeZone.GetDaylightChanges(System.DateTime.Now.Year)))
            {
                result = result.AddHours(1);
            }

            return result;
        }

        /// <summary>
        /// Get the years we have been using Epicor
        /// </summary>
        /// <returns>Years as an Integer</returns>
        public static int GetYearsUsingEpicor()
        {
            var startYear = Convert.ToInt32(EnvironmentSettings.AppSettings("EpicorStartYear"));

            var diff = DateTime.Now.Year - startYear;

            return diff;
        }

        /// <summary>
        /// Handle what is in a json object as date or string.
        /// </summary>
        /// <param name="input">value</param>
        /// <returns>string date</returns>
        public static string HandleDatesInJson(dynamic input)
        {
            if (input != null)
            {
                return Convert.ToDateTime(input).ToString().Replace("00:00:00", "");
            }

            return "";
        }

        /// <summary>
        /// Handle what is in a json object as date or string without time.
        /// </summary>
        /// <param name="input">value</param>
        /// <returns>string date</returns>
        public static string HandleTimelessDatesInJson(dynamic input)
        {
            if (input != null)
            {
                return Convert.ToDateTime(input).ToShortDateString();
            }

            return "";
        }
    }
}