// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionManagement.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the ExceptionManagement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using LIB.Tools.Security;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    /// <summary>
    /// The exception management.
    /// </summary>
    public class ExceptionManagement
    {
        /// <summary>
        /// The handle exception.
        /// </summary>
        /// <param name="ex">
        /// The Exception.
        /// </param>
        /// <param name="sendToLog">
        /// The b send to log.
        /// </param>
        /// <param name="sendToDevelopers">
        /// The b send to developers.
        /// </param>
        /// <param name="additionalInfo">
        /// The additional_ info.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1123:DoNotPlaceRegionsWithinElements", Justification = "Reviewed. Suppression is OK here.")]
        public static string HandleExceptionByEmail(
            Exception ex, bool sendToLog, bool sendToDevelopers, string additionalInfo = "")
        {
            try
            {
                #region Construct Message
                var fullExceptionInfo = new System.Text.StringBuilder(2000);

                fullExceptionInfo.Append("Exception Occured\r\n");

                var ctx = new HttpContextWrapper(HttpContext.Current);

                // add session info when possible
                if (null == ctx.Session)
                {
                    fullExceptionInfo.AppendFormat("Session is null \r\n");
                }

                // add server variables info
                fullExceptionInfo.AppendFormat("HTTP_USER_AGENT: {0}\r\n", ctx.Request.UserAgent);
                fullExceptionInfo.AppendFormat("REMOTE_ADDRESS: {0}\r\n", ctx.Request.UserHostAddress);
                if (ctx.Request.UrlReferrer != null)
                {
                    fullExceptionInfo.AppendFormat("HTTP_REFERER: {0}\r\n", ctx.Request.UrlReferrer.AbsolutePath);
                }

                fullExceptionInfo.AppendFormat("HTTP_HOST: {0}\r\n", ctx.Request.RawUrl);

                fullExceptionInfo.AppendFormat("\r\nMessage: {0}\r\n", ex.Message);

                var currentUser = Authentication.GetCurrentUser();
                if (currentUser != null)
                {
                    fullExceptionInfo.AppendFormat("\r\nUser: {0}\r\n", currentUser.GetName());
                }
                fullExceptionInfo.AppendFormat("Source: {0}\r\n", ex.Source);
                fullExceptionInfo.AppendFormat("AllData: {0}\r\n\r\n", ex.ToString());

                if (null != ctx)
                {
                    fullExceptionInfo.AppendFormat("RequestType: {0}\r\n", ctx.Request.RequestType);
                    fullExceptionInfo.AppendFormat("QueryString: {0}\r\n", ctx.Request.QueryString);
                    var rdr = new System.IO.StreamReader(ctx.Request.InputStream, System.Text.Encoding.UTF8);
                    var inputStream = new System.Text.StringBuilder(1000);
                    var strRequest = rdr.ReadToEnd();
                    if (strRequest != string.Empty && strRequest.Length > 1000)
                    {
                        inputStream.Append(strRequest, 0, 1000);
                    }
                    else if (strRequest != string.Empty)
                    {
                        inputStream.Append(strRequest, 0, strRequest.Length);
                    }

                    fullExceptionInfo.AppendFormat("InputStream: {0}\r\n", inputStream.ToString().Trim());
                }

                var stringErrorMessage = fullExceptionInfo.ToString();
                #endregion

                #region Write Exception To Windows Event Log
                if (sendToLog)
                {
                    var ev = new System.Diagnostics.EventLog("Application");
                    ev.Source = AppConstants.AppName;

                    ev.WriteEntry(stringErrorMessage, System.Diagnostics.EventLogEntryType.Error);
                }
                #endregion

                #region Send E-mail about exception to operator

                var msgTo = System.Configuration.ConfigurationManager.AppSettings["MailToSupport"];

                if (sendToDevelopers && !string.IsNullOrEmpty(msgTo))
                {
                    UtilityMail.SendMessage(
                        new System.Web.UI.WebControls.Panel(), stringErrorMessage, null, msgTo, Config.GetConfigValue("AppDevName") + " Support", false);
                }

                #endregion

                return stringErrorMessage;
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        public static void HandleExceptionByAPI(Exception pException, HttpContext ctx = null)
        {
            try
            {
                var client = new ExceptionHandler.ExceptionServiceClient();

                var lExcep = new ExceptionHandler.Excep() { Name = pException.Message };
                var fullExceptionInfo = new System.Text.StringBuilder(2000);
                if (ctx != null)
                {

                    lExcep.StatusCode = new ExceptionHandler.StatusCode() { Value = ctx.Response.StatusCode };
                    lExcep.URL = ctx.Request.RawUrl;
                    lExcep.QueryString = ctx.Request.QueryString.ToString();

                    var rdr = new System.IO.StreamReader(ctx.Request.InputStream, System.Text.Encoding.UTF8);
                    var inputStream = new System.Text.StringBuilder(1000);
                    var strRequest = rdr.ReadToEnd();
                    if (strRequest != string.Empty && strRequest.Length > 1000)
                    {
                        inputStream.Append(strRequest, 0, 1000);
                    }
                    else if (strRequest != string.Empty)
                    {
                        inputStream.Append(strRequest, 0, strRequest.Length);
                    }

                    lExcep.Post = inputStream.ToString().Trim();

                    var currentUser = Authentication.GetCurrentUser();
                    if (currentUser != null)
                    {
                        lExcep.User = currentUser.GetName();
                    }

                    lExcep.UserAgent = ctx.Request.UserAgent;
                    lExcep.UserHostAdress = ctx.Request.UserHostAddress;
                    if (ctx.Request.UrlReferrer != null)
                    {
                        lExcep.UrlReferrer = ctx.Request.UrlReferrer.AbsolutePath;
                    }
                    lExcep.RequestType = new ExceptionHandler.RequestType() { Name = ctx.Request.RequestType };
                }
                else
                {
                    fullExceptionInfo.AppendFormat("Session is null \r\n");
                }

                fullExceptionInfo.Append(pException.ToString());
                lExcep.Body = fullExceptionInfo.ToString();


                client.SaveException(lExcep, Config.GetConfigValue("JuliaAlertInstallCode"));
            }
            catch (Exception ex)
            {
                General.TraceWarn(ex.ToString());
            }
        }

        public static System.Web.Routing.RouteValueDictionary customError(HttpException httpException, System.Web.Routing.RouteValueDictionary routeValues = null)
        {

            HttpApplication httpApplication = null;

            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        routeValues.Add("action", "NotFound");

                        break;
                    case 503:
                        routeValues.Add("action", "Unavailable");

                        break;
                    default:
                        routeValues.Add("action", "Index");

                        break;

                }
            }
            else
            {
                routeValues.Add("action", "Index");
            }
            return routeValues;
        }
    }
}