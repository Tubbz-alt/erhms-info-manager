﻿using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class IncidentNote : TableEntity
    {
        public override string Guid
        {
            get
            {
                return GetProperty<string>("IncidentNoteId");
            }
            set
            {
                if (!SetProperty("IncidentNoteId", value))
                {
                    return;
                }
                OnPropertyChanged("Guid");
            }
        }

        public string IncidentNoteId
        {
            get { return GetProperty<string>("IncidentNoteId"); }
            set { SetProperty("IncidentNoteId", value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }

        public string Content
        {
            get { return GetProperty<string>("Content"); }
            set { SetProperty("Content", value); }
        }

        public DateTime Date
        {
            get { return GetProperty<DateTime>("Date"); }
            set { SetProperty("Date", value); }
        }
    }
}