﻿//'------------------------------------------------------------------------------------------------------------------------
//'
//' TODO: Determine how to use the TraceEventType enumeration.  Vista seems to have more levels.  Windows 2003 fewer.  
//'       Should probably plan for future. Review the mapping levels
//'       Not sure how to get Critical messages to appear in eventlog.  Go look in EntLib code to see if they do mapping.
//'
//'       TraceEventType  Description                         Value   Vista Log   2003 Log    2008 Log    Log Method	    Priority Filter
//'       --------------  ---------------------------------   -----   ---------   --------    --------    ------------		---------------
//'       Critical        Fatal error or application crash    1       Error       ?           ?           Failure			Log.Failure		(-10)
//'       Error           Recoverable error                   2       Error                               Error				Log.Error		(-1)
//'       Warning         Non-Critical Problem                4       Warning                             Warning			Log.Warning		(1)       
//'       Information     Informational message               8       Information Information ?           Info				Log.Info		(100)
//'																										  Info1				Log.Info1		(101)
//'																										  Info2				Log.Info2		(102)
//'																										  Info3				Log.Info3		(103)
//'																										  Info4				Log.Info4		(104)
//'																										  Info5				Log.Info5		(105)
//'       Verbose         Debugging trace                     16                                          Debug				Log.Debug		(1000)
//'																										  Debug1			Log.Debug1		(1001)
//'																										  Debug2			Log.Debug2		(1002)
//'																										  Debug3			Log.Debug3		(1003)
//'																										  Debug4			Log.Debug4		(1004)
//'																										  Debug5			Log.Debug5		(1005)
//'                                                                                                       Trace				Log.Trace       (10000)
//'                                                                                                       Trace1			Log.Trace1		(10001)
//'                                                                                                       Trace2			Log.Trace2		(10002)
//'                                                                                                       Trace3			Log.Trace3		(10003)
//'                                                                                                       Trace4			Log.Trace4		(10004)
//'                                                                                                       Trace5			Log.Trace5		(10005)
//'                                                                                                       Trace6			Log.Trace6		(10006)
//'                                                                                                       Trace7			Log.Trace7		(10007)
//'                                                                                                       Trace8			Log.Trace8		(10008)
//'                                                                                                       Trace9			Log.Trace9		(10009)
//'                                                                                                       Trace10			Log.Trace10		(100010)
//'                                                                                                       Trace11			Log.Trace11		(100011)
//'                                                                                                       Trace12			Log.Trace12		(100012)
//'                                                                                                       Trace13			Log.Trace13		(100013)
//'                                                                                                       Trace14			Log.Trace14		(100014)
//'                                                                                                       Trace15			Log.Trace15		(100015)
//'                                                                                                       Trace16			Log.Trace16		(100016)
//'                                                                                                       Trace17			Log.Trace17		(100017)
//'                                                                                                       Trace18			Log.Trace18		(100018)
//'                                                                                                       Trace19			Log.Trace19		(100019)
//'                                                                                                       Trace20			Log.Trace20		(100020)
//'                                                                                                       Trace21			Log.Trace21		(100021)
//'                                                                                                       Trace22			Log.Trace22		(100022)
//'                                                                                                       Trace23			Log.Trace23		(100023)
//'                                                                                                       Trace24			Log.Trace24		(100024)
//'                                                                                                       Trace25			Log.Trace25		(100025)
//'                                                                                                       Trace26			Log.Trace26		(100026)
//'                                                                                                       Trace27			Log.Trace27		(100027)
//'                                                                                                       Trace28			Log.Trace28		(100028)
//'                                                                                                       Trace29			Log.Trace29		(100029)
//'                                                                                                       Trace30			Log.Trace30		(100030)
//'
//'       Start          Starting of logical operation        256
//'       Stop           Stopping of logical operation        512
//'       Suspend        Suspension of logical operation      1024
//'       Resume         Resumption of logical operation      2048
//'       Transfer       Changing of correlation identity     4096
//'
//' TO DECIDE:
//'   I don't think we need the methods that don't pass a applicationCategory.  I also think we might want to skip the Info, Debug, and Trace
//'   versions that take a stacktrace flag.
//'   That would leave two versions for Failure and Error and one each for Warning, Info, Debug, and Trace.
//'
//' NOTES:
//'   The methods use the
//'           <System.Diagnostics.DebuggerStepThrough()> _
//'   attribute to suppress tracing through the logging code.  You can still set breakpoints in the method if you want.
//'
//'------------------------------------------------------------------------------------------------------------------------

//''' <summary>
//''' 
//''' </summary>
//''' <remarks>
//      Wrapper around EnterpriseLibrary.Logging.Logger.Write  
//      Simplifies calling by setting values for Priority and Severity.  
//      Passes several extended properties.
//  </remarks>

