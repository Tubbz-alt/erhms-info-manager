﻿using ERHMS.Utility;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ERHMS.EpiInfo.Communication
{
    public class Service : IService
    {
        public static IService Connect()
        {
            Log.Current.DebugFormat("Connecting to service: {0}", Settings.Instance.ServiceAddress);
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            EndpointAddress address = new EndpointAddress(Settings.Instance.ServiceAddress);
            ChannelFactory<IService> factory = new ChannelFactory<IService>(binding, address);
            IService service = factory.CreateChannel();
            try
            {
                service.Ping();
                return service;
            }
            catch
            {
                Log.Current.WarnFormat("Failed to connect to service: {0}", Settings.Instance.ServiceAddress);
                return null;
            }
        }

        public ServiceHost OpenHost()
        {
            Log.Current.DebugFormat("Opening service host: {0}", Settings.Instance.ServiceAddress);
            ServiceHost host = new ServiceHost(this, new Uri(Settings.Instance.ServiceAddress));
            ServiceBehaviorAttribute behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.InstanceContextMode = InstanceContextMode.Single;
#if DEBUG
            host.Description.Behaviors.Add(new ServiceMetadataBehavior());
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");
#endif
            host.AddServiceEndpoint(typeof(IService), new NetNamedPipeBinding(), "");
            try
            {
                host.Open();
                return host;
            }
            catch
            {
                Log.Current.WarnFormat("Failed to open service host: {0}", Settings.Instance.ServiceAddress);
                return null;
            }
        }

        public void Ping() { }

        private void OnEvent<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs e, string eventName) where TEventArgs : EventArgsBase
        {
            string message = string.Format("Invoking {0} service event", eventName);
            Log.Current.DebugFormat("{0}: {1}", message, e);
            if (handler == null)
            {
                return;
            }
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Log.Current.Warn(string.Format("{0} failed: {1}", message, e), ex);
            }
        }

        public event EventHandler<ViewEventArgs> ViewAdded;
        public void OnViewAdded(string projectPath, string viewName, string tag = null)
        {
            OnEvent(ViewAdded, new ViewEventArgs(projectPath, viewName, tag), "ViewAdded");
        }

        public event EventHandler<ViewEventArgs> ViewDataImported;
        public void OnViewDataImported(string projectPath, string viewName, string tag = null)
        {
            OnEvent(ViewDataImported, new ViewEventArgs(projectPath, viewName, tag), "ViewDataImported");
        }

        public event EventHandler<RecordEventArgs> RecordSaved;
        public void OnRecordSaved(string projectPath, string viewName, string globalRecordId, string tag = null)
        {
            OnEvent(RecordSaved, new RecordEventArgs(projectPath, viewName, globalRecordId, tag), "RecordSaved");
        }

        public event EventHandler<TemplateEventArgs> TemplateAdded;
        public void OnTemplateAdded(string templatePath, string tag = null)
        {
            OnEvent(TemplateAdded, new TemplateEventArgs(templatePath, tag), "TemplateAdded");
        }

        public event EventHandler<CanvasEventArgs> CanvasClosed;
        public void OnCanvasClosed(string projectPath, int canvasId, string canvasPath, string tag = null)
        {
            OnEvent(CanvasClosed, new CanvasEventArgs(projectPath, canvasId, canvasPath, tag), "CanvasClosed");
        }
    }
}