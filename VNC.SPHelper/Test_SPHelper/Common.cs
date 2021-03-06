﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Test_SPHelper
{
    ///<summary>
    ///Common items declared at the Class level.
    ///</summary>
    ///<remarks>
    ///Use this class for any thing you want globally available.
    ///Place only Static items in this class.  This Class cannot not be instantiated.
    ///</remarks>  
    ///
    public static class Common
    {
        new public const string PROJECT_NAME = "Test_SPHelper";

        public const string cCONFIG_FILE = @"C:\temp\SupportTools_Config.xml";


        const string TYPE_NAME = "Common";

        // These values are added to the dimensions of a hosting window if the
        // hostee User_Control specifies values for MinWidth/MinHeight.
        // They have not been thought through but do seem to "work".

        internal const int WINDOW_HOSTING_USER_CONTROL_WIDTH_PAD = 30;
        internal const int WINDOW_HOSTING_USER_CONTROL_HEIGHT_PAD = 75;

        public static void Initialize()
        {
#if TRACE
            //Common.WriteToDebugWindow(string.Format("{0}:{1}()", TYPE_NAME, System.Reflection.MethodInfo.GetCurrentMethod().Name));
#endif

        }

        public static IPrincipal CurrentUser
        {
            get;
            set;
        }

        #region Core

        // TODO: Add as many DebugLevels as needed.
        // Add accompanying checkboxes on frmDebugWindow

        public static bool DebugLevel1
        {
            get;
            set;
        }
        public static bool DebugLevel2
        {
            get;
            set;
        }
        public static bool DebugSQL
        {
            get;
            set;
        }

        public static event EventHandler AutoHideGroupSpeedChanged;

        private static int _AutoHideGroupSpeed = 250;

        public static int AutoHideGroupSpeed
        {
            get { return _AutoHideGroupSpeed; }
            set
            {
                _AutoHideGroupSpeed = value;

                EventHandler evt = AutoHideGroupSpeedChanged;

                if (evt != null)
                {
                    evt(null, EventArgs.Empty); ;
                }
            }
        }

        //public static void RaiseAutoHideGroupSpeedChanged()
        //{
        //    EventHandler evt = AutoHideGroupSpeedChanged;

        //    if (evt != null)
        //    {
        //        evt(null, EventArgs.Empty); ;
        //    }
        //}

        /// <summary>
        /// Indicates whether the UI is running in DeveloperMode
        /// </summary>
        public static bool DeveloperMode
        {
            get;
            set;
        }
        /// <summary>
        /// Indicates whether the UI is running in DebugMode
        /// </summary>
        public static bool DebugMode
        {
            get;
            set;
        }

        //private static OutputWindow _outputWindow;

        //public static OutputWindow OutputWindow
        //{
        //    get
        //    {
        //        if(_outputWindow == null)
        //        {
        //            _outputWindow = new OutputWindow();
        //        }
        //        return _outputWindow;
        //    }
        //    set
        //    {
        //        _outputWindow = value;
        //    }
        //}

        //public static long WriteToDebugWindow(string message)
        //{
        //    return OutputWindow.Write(message);
        //}

        //public static long WriteToDebugWindow(string message, long startTime)
        //{
        //    return OutputWindow.Write(message, startTime);
        //}

        #endregion

        // This controls the behavior of the overall application.
        // It is intialized from app.config and is updated when the user changes the mode.
        // Changes are reflected in the app.config file.

        public static bool IsAdministrator { get; set; }
        public static bool IsBetaUser { get; set; }
        public static bool IsDeveloper { get; set; }
        //public static bool IsAdvancedUser { get; set; }

        public static bool AllowEditing { get; set; }

        public static string RowDetailMode { get; set; }

    }
}