namespace VNC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    [Serializable]
    public partial class Log
    {
        private const long NoElapsedTime = 0L;

        public enum LoggingPriority : int
        {
			// Some priorities have been used consistently and are named for easier use.
			// Keep this in sync with LogNamed.tt named_methods

			APPLICATION_START = 100,
			APPLICATION_END = 100,
			LOADEASE = 100,

			SQL_CALL = 1002,

			PAGE_LOAD = 10000,
			FORM_LOAD = 10000,

			EVENT_HANDLER = 10001,
            EVENT = 10021,

			STATUS = 10002,
            APPLICATION_INITIALIZE = 10002,

			REDIRECT_TRANSFER = 10003,

			POLLING = 10004,

			ERROR_TRACE = 10005,
			ERROR_TRACE_LOW = 10015,

			EASESYS_IO = 10006,
			EASESYS_IO_MED = 10016,
			EASESYS_IO_LOW = 10026,
            INFRASTRUCTURE = 10016,

			UI_CONTROL = 10007,
			UI_CONTROL_MED = 10017,
			UI_CONTROL_LOW = 10027,

            PRESENTATION = 10007,
            VIEW = 10017,
            VIEW_LOW = 10027,

			UTILITY = 10008,
			UTILITY_MED = 10018,
			UTILITY_LOW = 10028,
            VIEWMODEL = 10018,
            VIEWMODEL_LOW = 10028,

			OPERATION = 10009,
			OPERATION_LOW = 10019,
            CORE = 10009,
            MODULE = 10019,
            MODULE_INITIALIZE = 10029,

			APPLICATION_SESSION = 10010,
			APPLICATION_SESSION_LOW = 10020,

            APPLICATION = 10010,
            APPLICATIONSERVICES = 10020,

			SYSTEM_CONFIG = 10011,
			SYSTEM_CONFIG_LOW = 10021,

			FILE_DIR_IO = 10012,
			FILE_DIR_IO_LOW = 10022,

            DOMAIN = 10012,
            DOMAINSERVICES = 10022,

			DATABASE_IO = 10013,
			DATABASE_IO_LOW = 10023,

            PERSISTENCE = 10013,
            PERSISTENCE_LOW = 10023,

			SECURITY = 10014,
			SECURITY_LOW = 10024,

			DEFAULT = 10019,

			CLEAR_INITIALIZE = 10025,
            CONSTRUCTOR = 10025,

			// Below are the standard levels

            Failure		= -10,
            Error		= -1,
            Warning		= 1,
            Info		= 100,
			Info1		= 101,
			Info2		= 102,
			Info3		= 103,
			Info4		= 104,
			Info5		= 105,
            Debug		= 1000,
			Debug1		= 1001,
			Debug2		= 1002,
			Debug3		= 1003,
			Debug4		= 1004,
			Debug5		= 1005,
            Trace		= 10000,
			Trace1		= 10001,
			Trace2		= 10002,
			Trace3		= 10003,
			Trace4		= 10004,
			Trace5		= 10005,
			Trace6		= 10006,
			Trace7		= 10007,
			Trace8		= 10008,
			Trace9		= 10009,
			Trace10		= 10010,
			Trace11		= 10011,
			Trace12		= 10012,
			Trace13		= 10013,
			Trace14		= 10014,
			Trace15		= 10015,
			Trace16		= 10016,
			Trace17		= 10017,
			Trace18		= 10018,
			Trace19		= 10019,
			Trace20		= 10020,
			Trace21		= 10021,
			Trace22		= 10022,
			Trace23		= 10023,
			Trace24		= 10024,
			Trace25		= 10025,
			Trace26		= 10026,
			Trace27		= 10027,
			Trace28		= 10028,
			Trace29		= 10029,
			Trace30		= 10030,
			Max			= 10030
        }

        public enum ShowStack
        {
            No,
            Yes
        }

        static Log()
        {
            IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();
            LogWriterFactory logWriterFactory = new LogWriterFactory(configurationSource);
            Logger.SetLogWriter(logWriterFactory.Create());
        }

    #region Public Write Methods

        [DebuggerStepThrough]
        public static void Write(Exception ex, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, bool showStack)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;
            if (category.Length == 0)
            {
                category = "General";
            }
            InternalWrite(ex.Message + ex.StackTrace, severity, category, priority, className, methodName, name, showStack);
        }

        [DebuggerStepThrough]
        public static void Write(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, bool showStack)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;
            if (category.Length == 0)
            {
                category = "General";
            }
            InternalWrite(message, severity, category, priority, className, methodName, name, showStack);
        }

        [DebuggerStepThrough]
        public static void Write(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, bool showStack, long startTicks)
        {
            string name = Assembly.GetCallingAssembly().GetName().Name;
            if (category.Length == 0)
            {
                category = "General";
            }
            InternalWrite(message, severity, category, priority, className, methodName, name, showStack, startTicks);
        }

        [DebuggerStepThrough]
        public static void WriteLight(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, bool showStack, long startTicks)
        {
            if (category.Length == 0)
            {
                category = "General";
            }
            InternalWrite(message, severity, category, priority, className, methodName, "<unknown>", showStack, startTicks);
        }

    #endregion

    #region Private Write Methods

        [DebuggerStepThrough]
        private static void InternalWrite(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, string callingAssemblyName, bool showStack)
        {
            string str = "";

            if (showStack)
            {
                StackTrace trace = new StackTrace();
                string str2 = "";
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    str2 = method.ReflectedType.Name + "." + method.Name + " > " + str2;
                }
                str = str2 + " " + str;
            }

            LogEntry log = new LogEntry();

            if (category.Length == 0)
            {
                category = "General";
            }

            log.Categories.Add(category);
            log.Severity = severity;
            log.Priority = (int)priority;
            log.ExtendedProperties.Add("Calling Assembly", callingAssemblyName);
            log.ExtendedProperties.Add("Class Name", className);
            log.ExtendedProperties.Add("Method Name", methodName);
            log.ExtendedProperties.Add("Stack", str);
            log.ExtendedProperties.Add("User Name", Environment.UserName);
            log.Message = message;

            try
            {
                Logger.Write(log);
            }
			//catch (ServiceLocation.ActivationException)
			//{
			////	InternalWriter().Write(log);
			//}
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }

        [DebuggerStepThrough]
        private static void InternalWrite(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, string callingAssemblyName, bool showStack, int EventId)
        {
            string str = "";

            if (showStack)
            {
                StackTrace trace = new StackTrace();
                string str2 = "";
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    str2 = method.ReflectedType.Name + "." + method.Name + " > " + str2;
                }
                str = str2 + " " + str;
            }

            LogEntry log = new LogEntry();

            if (category.Length == 0)
            {
                category = "General";
            }

            log.Categories.Add(category);
            log.EventId = EventId;
            log.Severity = severity;
            log.Priority = (int)priority;
            log.ExtendedProperties.Add("Calling Assembly", callingAssemblyName);
            log.ExtendedProperties.Add("Class Name", className);
            log.ExtendedProperties.Add("Method Name", methodName);
            log.ExtendedProperties.Add("Stack", str);
            log.ExtendedProperties.Add("User Name", Environment.UserName);
            log.Message = message;

            try
            {
                Logger.Write(log);
            }
			//catch (ServiceLocation.ActivationException)
			//{
			////	InternalWriter().Write(log);
			//}
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }

        [DebuggerStepThrough]
        private static void InternalWrite(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, string callingAssemblyName, bool showStack, long startTicks)
        {
            string str = "";

            if (showStack)
            {
                StackTrace trace = new StackTrace();
                string str2 = "";
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    str2 = method.ReflectedType.Name + "." + method.Name + " > " + str2;
                }
                str = str2 + " " + str;
            }

            LogEntry log = new LogEntry();

            if (category.Length == 0)
            {
                category = "General";
            }

            log.Categories.Add(category);
            log.Severity = severity;
            log.Priority = (int)priority;
            log.ExtendedProperties.Add("Calling Assembly", callingAssemblyName);
            log.ExtendedProperties.Add("Class Name", className);
            log.ExtendedProperties.Add("Method Name", methodName);
            log.ExtendedProperties.Add("Stack", str);
            log.ExtendedProperties.Add("User Name", Environment.UserName);
            log.ExtendedProperties.Add("Duration", GetDuration(startTicks));
            log.Message = message;

            try
            {
                Logger.Write(log);
            }
			//catch (ServiceLocation.ActivationException)
			//{
			////	InternalWriter().Write(log);
			//}
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }

        [DebuggerStepThrough]
        private static void InternalWrite(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, string callingAssemblyName, bool showStack, Dictionary<string, string> extendedProp)
        {
            string str = "";

            if (showStack)
            {
                StackTrace trace = new StackTrace();
                string str2 = "";
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    str2 = method.ReflectedType.Name + "." + method.Name + " > " + str2;
                }
                str = str2 + " " + str;
            }

            LogEntry log = new LogEntry();

            if (category.Length == 0)
            {
                category = "General";
            }

            log.Categories.Add(category);
            log.Severity = severity;
            log.Priority = (int)priority;
            log.ExtendedProperties.Add("Calling Assembly", callingAssemblyName);
            log.ExtendedProperties.Add("Class Name", className);
            log.ExtendedProperties.Add("Method Name", methodName);
            log.ExtendedProperties.Add("Stack", str);
            log.ExtendedProperties.Add("User Name", Environment.UserName);

            foreach (KeyValuePair<string, string> pair in extendedProp)
            {
                log.ExtendedProperties.Add(pair.Key, pair.Value);
            }

            log.Message = message;

            try
            {
                Logger.Write(log);
            }
			//catch (ServiceLocation.ActivationException)
			//{
			////	InternalWriter().Write(log);
			//}
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }

        [DebuggerStepThrough]
        private static void InternalWrite(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, string callingAssemblyName, bool showStack, int EventId, long startTicks)
        {
            string str = "";

            if (showStack)
            {
                StackTrace trace = new StackTrace();
                string str2 = "";
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    str2 = method.ReflectedType.Name + "." + method.Name + " > " + str2;
                }
                str = str2 + " " + str;
            }

            LogEntry log = new LogEntry();

            if (category.Length == 0)
            {
                category = "General";
            }

            log.Categories.Add(category);
            log.EventId = EventId;
            log.Severity = severity;
            log.Priority = (int)priority;
            log.ExtendedProperties.Add("Calling Assembly", callingAssemblyName);
            log.ExtendedProperties.Add("Class Name", className);
            log.ExtendedProperties.Add("Method Name", methodName);
            log.ExtendedProperties.Add("Stack", str);
            log.ExtendedProperties.Add("User Name", Environment.UserName);
            log.ExtendedProperties.Add("Duration", GetDuration(startTicks));
            log.Message = message;

            try
            {
                Logger.Write(log);
            }
			//catch (ServiceLocation.ActivationException)
			//{
			////	InternalWriter().Write(log);
			//}
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }

        [DebuggerStepThrough]
        private static void InternalWrite(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, string callingAssemblyName, bool showStack, int EventId, Dictionary<string, string> extendedProp)
        {
            string str = "";

            if (showStack)
            {
                StackTrace trace = new StackTrace();
                string str2 = "";
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    str2 = method.ReflectedType.Name + "." + method.Name + " > " + str2;
                }
                str = str2 + " " + str;
            }

            LogEntry log = new LogEntry();

            if (category.Length == 0)
            {
                category = "General";
            }

            log.Categories.Add(category);
            log.EventId = EventId;
            log.Severity = severity;
            log.Priority = (int)priority;
            log.ExtendedProperties.Add("Calling Assembly", callingAssemblyName);
            log.ExtendedProperties.Add("Class Name", className);
            log.ExtendedProperties.Add("Method Name", methodName);
            log.ExtendedProperties.Add("Stack", str);
            log.ExtendedProperties.Add("User Name", Environment.UserName);

            foreach (KeyValuePair<string, string> pair in extendedProp)
            {
                log.ExtendedProperties.Add(pair.Key, pair.Value);
            }

            log.Message = message;

            try
            {
                Logger.Write(log);
            }
			//catch (ServiceLocation.ActivationException)
			//{
			////	InternalWriter().Write(log);
			//}
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }

        [DebuggerStepThrough]
        private static void InternalWrite(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, string callingAssemblyName, bool showStack, long startTicks, Dictionary<string, string> extendedProp)
        {
            string str = "";

            if (showStack)
            {
                StackTrace trace = new StackTrace();
                string str2 = "";
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    str2 = method.ReflectedType.Name + "." + method.Name + " > " + str2;
                }
                str = str2 + " " + str;
            }

            LogEntry log = new LogEntry();

            if (category.Length == 0)
            {
                category = "General";
            }

            log.Categories.Add(category);
            log.Severity = severity;
            log.Priority = (int)priority;
            log.ExtendedProperties.Add("Calling Assembly", callingAssemblyName);
            log.ExtendedProperties.Add("Class Name", className);
            log.ExtendedProperties.Add("Method Name", methodName);
            log.ExtendedProperties.Add("Stack", str);
            log.ExtendedProperties.Add("User Name", Environment.UserName);
            log.ExtendedProperties.Add("Duration", GetDuration(startTicks));

            foreach (KeyValuePair<string, string> pair in extendedProp)
            {
                log.ExtendedProperties.Add(pair.Key, pair.Value);
            }

            log.Message = message;

            try
            {
                Logger.Write(log);
            }
			//catch (ServiceLocation.ActivationException)
			//{
			////	InternalWriter().Write(log);
			//}
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }

        [DebuggerStepThrough]
        private static void InternalWrite(string message, TraceEventType severity, string category, LoggingPriority priority, string className, string methodName, string callingAssemblyName, bool showStack, int EventId, long startTicks, Dictionary<string, string> extendedProp)
        {
            string str = "";

            if (showStack)
            {
                StackTrace trace = new StackTrace();
                string str2 = "";
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    str2 = method.ReflectedType.Name + "." + method.Name + " > " + str2;
                }
                str = str2 + " " + str;
            }

            LogEntry log = new LogEntry();

            if (category.Length == 0)
            {
                category = "General";
            }

            log.Categories.Add(category);
            log.EventId = EventId;
            log.Severity = severity;
            log.Priority = (int)priority;
            log.ExtendedProperties.Add("Calling Assembly", callingAssemblyName);
            log.ExtendedProperties.Add("Class Name", className);
            log.ExtendedProperties.Add("Method Name", methodName);
            log.ExtendedProperties.Add("Stack", str);
            log.ExtendedProperties.Add("User Name", Environment.UserName);
            log.ExtendedProperties.Add("Duration", GetDuration(startTicks));

            foreach (KeyValuePair<string, string> pair in extendedProp)
            {
                log.ExtendedProperties.Add(pair.Key, pair.Value);
            }

            log.Message = message;

            try
            {
                Logger.Write(log);
            }
			//catch (ServiceLocation.ActivationException)
			//{
			////	InternalWriter().Write(log);
			//}
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }

		// This is new in EASE
		// Think it might be an alternative to the Log() constructor

		//private static LogWriter InternalWriter()
        //{
		//	//Internal Writer allows callers to just log without any configuration from the calling assembly
		//	//This also allows us to wrap tests around this class.
		//
        //    string loggingConfigSetting = Properties.Resources.LoggingConfiguration;
		//
        //    string tempConfigPath = System.IO.Path.GetTempFileName();
        //    System.IO.File.AppendAllText(tempConfigPath, loggingConfigSetting);
		//
        //    Microsoft.Practices.EnterpriseLibrary.Common.Configuration.FileConfigurationSource configSource = new Microsoft.Practices.EnterpriseLibrary.Common.Configuration.FileConfigurationSource(tempConfigPath);
        //    LogWriterFactory logFactory = new LogWriterFactory(configSource);
        //    return logFactory.Create();
        //}

    #endregion

