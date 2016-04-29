﻿using ERHMS.Domain;
using System.ComponentModel;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentViewModel : DocumentViewModel
    {
        public Incident Incident { get; private set; }
        public IncidentDetailViewModel Detail { get; private set; }
        public IncidentNotesViewModel Notes { get; private set; }
        public LocationListViewModel Locations { get; private set; }

        public IncidentViewModel(Incident incident)
        {
            Incident = incident;
            incident.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "New")
                {
                    UpdateTitle();
                }
            };
            UpdateTitle();
            Detail = new IncidentDetailViewModel(incident);
            Notes = new IncidentNotesViewModel(incident);
            Locations = new LocationListViewModel(incident);
        }

        private void UpdateTitle()
        {
            if (Incident.New)
            {
                Title = "New Incident";
            }
            else
            {
                Title = Incident.Name;
            }
        }
    }
}
