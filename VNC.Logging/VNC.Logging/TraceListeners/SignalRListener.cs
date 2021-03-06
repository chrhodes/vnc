﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Cors;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
//using Microsoft.Owin.Hosting;
using Owin;

//using VNC.Logging.CustomTraceListeners.ServiceReference1;

namespace VNC.Logging.TraceListeners
{
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    public class SignalRListener : Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.CustomTraceListener
    {
        //private const Int32 BASE_ERRORNUMBER = EaseCore.ErrorNumbers.EASECLASS;
        //private const string LOG_APPNAME = "SIGNALRLISTENER";
        private const string LOG_APPNAME = "EASECORE";

        private string sLoggingDbConString = string.Empty;
        private int iSQLCommandTimeoutInSecs = 300;
        private Int32 iPriority = 0;
        private string sEntryID = string.Empty;
        private string sMachineName = string.Empty;
        private string sUserName = string.Empty;
        private string sApplicationName = string.Empty;
        private string sCategoryName = string.Empty;
        private string sSeverity_TC = string.Empty;
        private string sMessage_Text = string.Empty;
        private string sSessionID = string.Empty;
        private string sThreadID = string.Empty;
        private string sExecutableName = string.Empty;
        private string sCallstack = string.Empty;
        private double dPerformance = 0.00;
        private int iEventID = 0;

        private double maxDuration = -1; // Log if greater than this time.

        string[] _supportedCustomAttributes = new string[] {
            "maxDuration", "MaxDuration", "maxduration",
            "suppressEnter", "SuppressEnter", "suppressenter" };

        //private LiveViewClient client = null;

        public IDisposable SignalR { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public SignalRListener()
        {
            //Log.Info("SignalRListener()", LOG_APPNAME);

            //client = new LiveViewClient();
            ConnectAsync();
        }

        public SignalRListener(string duration)
        {
            maxDuration = double.Parse(duration);
            //Log.Info("SignalRListener()", LOG_APPNAME);

            //client = new LiveViewClient();
            ConnectAsync();
        }

        protected override string[] GetSupportedAttributes()
        {
            return _supportedCustomAttributes;
        }
    
        public double MaxDuration
        {
            get
            {
                if (maxDuration < 0)
                {
                    // Initialize from the custom attributes

                    var key = Attributes.Keys.Cast<string>().
                    FirstOrDefault(s => string.Equals(s, "maxduration", StringComparison.InvariantCultureIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        double.TryParse(Attributes[key], out maxDuration);
                    }
                }

                return maxDuration;
            }
        }

        private bool? suppressEnter = null;

        public bool SuppressEnter
        {
            get
            {
                if (suppressEnter == null)
                {
                    // Initialize from the custom attributes

                    var key = Attributes.Keys.Cast<string>().
                    FirstOrDefault(s => string.Equals(s, "suppressenter", StringComparison.InvariantCultureIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        bool suppress = false;
                        bool.TryParse(Attributes[key], out suppress);
                        suppressEnter = suppress;
                    }
                }

                return (bool)suppressEnter;
            }
        }
        /// <summary>
        /// This name is simply added to sent messages to identify the user; this 
        /// sample does not include authentication.
        /// </summary>
        public String UserName { get; set; }
        public IHubProxy HubProxy { get; set; }
        const string ServerURI = "http://localhost:8095/signalr";
        const string SignalRListenerUser = "SRL";   // This identifies this user to the Hub

        public HubConnection Connection { get; set; }

        /// <summary>
        /// List of Attributes allowed
        /// </summary>
        /// <returns></returns>
        //protected override string[] GetSupportedAttributes()
        //{
        //    return new string[] { "LoggingDbConString", "SQLCommandTimeout" };
        //    //return base.GetSupportedAttributes();
        //}
        public override void Write(string message)
        {
            try
            {
                //message = message + "*W*";
                //client.DisplayLogEntry(message);  // Named pipes
                //HubProxy.Invoke("Send", SignalRListenerUser, message);
                HubProxy.Invoke("SendPriority", message, iPriority);
            }
            catch (InvalidOperationException)
            {
                // Logging framework likely spins up worker threads that are killed
                // if not active.  When that happens we need to start again.

                ConnectAsync();

                // Send the message so it doesn't get lost

                HubProxy.Invoke("SendPriority", message, iPriority);
            }
            catch (Exception ex)
            {
                //Log.Error(ex, LOG_APPNAME);
                var errorMessage = ex.ToString();
                //client.DisplayLogEntry(string.Format("SRLWex: {0}", ex.ToString()));
            }
        }

        public override void WriteLine(string message)
        {
            try
            {
                //message = message + "*WL*";
                //client.DisplayLogEntry(message); named pipes
                //HubProxy.Invoke("Send", SignalRListenerUser, message);
                HubProxy.Invoke("SendPriority", message, iPriority);
            }
            catch (System.InvalidOperationException)
            {
                // Logging framework likely spins up worker threads that are killed
                // if not active.  When that happens we need to start again.
                // This seems less likely.

                // Calling ConnectAsync gives us two clients.

                //ConnectAsync();

                // Send the message so it doesn't get lost

                switch (Connection.State)
                {
                    case Microsoft.AspNet.SignalR.Client.ConnectionState.Connected:
                        HubProxy.Invoke("SendPriority", message, iPriority);
                        break;

                    case Microsoft.AspNet.SignalR.Client.ConnectionState.Connecting:

                        break;

                    case Microsoft.AspNet.SignalR.Client.ConnectionState.Reconnecting:

                        break;

                    case Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected:
                        ConnectAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, LOG_APPNAME);
                var errorMessage = ex.ToString();
                //client.DisplayLogEntry(string.Format("SRLWLex: {0}", ex.ToString()));
                //ConnectAsync();
            }
        }

        /// <summary>
        /// Creates and connects the hub connection and hub proxy. This method
        /// is called asynchronously from SignInButton_Click.
        /// </summary>
        private async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);

            Connection.Closed += Connection_Closed;
            Connection.Error += Connection_Error;
            Connection.Received += Connection_Received;
            Connection.Reconnected += Connection_Reconnected;
            Connection.Reconnecting += Connection_Reconnecting;
            Connection.StateChanged += Connection_StateChanged;

            HubProxy = Connection.CreateHubProxy("MyHub");

            //Handle incoming event from server: use Invoke to write to console from SignalR's thread

            //HubProxy.On<string, string>("AddMessage", (name, message) =>
            //    this.Dispatcher.Invoke(() =>
            //        RichTextBoxConsole.AppendText(String.Format("{0}: {1}\r", name, message))
            //    )
            //);

            try
            {
                //Connection.Start().RunSynchronously();
                await Connection.Start();
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = ex.ToString();
                //Log.Error(string.Format("SRLCAex: {0}", ex.ToString()), LOG_APPNAME);
                //client.DisplayLogEntry(string.Format("SRLCAex: {0}", ex.ToString()));
                //StatusText.Content = "Unable to connect to server: Start server before connecting clients.";
                //No connection: Don't enable Send button or show chat UI
                return;
            }
            catch (Exception ex)
            {
                var errorMessage = ex.ToString();
            }

            //Show chat UI; hide login UI
            //SignInPanel.Visibility = Visibility.Collapsed;
            //ChatPanel.Visibility = Visibility.Visible;
            //ButtonSend.IsEnabled = true;
            //TextBoxMessage.Focus();
            //RichTextBoxConsole.AppendText("Connected to server at " + ServerURI + "\r");
        }

        #region Connection Events

        void Connection_Reconnected()
        {
            var info = Connection;
            //var dispatcher = Application.Current.Dispatcher;

            //dispatcher.Invoke(() => teLogStream.Text += "Connection_Reconnected\n");
        }

        void Connection_Reconnecting()
        {
            var info = Connection;
            //var dispatcher = Application.Current.Dispatcher;

            //dispatcher.Invoke(() => teLogStream.Text += "Connection_Reconnecting\n");
        }

        void Connection_StateChanged(StateChange obj)
        {
            var info = Connection;
            //var dispatcher = Application.Current.Dispatcher;
            //var message = string.Format("Connection_StateChanged {0,15} -> {1,-15}\n", obj.OldState, obj.NewState);

            //dispatcher.Invoke(() => teLogStream.Text += message);
        }

        private void Connection_Received(string obj)
        {
            var info = Connection;
            //var dispatcher = Application.Current.Dispatcher;

            //dispatcher.Invoke(() => teLogStream.Text += "Connection_Received");
        }

        private void Connection_Error(Exception obj)
        {
            var info = Connection;
            //var dispatcher = Application.Current.Dispatcher;

            //var message = string.Format("Connection_Error >{0}<\n", obj.GetBaseException().ToString());
            //dispatcher.Invoke(() => teLogStream.Text += message);
        }

        /// <summary>
        /// If the server is stopped, the connection will time out after 30 seconds (default), and the 
        /// Closed event will fire.
        /// </summary>
        void Connection_Closed()
        {
            var info = Connection;
            ////Hide chat UI; show login UI
            //var dispatcher = Application.Current.Dispatcher;
            //dispatcher.Invoke(() => ChatPanel.Visibility = Visibility.Collapsed);
            //dispatcher.Invoke(() => btnSendPriority.IsEnabled = false);
            //dispatcher.Invoke(() => btnSend.IsEnabled = false);
            //dispatcher.Invoke(() => teLogStream.Text += "You have been disconnected.\n");
            //dispatcher.Invoke(() => SignInPanel.Visibility = Visibility.Visible);
        }
        #endregion

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            //Dictionary<String, Object> dictionary = new Dictionary<String, Object>();

            ////Get Logging database connection string
            //if (Attributes.ContainsKey("LoggingDbConString") == true)
            //{
            //    sLoggingDbConString = Attributes["LoggingDbConString"];
            //}
            //else
            //{
            //    throw new LoggingException("Logging Database Connection String is missing");
            //}
            ////Get SQL Command Timeout in seconds
            //if (Attributes.ContainsKey("SQLCommandTimeout") == true)
            //{
            //    iSQLCommandTimeoutInSecs = Convert.ToInt32(Attributes["SQLCommandTimeout"]);
            //}
            //else
            //{
            //    //use default value
            //    iSQLCommandTimeoutInSecs = 300;
            //}
            if (data is LogEntry && this.Formatter != null)
            {
                try
                {
                    LogEntry e = (LogEntry)data;
                    this.iPriority = e.Priority;
                    this.sEntryID = System.Guid.NewGuid().ToString();
                    this.sMachineName = e.MachineName;
                    this.sApplicationName = e.CategoriesStrings[0];
                    this.iEventID = e.EventId;
                    this.sCategoryName = e.CategoriesStrings[0];
                    this.sSeverity_TC = e.Severity.ToString();
                    this.sMessage_Text = e.Message;
                    this.sThreadID = e.Win32ThreadId;

                    foreach (KeyValuePair<string, Object> kvp in e.ExtendedProperties)
                    {
                        switch (kvp.Key.ToString())
                        {
                            case "User Name":
                                {
                                    this.sUserName = kvp.Value.ToString();
                                    break;
                                }
                            case "Calling Assembly":
                                {

                                    this.sExecutableName = kvp.Value.ToString();
                                    break;
                                }
                            case "Stack":
                                {
                                    this.sCallstack = kvp.Value.ToString();
                                    break;
                                }
                            case "Duration":
                                {
                                    this.dPerformance = Convert.ToDouble(kvp.Value);
                                    break;
                                }
                            default:
                                {
                                    //dictionary.Add(kvp.Key.ToString(), kvp.Value);
                                    break;
                                }
                        }
                    }

                    string message = this.Formatter.Format(data as LogEntry);

                    this.WriteLine(message);

                    //if (dPerformance > MaxDuration)
                    //{
                    //    this.WriteLine(message);
                    //    dPerformance = 0;
                    //}
                    //else if (message.Contains("Enter"))
                    //{
                    //    if (!SuppressEnter)
                    //    {
                    //        this.WriteLine(message);
                    //    }
                    //}
                    //else
                    //{
                    //    this.WriteLine("Hum" + message);
                    //}

                    //this.WriteLine(this.Formatter.Format(data as LogEntry));
                    //Insert Log entry
                    //InsertLogEntryIntoDb();

                    //foreach (KeyValuePair<string, Object> kvp in dictionary)
                    //{
                    //    InsertExtraData(kvp.Key, kvp.Value.ToString());
                    //}
                }
                catch (Exception ex)
                {
                    //Log.Error(ex, LOG_APPNAME);
                    //throw new LoggingException(ex.Message);
                    var errorMessage = ex.ToString();
                    //EventLog.WriteEntry("CustomDatabaseTraceListener", ex.Message, EventLogEntryType.Information);

                }
                finally
                {
                    //dictionary.Clear();
                    //dictionary = null;
                }
            }
            else
            {
                this.Write("not LogEntry");
                this.WriteLine(data.ToString());
                //Not a LogEntry. Ignore
            }
        }

        private bool InsertLogEntryIntoDb()
        {
            bool bRetVal = false;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                //Open SQL connection 
                conn = new SqlConnection(sLoggingDbConString);
                conn.Open();

                //Set command options
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = iSQLCommandTimeoutInSecs;

                //Set stored procedure
                cmd.CommandText = "sp_logging_add_log_entry";

                cmd.Parameters.Add(new SqlParameter("@entry_id", this.sEntryID));
                cmd.Parameters.Add(new SqlParameter("@writeDate", System.DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@machine_name", this.sMachineName));
                cmd.Parameters.Add(new SqlParameter("@user_name", this.sUserName));
                cmd.Parameters.Add(new SqlParameter("@application_name", this.sApplicationName));
                cmd.Parameters.Add(new SqlParameter("@category_name", this.sCategoryName));
                cmd.Parameters.Add(new SqlParameter("@severity_tc", this.sSeverity_TC.Substring(0, 1)));
                cmd.Parameters.Add(new SqlParameter("@error_number", 9999));
                cmd.Parameters.Add(new SqlParameter("@message_txt", this.sMessage_Text));
                cmd.Parameters.Add(new SqlParameter("@session_id", this.sSessionID));
                cmd.Parameters.Add(new SqlParameter("@thread_id", this.sThreadID));
                cmd.Parameters.Add(new SqlParameter("@executable_name", this.sExecutableName));
                cmd.Parameters.Add(new SqlParameter("@callstack", this.sCallstack));
                if (dPerformance != 0.00)
                {
                    cmd.Parameters.Add(new SqlParameter("@performance", dPerformance));
                }
                cmd.Parameters.Add(new SqlParameter("@step_int", 1));
                if (iEventID != 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@event_id", iEventID));
                }
                //Execute Query 
                int i = cmd.ExecuteNonQuery();
                cmd.Dispose();
                bRetVal = true;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, LOG_APPNAME);
                //throw new LoggingException(ex.Message);
                //EventLog.WriteEntry("CustomDatabaseTraceListener", ex.Message, EventLogEntryType.Error);
            }
            finally
            {
                cmd = null;

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            return bRetVal;
        }

        private bool InsertExtraData(string sKey, string sValue)
        {
            bool bRetVal = false;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                //Open SQL connection 
                conn = new SqlConnection(sLoggingDbConString);
                conn.Open();

                //Set command options
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = iSQLCommandTimeoutInSecs;

                //Set stored procedure
                cmd.CommandText = "sp_logging_add_extra_data";
                cmd.Parameters.Add(new SqlParameter("@entry_id", this.sEntryID));
                cmd.Parameters.Add(new SqlParameter("@name", sKey));
                cmd.Parameters.Add(new SqlParameter("@value", sValue));

                //Execute Query 
                int i = cmd.ExecuteNonQuery();
                cmd.Dispose();

                bRetVal = true;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, LOG_APPNAME);
                //throw new LoggingException(ex.Message);
                //EventLog.WriteEntry("CustomDatabaseTraceListener", ex.Message, EventLogEntryType.Error);
            }
            finally
            {

                cmd = null;

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }

            }
            return bRetVal;

        }
    }

    /// <summary>
    /// Used by OWIN's startup process. 
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    public class MyHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addUserMessage(name, message);
        }

        public void Send(string message)
        {
            Clients.All.addMessage(message);
        }
        public void SendPriority(string message, Int32 priority)
        {
            Clients.All.addPriorityMessage(message, priority);
        }

        //public override Task OnConnected()
        //{
        //    //Use Application.Current.Dispatcher to access UI thread from outside the MainWindow class
        //    Application.Current.Dispatcher.Invoke(() =>
        //        ((MainWindow)Application.Current.MainWindow).WriteToConsole("Client connected: " + Context.ConnectionId));

        //    return base.OnConnected();
        //}
        //public override Task OnDisconnected()
        //{
        //    //Use Application.Current.Dispatcher to access UI thread from outside the MainWindow class
        //    Application.Current.Dispatcher.Invoke(() =>
        //        ((MainWindow)Application.Current.MainWindow).WriteToConsole("Client disconnected: " + Context.ConnectionId));

        //    return base.OnDisconnected();
        //}
    }
}