// TODO(crhodes)
// Rethink this in light of LogNamed.tt
// Maybe treat this as just a bunch of named_methods
// E.g. Failure, Error, Warning, InfoN, DebugNN, TraceNN
// Could probably get clever with a few more TT loops

    #region Log Failure Methods

        [DebuggerStepThrough]
        public static void Failure(Exception ex, string applicationCategory)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure, method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, true);
        }

        [DebuggerStepThrough]
        public static void Failure(Exception ex, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, true, props);
        }

        [DebuggerStepThrough]
        public static void Failure(string message, string applicationCategory)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
        }

        [DebuggerStepThrough]
        public static void Failure(Exception ex, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(1), EventId);
        }

        [DebuggerStepThrough]
        public static void Failure(string message, string applicationCategory, bool showStack)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, showStack);
        }

        [DebuggerStepThrough]
        public static void Failure(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
        }

        [DebuggerStepThrough]
        public static void Failure(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
        }

        [DebuggerStepThrough]
        public static void Failure(Exception ex, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(1), EventId, props);
        }

        [DebuggerStepThrough]
        public static void Failure(string message, string applicationCategory, bool showStack, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, showStack, props);
        }

        [DebuggerStepThrough]
        public static void Failure(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Critical, applicationCategory, LoggingPriority.Failure,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
        }

    #endregion

    #region Log Error Methods

        [DebuggerStepThrough]
        public static void Error(Exception ex, string applicationCategory)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, true);
        }

        [DebuggerStepThrough]
        public static void Error(string message, string applicationCategory)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
        }

        [DebuggerStepThrough]
        public static void Error(Exception ex, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(1), EventId);
        }

        [DebuggerStepThrough]
        public static void Error(Exception ex, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, true, props);
        }

        [DebuggerStepThrough]
        public static void Error(string message, string applicationCategory, bool showStack)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, showStack);
        }

        [DebuggerStepThrough]
        public static void Error(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
        }

        [DebuggerStepThrough]
        public static void Error(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
        }

        [DebuggerStepThrough]
        public static void Error(Exception ex, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(1), EventId, props);
        }

        [DebuggerStepThrough]
        public static void Error(string message, string applicationCategory, bool showStack, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, showStack, props);
        }

        [DebuggerStepThrough]
        public static void Error(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Error, applicationCategory, LoggingPriority.Error,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
        }

    #endregion

    #region Log Warning Methods

        [DebuggerStepThrough]
        public static void Warning(Exception ex, string applicationCategory)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, true);
        }

        [DebuggerStepThrough]
        public static void Warning(string message, string applicationCategory)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
        }

        [DebuggerStepThrough]
        public static void Warning(Exception ex, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(1), EventId);
        }

        [DebuggerStepThrough]
        public static void Warning(Exception ex, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, true, props);
        }

        [DebuggerStepThrough]
        public static void Warning(string message, string applicationCategory, bool showStack)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, showStack);
        }

        [DebuggerStepThrough]
        public static void Warning(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
        }

        [DebuggerStepThrough]
        public static void Warning(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
        }

        [DebuggerStepThrough]
        public static void Warning(Exception ex, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(ex.Message + ex.StackTrace, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(1), EventId, props);
        }

        [DebuggerStepThrough]
        public static void Warning(string message, string applicationCategory, bool showStack, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, showStack, props);
        }

        [DebuggerStepThrough]
        public static void Warning(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Warning, applicationCategory, LoggingPriority.Warning,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
        }

    #endregion

    #region Log Info Methods

		#region Info

        [DebuggerStepThrough]
        public static long Info(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
		#region Info1

        [DebuggerStepThrough]
        public static long Info1(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info1(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info1(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info1(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info1(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info1(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info1(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info1(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
		#region Info2

        [DebuggerStepThrough]
        public static long Info2(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info2(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info2(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info2(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info2(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info2(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info2(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info2(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
		#region Info3

        [DebuggerStepThrough]
        public static long Info3(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info3(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info3(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info3(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info3(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info3(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info3(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info3(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
		#region Info4

        [DebuggerStepThrough]
        public static long Info4(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info4(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info4(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info4(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info4(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info4(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info4(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info4(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
		#region Info5

        [DebuggerStepThrough]
        public static long Info5(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info5(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info5(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info5(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info5(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info5(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info5(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Info5(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Information, applicationCategory, LoggingPriority.Info5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
	#endregion

    #region Log Debug Methods

	
		#region Debug

        [DebuggerStepThrough]
        public static long Debug(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
	
		#region Debug1

        [DebuggerStepThrough]
        public static long Debug1(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug1(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug1(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug1(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug1(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug1(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug1(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug1(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
	
		#region Debug2

        [DebuggerStepThrough]
        public static long Debug2(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug2(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug2(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug2(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug2(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug2(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug2(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug2(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
	
		#region Debug3

        [DebuggerStepThrough]
        public static long Debug3(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug3(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug3(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug3(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug3(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug3(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug3(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug3(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
	
		#region Debug4

        [DebuggerStepThrough]
        public static long Debug4(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug4(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug4(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug4(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug4(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug4(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug4(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug4(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion
	
		#region Debug5

        [DebuggerStepThrough]
        public static long Debug5(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug5(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug5(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug5(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug5(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug5(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug5(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Debug5(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Debug5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

		#endregion

    #endregion

	#region Trace Log Methods

		#region Trace

        [DebuggerStepThrough]
        public static long Trace(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace1

        [DebuggerStepThrough]
        public static long Trace1(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace1(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace1(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace1(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace1(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace1(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace1(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace1(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace1,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace2

        [DebuggerStepThrough]
        public static long Trace2(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace2(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace2(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace2(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace2(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace2(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace2(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace2(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace2,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace3

        [DebuggerStepThrough]
        public static long Trace3(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace3(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace3(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace3(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace3(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace3(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace3(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace3(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace3,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace4

        [DebuggerStepThrough]
        public static long Trace4(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace4(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace4(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace4(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace4(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace4(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace4(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace4(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace4,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace5

        [DebuggerStepThrough]
        public static long Trace5(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace5(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace5(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace5(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace5(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace5(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace5(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace5(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace5,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace6

        [DebuggerStepThrough]
        public static long Trace6(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace6,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace6(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace6,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace6(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace6,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace6(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace6,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace6(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace6,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace6(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace6,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace6(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace6,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace6(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace6,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace7

        [DebuggerStepThrough]
        public static long Trace7(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace7,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace7(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace7,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace7(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace7,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace7(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace7,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace7(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace7,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace7(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace7,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace7(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace7,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace7(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace7,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace8

        [DebuggerStepThrough]
        public static long Trace8(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace8,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace8(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace8,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace8(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace8,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace8(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace8,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace8(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace8,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace8(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace8,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace8(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace8,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace8(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace8,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace9

        [DebuggerStepThrough]
        public static long Trace9(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace9,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace9(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace9,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace9(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace9,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace9(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace9,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace9(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace9,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace9(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace9,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace9(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace9,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace9(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace9,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace10

        [DebuggerStepThrough]
        public static long Trace10(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace10,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace10(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace10,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace10(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace10,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace10(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace10,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace10(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace10,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace10(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace10,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace10(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace10,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace10(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace10,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace11

        [DebuggerStepThrough]
        public static long Trace11(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace11,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace11(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace11,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace11(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace11,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace11(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace11,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace11(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace11,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace11(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace11,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace11(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace11,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace11(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace11,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace12

        [DebuggerStepThrough]
        public static long Trace12(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace12,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace12(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace12,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace12(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace12,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace12(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace12,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace12(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace12,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace12(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace12,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace12(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace12,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace12(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace12,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace13

        [DebuggerStepThrough]
        public static long Trace13(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace13,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace13(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace13,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace13(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace13,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace13(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace13,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace13(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace13,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace13(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace13,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace13(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace13,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace13(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace13,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace14

        [DebuggerStepThrough]
        public static long Trace14(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace14,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace14(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace14,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace14(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace14,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace14(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace14,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace14(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace14,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace14(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace14,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace14(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace14,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace14(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace14,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace15

        [DebuggerStepThrough]
        public static long Trace15(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace15,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace15(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace15,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace15(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace15,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace15(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace15,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace15(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace15,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace15(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace15,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace15(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace15,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace15(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace15,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace16

        [DebuggerStepThrough]
        public static long Trace16(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace16,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace16(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace16,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace16(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace16,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace16(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace16,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace16(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace16,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace16(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace16,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace16(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace16,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace16(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace16,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace17

        [DebuggerStepThrough]
        public static long Trace17(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace17,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace17(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace17,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace17(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace17,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace17(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace17,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace17(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace17,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace17(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace17,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace17(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace17,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace17(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace17,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace18

        [DebuggerStepThrough]
        public static long Trace18(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace18,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace18(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace18,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace18(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace18,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace18(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace18,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace18(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace18,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace18(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace18,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace18(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace18,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace18(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace18,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace19

        [DebuggerStepThrough]
        public static long Trace19(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace19,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace19(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace19,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace19(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace19,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace19(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace19,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace19(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace19,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace19(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace19,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace19(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace19,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace19(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace19,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace20

        [DebuggerStepThrough]
        public static long Trace20(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace20,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace20(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace20,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace20(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace20,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace20(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace20,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace20(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace20,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace20(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace20,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace20(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace20,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace20(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace20,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace21

        [DebuggerStepThrough]
        public static long Trace21(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace21,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace21(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace21,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace21(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace21,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace21(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace21,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace21(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace21,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace21(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace21,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace21(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace21,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace21(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace21,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace22

        [DebuggerStepThrough]
        public static long Trace22(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace22,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace22(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace22,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace22(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace22,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace22(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace22,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace22(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace22,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace22(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace22,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace22(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace22,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace22(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace22,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace23

        [DebuggerStepThrough]
        public static long Trace23(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace23,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace23(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace23,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace23(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace23,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace23(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace23,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace23(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace23,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace23(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace23,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace23(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace23,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace23(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace23,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace24

        [DebuggerStepThrough]
        public static long Trace24(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace24,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace24(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace24,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace24(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace24,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace24(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace24,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace24(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace24,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace24(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace24,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace24(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace24,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace24(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace24,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace25

        [DebuggerStepThrough]
        public static long Trace25(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace25,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace25(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace25,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace25(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace25,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace25(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace25,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace25(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace25,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace25(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace25,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace25(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace25,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace25(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace25,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace26

        [DebuggerStepThrough]
        public static long Trace26(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace26,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace26(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace26,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace26(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace26,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace26(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace26,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace26(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace26,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace26(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace26,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace26(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace26,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace26(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace26,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace27

        [DebuggerStepThrough]
        public static long Trace27(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace27,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace27(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace27,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace27(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace27,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace27(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace27,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace27(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace27,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace27(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace27,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace27(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace27,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace27(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace27,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace28

        [DebuggerStepThrough]
        public static long Trace28(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace28,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace28(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace28,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace28(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace28,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace28(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace28,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace28(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace28,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace28(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace28,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace28(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace28,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace28(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace28,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace29

        [DebuggerStepThrough]
        public static long Trace29(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace29,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace29(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace29,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace29(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace29,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace29(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace29,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace29(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace29,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace29(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace29,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace29(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace29,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace29(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace29,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion

		#region Trace30

        [DebuggerStepThrough]
        public static long Trace30(string message, string applicationCategory, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace30,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace30(string message, string applicationCategory, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace30,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, false, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace30(string message, string applicationCategory, int EventId)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace30,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace30(string message, string applicationCategory, long startTicks, MethodBase method = null)
        {
            if (method == null) method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace30,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace30(string message, string applicationCategory, int EventId, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace30,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace30(string message, string applicationCategory, int EventId, long startTicks)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace30,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace30(string message, string applicationCategory, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace30,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        [DebuggerStepThrough]
        public static long Trace30(string message, string applicationCategory, int EventId, long startTicks, Dictionary<string, string> props)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            InternalWrite(message, TraceEventType.Verbose, applicationCategory, LoggingPriority.Trace30,
                method.ReflectedType.Name, method.Name, Assembly.GetCallingAssembly().GetName().Name, Convert.ToBoolean(0), EventId, startTicks, props);
            return Stopwatch.GetTimestamp();
        }

        #endregion


	#endregion

    public static double GetDuration(long startTicks)
    {
        return (double)(Stopwatch.GetTimestamp() - startTicks) / ((double)Stopwatch.Frequency);
    }

    public static double GetDuration(long startTicks, long endTicks)
    {
        return (double)(endTicks - startTicks) / ((double)Stopwatch.Frequency);
    }

    // TODO(crhodes)
    // Dojie put this as an nested class.  Might want to pull out and put above.  Go see how these are used.

	#region Helper Class

        public static class Helpers
        {
            [DebuggerStepThrough]
            public static T ExecuteLogHandledOp<T>(Func<T> action, Log.LoggingPriority loggingPriority, string applicationCategory, string additionalStartMessage = null, string additionalEndMessage = null)
            {
                //StackTrace trace = new StackTrace();
                MethodBase method = new StackFrame(1).GetMethod();
                var startMethod = typeof(Log).GetMethod(loggingPriority.ToString(), new Type[] { typeof(string), typeof(string), typeof(MethodBase) });
                long dbTicks = 0;
                if (startMethod != null)
                {
				    try
                    {
						dbTicks = (long)startMethod.Invoke(null, new object[] { string.Format("Enter: {0}", additionalStartMessage), applicationCategory, method });                   
                    }
                    catch (Exception ex)
                    {
						Log.Error(string.Format("Exception: {0}", additionalStartMessage), applicationCategory);
                        Log.Error(ex, applicationCategory);
                    }
                }

                var result = action();

                var endMethod = typeof(Log).GetMethod(loggingPriority.ToString(), new Type[] { typeof(string), typeof(string), typeof(long), typeof(MethodBase) });
                if (endMethod != null)
                {
				    try
                    {
						endMethod.Invoke(null, new object[] { string.Format("Exit: result:({0}) {1}", result, additionalEndMessage), applicationCategory, dbTicks, method });                                    
                    }
                    catch (Exception ex)
                    {
						Log.Error(string.Format("Exception: {0}", additionalEndMessage), applicationCategory);
                        Log.Error(ex, applicationCategory);
                    }
                }

                return result;
            }

			[DebuggerStepThrough]
            public static void ExecuteLogHandledOp(Action action, Log.LoggingPriority loggingPriority, string applicationCategory, string additionalStartMessage = null, string additionalEndMessage = null)
            {
                //StackTrace trace = new StackTrace();
                MethodBase method = new StackFrame(1).GetMethod();
                var startMethod = typeof(Log).GetMethod(loggingPriority.ToString(), new Type[] { typeof(string), typeof(string), typeof(MethodBase) });
                long dbTicks = 0;
                if (startMethod != null)
                {
                    try
                    {
						dbTicks = (long)startMethod.Invoke(null, new object[] { string.Format("Enter: {0}", additionalStartMessage), applicationCategory, method });                                   
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("Exception: {0}", additionalStartMessage), applicationCategory);
						Log.Error(ex, applicationCategory);
                    }
				}

                action();

                var endMethod = typeof(Log).GetMethod(loggingPriority.ToString(), new Type[] { typeof(string), typeof(string), typeof(long), typeof(MethodBase) });
                if (endMethod != null)
                {
					try
                    {
						endMethod.Invoke(null, new object[] { string.Format("Exit: {0}", additionalEndMessage), applicationCategory, dbTicks, method });
				    }
                    catch (Exception ex)
                    {
						Log.Error(string.Format("Exception: {0}", additionalEndMessage), applicationCategory);
						Log.Error(ex, applicationCategory);
					}
				}
            }


        }

        #endregion 

    }
}