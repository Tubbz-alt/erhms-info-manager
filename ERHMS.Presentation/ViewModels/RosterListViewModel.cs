﻿using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class RosterListViewModel : ViewModelBase
    {
        public class RosterListByStatusViewModel : ListViewModelBase<RosterViewModel>
        {
            public RosterListViewModel Parent { get; private set; }
            public bool Rostered { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public RosterListByStatusViewModel(RosterListViewModel parent, bool rostered)
            {
                Parent = parent;
                Rostered = rostered;
                Selecting += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
            }

            protected override ICollectionView GetItems()
            {
                ICollection<RosterViewModel> items = new List<RosterViewModel>();
                if (Rostered)
                {
                    foreach (Roster roster in Parent.Rosters)
                    {
                        Responder responder = Parent.Responders.SingleOrDefault(_responder => _responder.ResponderId.Equals(roster.ResponderId, StringComparison.OrdinalIgnoreCase));
                        if (responder != null)
                        {
                            items.Add(new RosterViewModel(roster, responder));
                        }
                    }
                }
                else
                {
                    foreach (Responder responder in Parent.Responders)
                    {
                        Roster roster = Parent.Rosters.SingleOrDefault(_roster => _roster.ResponderId.Equals(responder.ResponderId, StringComparison.OrdinalIgnoreCase));
                        if (roster == null)
                        {
                            items.Add(new RosterViewModel(null, responder));
                        }
                    }
                }
                return CollectionViewSource.GetDefaultView(items.OrderBy(roster => roster.Responder.LastName).ThenBy(roster => roster.Responder.FirstName));
            }

            protected override IEnumerable<string> GetFilteredValues(RosterViewModel item)
            {
                return RosterListViewModel.GetFilteredValues(item);
            }

            public void Edit()
            {
                Locator.Main.OpenResponderDetailView(SelectedItem.Responder);
            }
        }

        private static IEnumerable<string> GetFilteredValues(RosterViewModel item)
        {
            yield return item.Responder.ResponderId;
            yield return item.Responder.LastName;
            yield return item.Responder.FirstName;
            yield return item.Responder.EmailAddress;
            yield return item.Responder.City;
            yield return item.Responder.State;
            yield return item.Responder.OrganizationName;
            yield return item.Responder.Occupation;
        }

        public Incident Incident { get; private set; }
        private ICollection<Responder> Responders { get; set; }
        private ICollection<Roster> Rosters { get; set; }
        public RosterListByStatusViewModel UnrosteredResponders { get; private set; }
        public RosterListByStatusViewModel RosteredResponders { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public RosterListViewModel(Incident incident)
        {
            Incident = incident;
            UpdateTitle();
            UnrosteredResponders = new RosterListByStatusViewModel(this, false);
            RosteredResponders = new RosterListByStatusViewModel(this, true);
            UnrosteredResponders.Selecting += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            RosteredResponders.Selecting += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
            };
            Refresh();
            AddCommand = new RelayCommand(Add, HasSelectedUnrosteredResponder);
            RemoveCommand = new RelayCommand(Remove, HasSelectedRosteredResponder);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, OnRefreshIncidentMessage);
            Messenger.Default.Register<RefreshListMessage<Responder>>(this, OnRefreshResponderListMessage);
        }

        private void UpdateTitle()
        {
            string incidentName = Incident.New ? "New Incident" : Incident.Name;
            Title = string.Format("{0} Roster", incidentName).Trim();
        }

        public bool HasSelectedUnrosteredResponder()
        {
            return UnrosteredResponders.HasSelectedItem();
        }

        public bool HasSelectedRosteredResponder()
        {
            return RosteredResponders.HasSelectedItem();
        }

        public void Add()
        {
            foreach (RosterViewModel roster in UnrosteredResponders.SelectedItems)
            {
                Roster _roster = DataContext.Rosters.Create();
                _roster.ResponderId = roster.Responder.ResponderId;
                _roster.IncidentId = Incident.IncidentId;
                DataContext.Rosters.Save(_roster);
            }
            Refresh();
        }

        public void Remove()
        {
            foreach (RosterViewModel roster in RosteredResponders.SelectedItems)
            {
                DataContext.Rosters.Delete(roster.Roster);
            }
            Refresh();
        }

        public void Refresh()
        {
            Responders = DataContext.Responders.SelectByDeleted(false).ToList();
            Rosters = DataContext.Rosters.SelectByIncident(Incident.IncidentId).ToList();
            UnrosteredResponders.Refresh();
            RosteredResponders.Refresh();
        }

        private void OnRefreshIncidentMessage(RefreshMessage<Incident> msg)
        {
            if (msg.Entity == Incident)
            {
                UpdateTitle();
            }
        }

        private void OnRefreshResponderListMessage(RefreshListMessage<Responder> msg)
        {
            Refresh();
        }
    }
}
