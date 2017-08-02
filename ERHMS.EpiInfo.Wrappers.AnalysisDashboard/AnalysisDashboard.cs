﻿using Epi.Windows;
using ERHMS.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    public class AnalysisDashboard
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Wrapper.MainBase(args);
        }

        public class OpenCanvas : Wrapper
        {
            private static Project Project { get; set; }
            private static Canvas Canvas { get; set; }
            private static string CanvasPath { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, int canvasId, string content)
            {
                string canvasPath = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", Canvas.FileExtension);
                File.WriteAllText(canvasPath, content);
                return Create(() => MainInternal(projectPath, canvasId, canvasPath));
            }

            private static void MainInternal(string projectPath, int canvasId, string canvasPath)
            {
                Project = new Project(projectPath);
                Canvas = Project.GetCanvasById(canvasId);
                CanvasPath = canvasPath;
                using (FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(canvasPath), Path.GetFileName(canvasPath)))
                {
                    watcher.NotifyFilter = NotifyFilters.LastWrite;
                    watcher.Changed += Watcher_Changed;
                    watcher.EnableRaisingEvents = true;
                    Form = new MainForm
                    {
                        WindowState = FormWindowState.Minimized,
                        ShowInTaskbar = false
                    };
                    Form.Shown += Form_Shown;
                    Application.Run(Form);
                }
            }

            private static void Watcher_Changed(object sender, FileSystemEventArgs e)
            {
                using (FileStream stream = new FileStream(CanvasPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(stream))
                {
                    Canvas.Content = reader.ReadToEnd();
                }
                Project.UpdateCanvas(Canvas);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                using (SplashScreenForm splash = new SplashScreenForm())
                {
                    splash.ShowInTaskbar = false;
                    splash.Show(Form);
                    Application.DoEvents();
                    Form.BringToFront();
                    Form.OpenCanvas(CanvasPath);
                    splash.Close();
                    Form.WindowState = FormWindowState.Maximized;
                    Form.ShowInTaskbar = true;
                }
            }
        }
    }
}
