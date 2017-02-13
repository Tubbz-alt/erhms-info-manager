﻿using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public partial class Project : Epi.Project
    {
        public new const string FileExtension = ".prj";

        public static Project Create(ProjectCreationInfo creationInfo)
        {
            Log.Logger.DebugFormat("Creating project: {0}", creationInfo);
            Directory.CreateDirectory(creationInfo.Location);
            Project project = new Project
            {
                Id = Guid.NewGuid(),
                Version = Assembly.GetExecutingAssembly().GetName().Version,
                Name = creationInfo.Name,
                Description = creationInfo.Description,
                Location = creationInfo.Location,
                CollectedDataDriver = creationInfo.Driver,
                CollectedDataConnectionString = creationInfo.Builder.ConnectionString
            };
            project.CollectedDataDbInfo = new DbDriverInfo
            {
                DBCnnStringBuilder = creationInfo.Builder,
                DBName = creationInfo.DatabaseName
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, creationInfo.Driver, false);
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.Driver);
            if (creationInfo.Initialize)
            {
                Log.Logger.DebugFormat("Initializing project: {0}", project.FilePath);
                project.Metadata.CreateMetadataTables();
                project.Metadata.CreateCanvasesTable();
            }
            project.Save();
            return project;
        }

        private XmlElement XmlElement
        {
            get { return GetXmlDocument().DocumentElement; }
        }

        public Version Version
        {
            get
            {
                Version version;
                if (Version.TryParse(XmlElement.GetAttribute("erhmsVersion"), out version))
                {
                    return version;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                XmlElement.SetAttribute("erhmsVersion", value.ToString());
            }
        }

        public new MetadataDbProvider Metadata
        {
            get { return (MetadataDbProvider)base.Metadata; }
        }

        public IDbDriver Driver
        {
            get { return CollectedData.GetDbDriver(); }
        }

        private Project() { }

        public Project(string path)
            : base(path)
        {
            Log.Logger.DebugFormat("Opening project: {0}", path);
        }

        public bool IsValidViewName(string viewName, out InvalidViewNameReason reason)
        {
            if (ViewExtensions.IsValidName(viewName, out reason))
            {
                if (Views.Contains(viewName))
                {
                    reason = InvalidViewNameReason.ViewExists;
                    return false;
                }
                else if (Driver.TableExists(viewName))
                {
                    reason = InvalidViewNameReason.TableExists;
                    return false;
                }
                else
                {
                    reason = InvalidViewNameReason.None;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public string SuggestViewName(string viewName)
        {
            ICollection<string> tableNames = Driver.GetTableNames();
            return ViewExtensions.SanitizeName(viewName).MakeUnique("{0}_{1}", value =>
            {
                return Views.Contains(value) || tableNames.ContainsIgnoreCase(value);
            });
        }

        public override void Save()
        {
            Log.Logger.DebugFormat("Saving project: {0}", FilePath);
            base.Save();
        }
    }
}
