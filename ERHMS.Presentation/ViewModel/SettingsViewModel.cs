﻿using ERHMS.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace ERHMS.Presentation.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the StartViewModel class.
        /// </summary>
        private string smtpHost;
        public string SmtpHost
        {
            get { return smtpHost; }
            set { Set(() => SmtpHost, ref smtpHost, value); }
        }
        private int smtpPort;
        public int SmtpPort
        {
            get { return smtpPort; }
            set { Set(() => SmtpPort, ref smtpPort, value); }
        }
        private string emailSender;
        public string EmailSender
        {
            get { return emailSender; }
            set { Set(() => EmailSender, ref emailSender, value); }
        }
        private string bingMapsLicenseKey;
        public string BingMapsLicenseKey
        {
            get { return bingMapsLicenseKey; }
            set { Set(() => BingMapsLicenseKey, ref bingMapsLicenseKey, value); }
        }
        public RelayCommand SaveCommand { get; private set; }

        public SettingsViewModel()
        {
            SmtpHost = Settings.Default.EmailHost;
            smtpPort = Settings.Default.EmailPort;
            EmailSender = Settings.Default.EmailFromAddress;
            BingMapsLicenseKey = Settings.Default.MapLicenseKey;

            SaveCommand = new RelayCommand(() =>
            {
                try
                {
                    Settings.Default.EmailHost = SmtpHost;
                    Settings.Default.EmailPort = SmtpPort;
                    Settings.Default.EmailFromAddress = EmailSender;
                    Settings.Default.MapLicenseKey = BingMapsLicenseKey;

                    Settings.Default.Save();

                    Messenger.Default.Send(new NotificationMessage<string>("Application settings were successfully saved.", "ShowSuccessMessage"));
                }
                catch (Exception)
                {
                    Messenger.Default.Send(new NotificationMessage<string>("An error occurred while trying to save the settings.", "ShowErrorMessage"));
                }
            });
        }
    }
}