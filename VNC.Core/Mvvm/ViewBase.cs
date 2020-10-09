﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VNC.Core.Mvvm
{
    public class ViewBase : UserControl, IView
    {

        //public ViewBase() : base()
        //{

        //}

        //public ViewBase(IViewModel viewModel) : base()
        //{
        //    ViewModel = viewModel;
        //}

        private IViewModel _viewModel;

        public IViewModel ViewModel
        {
            get { return _viewModel; }

            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }
    }
}
