﻿using ERHMS.EpiInfo;
using ERHMS.Utility;
using System.Reflection;

namespace ERHMS.DataAccess
{
    public static class ViewExtensions
    {
        public static TemplateInfo CreateNewTemplate()
        {
            string path = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.DataAccess.Resources.View.xml", path);
            return TemplateInfo.Get(path);
        }
    }
}
