﻿using System;
using System.Windows.Input;

using Prism.Commands;
using Prism.Events;

using VNC.Core.Events;
using VNC.Core.Services;

namespace VNC.Core.Mvvm
{
    public class NavigationItemViewModel : EventViewModelBase
    {
        private string _displayMember;
        private string _detailViewModelName;

        // TODO(crhodes)
        // Decide if we will every need the messageDialogService.
        // This was not in Claudius.

        public NavigationItemViewModel(
            int id,
            string displayMember,
            string detailViewModelName,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
#if LOGGING
            Int64 startTicks = Log.CONSTRUCTOR($"Enter id:({id}) displayMember:({displayMember}) detailViewModelName:({detailViewModelName})", Common.LOG_APPNAME);
#endif
            Id = id;
            DisplayMember = displayMember;
            _detailViewModelName = detailViewModelName;

            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
#if LOGGING
            Log.CONSTRUCTOR("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }

        public int Id { get; set; }

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                if (_displayMember == value)
                    return;
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenDetailViewCommand { get; }

        // TODO(crhodes)
        // Maybe this should be protected virtual

        private void OnOpenDetailViewExecute()
        {
#if LOGGING
            Int64 startTicks = Log.EVENT_HANDLER("Enter", Common.LOG_APPNAME);
#endif

            PublishOpenDetailViewEvent();

#if LOGGING
            Log.EVENT_HANDLER("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }

        private void PublishOpenDetailViewEvent()
        {
#if LOGGING
            Int64 startTicks = Log.EVENT($"Enter Id:({Id})", Common.LOG_APPNAME);
#endif

            EventAggregator.GetEvent<OpenDetailViewEvent>()
              .Publish
                (
                    new OpenDetailViewEventArgs
                    {
                        Id = Id,
                        ViewModelName = _detailViewModelName
                    }
                );

#if LOGGING
            Log.EVENT("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }
    }
}

