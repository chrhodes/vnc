﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

namespace VNC.SP.User_Interface.User_Controls
{
    public partial class wucSPSiteCollection_Picker : UserControl
    {

        #region Constructors and Load

        public wucSPSiteCollection_Picker()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties and Fields

        // TODO:
        //  Add properties, backing fields, and dependency properties to match the attributes from each XML element
        //  Convention is _<Attribute Name>, <AttributeName>, <AttributeName>DP
        //  where <AttributeName> is the name of the attribute for the <Element>

        public string Domain
        {
            get { return teDomain.Text; }
        }

        public string Username
        {
            get { return teUsername.Text; }
        }

        public string Password
        {
            get { return tePassword.Text; }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }

        public string NameDP
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(NameDPProperty);
            }
            set
            {
                SetValue(NameDPProperty, value);
            }
        }

        // TODO:
        //  Update the parameters to .Register(...) as needed

        public static readonly DependencyProperty NameDPProperty = DependencyProperty.Register(
            "NameDP", typeof(string), typeof(wucSPSiteCollection_Picker), new PropertyMetadata(null, new PropertyChangedCallback(OnNameDPChanged)));

        private string _Uri;

        public string Uri
        {
            get { return _Uri; }
            set
            {
                _Uri = value;
            }
        }

        public string UriDP
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(UriDPProperty);
            }
            set
            {
                SetValue(UriDPProperty, value);
            }
        }

        // TODO:
        //  Update the parameters to .Register(...) as needed

        public static readonly DependencyProperty UriDPProperty = DependencyProperty.Register(
            "UriDP", typeof(string), typeof(wucSPSiteCollection_Picker), new PropertyMetadata(null, new PropertyChangedCallback(OnUriDPChanged)));

                
        #endregion

        #region Delegates and Events

        public delegate void ControlEvent();
        public event ControlEvent ControlChanged;

        #endregion

        #region Event Handlers

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var v = sender;
            var item = ((System.Windows.Controls.ComboBox)v).SelectedItem;

            if (null == item)
            {
                // May have just opened new file and no item has been selected.
                return;
            }

            _Name = ((System.Xml.XmlElement)item).Attributes["Name"].Value;
            NameDP = ((System.Xml.XmlElement)item).Attributes["Name"].Value;

            _Uri = ((System.Xml.XmlElement)item).Attributes["Uri"].Value;
            UriDP = ((System.Xml.XmlElement)item).Attributes["Uri"].Value;

            ControlEvent fireEvent = Interlocked.CompareExchange(ref ControlChanged, null, null);

            if (null != fireEvent)
            {
                fireEvent();
            }
        }

        protected virtual void OnNameDPChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        private static void OnNameDPChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            wucSPSiteCollection_Picker picker = o as wucSPSiteCollection_Picker;
            if (picker != null)
                picker.OnNameDPChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnUriDPChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        private static void OnUriDPChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            wucSPSiteCollection_Picker picker = o as wucSPSiteCollection_Picker;
            if (picker != null)
                picker.OnUriDPChanged((string)e.OldValue, (string)e.NewValue);
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            LoadDataFromFile();
        }

        #endregion

        #region Main Methods

        public void PopulateControlFromFile(string fileNameAndPath)
        {
            comboBox.Source = new Uri(fileNameAndPath);
        }

        #endregion

        #region Private Methods

        private void LoadDataFromFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FileName = "";

            if (true == openFileDialog1.ShowDialog())
            {
                string fileName = openFileDialog1.FileName;

                PopulateControlFromFile(fileName);
            }
        }

        #endregion

    }
}
