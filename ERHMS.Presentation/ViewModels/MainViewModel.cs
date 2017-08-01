﻿using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Presentation.ViewModels
{
    public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase, IDocumentManager
    {
        public IServiceManager Services { get; private set; }

        private string title;
        public string Title
        {
            get { return title; }
            private set { Set(nameof(Title), ref title, value); }
        }

        public ObservableCollection<ViewModelBase> Documents { get; private set; }

        private ViewModelBase activeDocument;
        public ViewModelBase ActiveDocument
        {
            get
            {
                return activeDocument;
            }
            set
            {
                if (Set(nameof(ActiveDocument), ref activeDocument, value))
                {
                    if (value != null)
                    {
                        Log.Logger.DebugFormat("Activating tab: {0}", value);
                    }
                    CloseCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DataContext Context
        {
            get
            {
                return Services.Context;
            }
            private set
            {
                Services.Context = value;
                ShowRespondersCommand.RaiseCanExecuteChanged();
                ShowNewResponderCommand.RaiseCanExecuteChanged();
                ShowIncidentsCommand.RaiseCanExecuteChanged();
                ShowNewIncidentCommand.RaiseCanExecuteChanged();
                ShowViewsCommand.RaiseCanExecuteChanged();
                ShowTemplatesCommand.RaiseCanExecuteChanged();
                ShowAssignmentsCommand.RaiseCanExecuteChanged();
                ShowPgmsCommand.RaiseCanExecuteChanged();
                ShowCanvasesCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand ShowDataSourcesCommand { get; private set; }
        public RelayCommand ShowRespondersCommand { get; private set; }
        public RelayCommand ShowNewResponderCommand { get; private set; }
        public RelayCommand ShowIncidentsCommand { get; private set; }
        public RelayCommand ShowNewIncidentCommand { get; private set; }
        public RelayCommand ShowViewsCommand { get; private set; }
        public RelayCommand ShowTemplatesCommand { get; private set; }
        public RelayCommand ShowAssignmentsCommand { get; private set; }
        public RelayCommand ShowPgmsCommand { get; private set; }
        public RelayCommand ShowCanvasesCommand { get; private set; }
        public RelayCommand ShowSettingsCommand { get; private set; }
        public RelayCommand ShowLogsCommand { get; private set; }
        public RelayCommand ShowHelpCommand { get; private set; }
        public RelayCommand ShowAboutCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public MainViewModel(IServiceManager services)
        {
            Services = services;
            Title = App.Title;
            Documents = new ObservableCollection<ViewModelBase>();
            ShowDataSourcesCommand = new RelayCommand(ShowDataSources);
            ShowRespondersCommand = new RelayCommand(ShowResponders, HasContext);
            ShowNewResponderCommand = new RelayCommand(ShowNewResponder, HasContext);
            ShowIncidentsCommand = new RelayCommand(ShowIncidents, HasContext);
            ShowNewIncidentCommand = new RelayCommand(ShowNewIncident, HasContext);
            ShowViewsCommand = new RelayCommand(ShowViews, HasContext);
            ShowTemplatesCommand = new RelayCommand(ShowTemplates, HasContext);
            ShowAssignmentsCommand = new RelayCommand(ShowAssignments, HasContext);
            ShowPgmsCommand = new RelayCommand(ShowPgms, HasContext);
            ShowCanvasesCommand = new RelayCommand(ShowCanvases, HasContext);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ShowLogsCommand = new RelayCommand(ShowLogs);
            ShowHelpCommand = new RelayCommand(ShowHelp);
            ShowAboutCommand = new RelayCommand(ShowAbout);
            CloseCommand = new RelayCommand(Close, HasActiveDocument);
            ExitCommand = new RelayCommand(Exit);
        }

        public bool HasActiveDocument()
        {
            return ActiveDocument != null;
        }

        public bool HasContext()
        {
            return Context != null;
        }

        private void OpenDataSourceInternal(string path)
        {
            try
            {
                foreach (ViewModelBase document in Documents.ToList())
                {
                    document.Close(false);
                }
                using (new WaitCursor())
                {
                    Context = new DataContext(new Project(path));
                }
                Title = string.Format("{0} - {1}", App.Title, Context.Project.Name);
                ShowHelp();
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to open data source", ex);
                Services.Dialogs.ShowErrorAsync("Failed to open data source.", ex);
                ShowDataSources();
            }
        }

        public void OpenDataSource(string path)
        {
            if (HasContext())
            {
                if (Context.Project.FilePath.EqualsIgnoreCase(path))
                {
                    Get<DataSourceListViewModel>()?.Close();
                    return;
                }
                ConfirmMessage msg = new ConfirmMessage
                {
                    Verb = "Open",
                    Message = "Open data source? This will close the current data source."
                };
                msg.Confirmed += (sender, e) =>
                {
                    OpenDataSourceInternal(path);
                };
                MessengerInstance.Send(msg);
            }
            else
            {
                OpenDataSourceInternal(path);
            }
        }

        private TViewModel Get<TViewModel>(Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase
        {
            foreach (TViewModel document in Documents.OfType<TViewModel>())
            {
                if (predicate == null || predicate(document))
                {
                    return document;
                }
            }
            return null;
        }

        private bool Activate<TViewModel>(Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase
        {
            TViewModel document = Get(predicate);
            if (document == null)
            {
                return false;
            }
            else
            {
                ActiveDocument = document;
                return true;
            }
        }

        private void Open(ViewModelBase document)
        {
            Log.Logger.DebugFormat("Opening tab: {0}", document);
            document.Closed += (sender, e) =>
            {
                Close(document);
            };
            Documents.Add(document);
            ActiveDocument = document;
        }

        public void Show<TViewModel>(Func<TViewModel> constructor, Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase
        {
            if (!Activate(predicate))
            {
                using (new WaitCursor())
                {
                    Open(constructor());
                }
            }
        }

        public void ShowDataSources()
        {
            Show(() => new DataSourceListViewModel(Services));
        }

        public void ShowResponders()
        {
            Show(() => new ResponderListViewModel(Services));
        }

        public void ShowResponder(Responder responder)
        {
            Show(
                () => new ResponderDetailViewModel(Services, responder),
                document => document.Responder.ResponderId.EqualsIgnoreCase(responder.ResponderId));
        }

        public void ShowNewResponder()
        {
            ShowResponder(new Responder());
        }

        public void ShowIncidents()
        {
            Show(() => new IncidentListViewModel(Services));
        }

        public void ShowIncident(Incident incident)
        {
            Show(
                () => new IncidentViewModel(Services, incident),
                document => document.Incident.IncidentId.EqualsIgnoreCase(incident.IncidentId));
        }

        public void ShowNewIncident()
        {
            ShowIncident(new Incident());
        }

        public void ShowLocation(Location location)
        {
            Show(
                () => new LocationDetailViewModel(Services, location),
                document => document.Location.LocationId.EqualsIgnoreCase(location.LocationId));
        }

        public void ShowViews()
        {
            Show(() => new ViewListViewModel(Services, null));
        }

        public void ShowRecords(Epi.View view)
        {
            Show(
                () => new RecordListViewModel(Services, view),
                document => document.Entities.View.Id == view.Id);
        }

        public void ShowTemplates()
        {
            Show(() => new TemplateListViewModel(Services, null));
        }

        public void ShowAssignments()
        {
            Show(() => new AssignmentListViewModel(Services, null));
        }

        public void ShowPgms()
        {
            Show(() => new PgmListViewModel(Services, null));
        }

        public void ShowCanvases()
        {
            Show(() => new CanvasListViewModel(Services, null));
        }

        public void ShowSettings()
        {
            Show(() => new SettingsViewModel(Services));
        }

        public async void ShowSettings(string message, Exception exception = null)
        {
            if (exception == null)
            {
                AlertMessage msg = new AlertMessage
                {
                    Message = message
                };
                msg.Dismissed += (sender, e) =>
                {
                    ShowSettings();
                };
                MessengerInstance.Send(msg);
            }
            else
            {
                await Services.Dialogs.ShowErrorAsync(message, exception);
                ShowSettings();
            }
        }

        public void ShowLogs()
        {
            Show(() => new LogListViewModel(Services));
        }

        public void ShowHelp()
        {
            Show(() => new HelpViewModel(Services));
        }

        public void ShowAbout()
        {
            Show(() => new AboutViewModel(Services));
        }

        private void Close(ViewModelBase document)
        {
            Log.Logger.DebugFormat("Closing tab: {0}", document);
            Documents.Remove(document);
            if (Documents.Count == 0)
            {
                ActiveDocument = null;
            }
        }

        public void Close()
        {
            ActiveDocument.Close();
        }

        public void Exit()
        {
            MessengerInstance.Send(new ExitMessage());
        }
    }
}
