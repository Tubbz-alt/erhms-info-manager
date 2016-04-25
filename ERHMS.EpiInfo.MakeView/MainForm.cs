﻿using Epi.Windows.MakeView.Forms;
using Epi.Windows.MakeView.PresentationLogic;
using ERHMS.Utility;
using System;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.MakeView
{
    internal class MainForm : MakeViewMainForm
    {
        public GuiMediator Mediator
        {
            get { return mediator; }
        }

        public ProjectExplorer ProjectExplorer
        {
            get { return projectExplorer; }
        }

        public MainForm(bool visible = true)
        {
            if (!visible)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
        }

        protected override object GetService(Type serviceType)
        {
            if (serviceType == typeof(MakeViewMainForm))
            {
                return this;
            }
            else
            {
                return base.GetService(serviceType);
            }
        }

        public void OpenProject(Project project)
        {
            if (ProjectExplorer.IsProjectLoaded)
            {
                ReflectionExtensions.Invoke(this, typeof(MakeViewMainForm), "CloseCurrentProject", Type.EmptyTypes, null);
            }
            ReflectionExtensions.Invoke(this, typeof(MakeViewMainForm), "OpenProject", new Type[] { typeof(Epi.Project) }, new object[] { project });
        }

        public void OpenProject(string projectPath)
        {
            OpenProject(new Project(projectPath));
        }
    }
}