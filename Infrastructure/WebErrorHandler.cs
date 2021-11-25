using MyUtils;
using System;
using System.Text;
using System.Web;

namespace protean.Infrastructure
{
    public class WebErrorHandler
    {
        /// <summary>
        /// Creates a summary of system and user information to be sent to recipient in an email and recorded to the error log.
        /// </summary>
        /// <param name="page">Page which error was generated as a String</param>
        /// <param name="ex">Exception generated as an Exception</param>
        /// <param name="text">Optional text for added debugging as a String.  Default is blank.</param>
        /// <param name="sendMail">Optional Boolean value to send email of error.  Default is true.</param>
        public static void HandleError(string page, Exception ex, string text = "", bool sendMail = true)
        {
            var sb = new StringBuilder();
            var errorCode = "";

            // Set defaults
            var writeError = true;
            var deliverMail = true;

            // If keys are set in web.config, use those, else use defaults
            try
            {
                if (!string.IsNullOrWhiteSpace(EnvironmentSettings.AppSettings("WriteErrorToFile")))
                {
                    writeError = Convert.ToBoolean(EnvironmentSettings.AppSettings("WriteErrorToFile"));
                }
            }
            catch (Exception)
            {
                writeError = true;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(EnvironmentSettings.AppSettings("WriteErrorToEmail")))
                {
                    deliverMail = Convert.ToBoolean(EnvironmentSettings.AppSettings("WriteErrorToEmail"));
                }
            }
            catch (Exception)
            {
                deliverMail = sendMail;
            }

            try
            {
                if ((ex) is HttpException)
                {
                    var checkException = (HttpException)ex;
                    errorCode = checkException.GetHttpCode().ToString();
                }

                sb.AppendLine(@"****************************************************");
                sb.AppendLine(@"Error on:  portal.grandrapidschair.com");
                sb.AppendLine(@"Time:  " + DateTime.Now.ToString());
                sb.AppendLine(@"Error Code:  " + errorCode);
                sb.AppendLine(@"Page:  " + page);
                sb.AppendLine();
                sb.AppendLine(@"Supplied Text:  " + text);
                sb.AppendLine();

                //Build user information that can be used to help solve the issue.
                sb.AppendLine(@"USER INFORMATION");
                var id = HttpContext.Current.User.Identity.Name;
                if (!String.IsNullOrWhiteSpace(id))
                {
                    sb.AppendLine(@"User: " + id);
                }
                sb.AppendLine(@"IP:  " + HttpContext.Current.Request.UserHostAddress);
                sb.AppendLine(@"Platform:  " + HttpContext.Current.Request.Browser.Platform.ToString());
                sb.AppendLine(@"Browser Type:  " + HttpContext.Current.Request.Browser.Type + ",  " + HttpContext.Current.Request.Browser.Version);
                sb.AppendLine();
                sb.AppendLine(@"ERROR GENERATED:");
                sb.AppendLine((ex.ToString().Length > 3000 ? ex.ToString().Substring(0, 3000) : ex.ToString()));

                // Crawlers may attempt to use a cached version of the WebResource.axd to scan the site.  This causes
                // a problem when the .NET Framework updates and it changes.  Do not send the error email
                if (page.Contains("WebResource.axd") && ex.ToString().Contains("This is an invalid webresource request"))
                {
                    deliverMail = false;
                }

                try
                {
                    // Send email
                    if (deliverMail) { SendErrorEmail(sb.ToString()); }
                }
                catch (Exception)
                {
                    // if this fails, make sure to write the error to file so there is a record
                    writeError = true;
                }

                // Write to text file
                if (writeError) { Utils.WriteErrorLog(sb.ToString()); }
            }
            catch (Exception)
            {
                HttpContext.Current.Server.ClearError();
            }
        }

        /// <summary>
        /// Send error summary as email.
        /// </summary>
        /// <param name="str">Error summary as a String</param>
        private static void SendErrorEmail(string str)
        {
            var n = new Notify(EnvironmentSettings.AppSettings("SMTPServer"));
            n.Send(
               "portal.grandrapidschair.com Error",
               str,
               String.Format(@"{0}<{1}>", EnvironmentSettings.AppSettings("AutomatedEmailUser"), EnvironmentSettings.AppSettings("AutomatedEmailEmail")),
               EnvironmentSettings.AppSettings("ErrorEmailRecipient")
            );
        }
    }
}