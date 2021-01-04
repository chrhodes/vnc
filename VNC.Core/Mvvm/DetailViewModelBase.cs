﻿using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Prism.Commands;
using Prism.Events;

using VNC.Core.Events;
using VNC.Core.Services;

namespace VNC.Core.Mvvm
{
    public abstract class DetailViewModelBase : EventViewModelBase, IDetailViewModel
    {
        private string _title;
        private int _id;

        private bool _hasChanges;

        public ICommand SaveCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand CloseDetailViewCommand { get; }

        public DetailViewModelBase(
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
#if LOGGING
            long startTicks = Log.CONSTRUCTOR("Enter", Common.LOG_APPNAME);
#endif
            SaveCommand = new DelegateCommand(
                SaveExecute, SaveCanExecute);

            DeleteCommand = new DelegateCommand(
                DeleteExecute, DeleteCanExecute);

            CloseDetailViewCommand = new DelegateCommand(
                CloseDetailViewExecute);
#if LOGGING
            Log.CONSTRUCTOR("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }

        protected virtual void PublishAfterCollectionSavedEvent()
        {
#if LOGGING
            long startTicks = Log.EVENT("Enter", Common.LOG_APPNAME);
#endif

            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Publish(new AfterCollectionSavedEventArgs
                {
                    ViewModelName = this.GetType().Name
                });
#if LOGGING
            Log.EVENT("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }

        protected virtual void CloseDetailViewExecute()
        {
#if LOGGING
            long startTicks = Log.VIEWMODEL("Enter", Common.LOG_APPNAME);
#endif

            if (HasChanges)
            {
                var result = MessageDialogService.ShowOkCancelDialog(
                    "You've made changes.  Close this item?", "Question");

                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            PublishAfterDetailClosedEvent();
#if LOGGING
            Log.VIEWMODEL("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }

        private void PublishAfterDetailClosedEvent()
        {
#if LOGGING
            long startTicks = Log.EVENT("Enter", Common.LOG_APPNAME);
#endif

            EventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Publish(new AfterDetailClosedEventArgs
                {
                    Id = this.Id,
                    ViewModelName = this.GetType().Name
                });
#if LOGGING
            Log.EVENT("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }

        public int Id
        {
            get { return _id; }
            protected set
            {
                if (_id == value)
                    return;
                _id = value;
                OnPropertyChanged();
            }
        }
     
        public string Title
        {
            get { return _title; }
            protected set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public abstract Task LoadAsync(int id);

        protected abstract bool DeleteCanExecute();

        protected abstract void DeleteExecute();

        protected abstract bool SaveCanExecute();

        protected abstract void SaveExecute();

        protected virtual void PublishAfterDetailDeletedEvent(int modelId)
        {
#if LOGGING
            long startTicks = Log.EVENT("Enter", Common.LOG_APPNAME);
#endif

            EventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Publish
                (
                    new AfterDetailDeletedEventArgs
                    {
                        Id = modelId,
                        ViewModelName = this.GetType().Name
                    }
                );
#if LOGGING
            Log.EVENT("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }

        protected virtual void PublishAfterDetailSavedEvent(int modelId, string displayMember)
        {
#if LOGGING
            long startTicks = Log.EVENT("Enter", Common.LOG_APPNAME);
#endif

            EventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Publish
                (
                    new AfterDetailSavedEventArgs
                    {
                        Id = modelId,
                        DisplayMember = displayMember,
                        ViewModelName = this.GetType().Name
                    }
                );
#if LOGGING
            Log.EVENT("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }

        protected virtual async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc, Action afterSaveAction)
        {
#if LOGGING
            Int64 startTicks = Log.VIEWMODEL("Enter", Common.LOG_APPNAME);
#endif

            try
            {
                await saveFunc();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var databaseValues = ex.Entries.Single().GetDatabaseValues();

                if (databaseValues == null)
                {
                    MessageDialogService.ShowInfoDialog(
                        "The entity has been deleted by another user.  Cannot continue.");
                    PublishAfterDetailDeletedEvent(Id);
                    return;
                }

                var result = MessageDialogService.ShowOkCancelDialog(
                    "The entity has been changed by someone else." +
                    " Click OK to save your changes anyway; Click Cancel" +
                    " to reload the entity from the database.", "Question");

                if (result == MessageDialogResult.OK)   // Client Wins
                {
                    // Update the original values with database-values
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    await saveFunc();
                }
                else  // Database Wins
                {
                    // Reload entity from database
                    await ex.Entries.Single().ReloadAsync();
                    await LoadAsync(Id);
                }
            }

            // Do anything that needs to occur after saving

            afterSaveAction();
#if LOGGING
            Log.VIEWMODEL("Exit", Common.LOG_APPNAME, startTicks);
#endif
        }
    }
}
