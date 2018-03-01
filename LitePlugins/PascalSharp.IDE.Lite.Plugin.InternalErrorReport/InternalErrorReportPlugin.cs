﻿using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory.Parser;
using PascalSharp.Compiler;
using PascalSharp.Internal.Errors;

namespace VisualPascalABCPlugins
{
    public class InternalErrorReport_VisualPascalABCPlugin : IVisualPascalABCPlugin
    {
        public static string StringsPrefix = "VPP_IER_";
        IVisualEnvironmentCompiler VisualEnvironmentCompiler;
        private CompilerInternalErrorReport CompilerInternalErrorReport = new CompilerInternalErrorReport();
        private ErrorReport ErrorReport = new ErrorReport();
        private List<string> FileNames = new List<string>();
        private string ReportText=null;
        private string States = "";

        public InternalErrorReport_VisualPascalABCPlugin(IWorkbench Workbench)
        {
        	this.VisualEnvironmentCompiler=Workbench.VisualEnvironmentCompiler;
            VisualEnvironmentCompiler.StandartCompiler.OnChangeCompilerState += new ChangeCompilerStateEventDelegate(Compiler_OnChangeCompilerState);
            if(VisualEnvironmentCompiler.RemoteCompiler!=null)
                VisualEnvironmentCompiler.RemoteCompiler.OnChangeCompilerState += new ChangeCompilerStateEventDelegate(Compiler_OnChangeCompilerState);
            //CompilerInternalErrorReport.Parent=(VisualEnvironmentCompiler as System.Windows.Forms.Control);
        }
        private string GetInfo()
        {
            string s;
            if (VisualEnvironmentCompiler.Compiler.InternalDebug.DebugVersion)
                s= ", debug version";
            else
                s= ", relase version";
            return string.Format("{0} ({1}){2}{3}", Compiler.Banner, Compiler.VersionDateTime.ToShortDateString(),s, Environment.NewLine) +
                    "Runtime version: " + Environment.Version + Environment.NewLine +
                    "OS version: " + Environment.OSVersion + Environment.NewLine+
                    "Processor count: " + Environment.ProcessorCount.ToString() + Environment.NewLine +
                    string.Format("WorkingSet: {0} kb", Environment.WorkingSet / 1024);
                    
        }
        void Compiler_OnChangeCompilerState(ICompiler sender, CompilerState State, string FileName)
        {
            States += State.ToString();
            if (FileName != null) 
                States += " "+System.IO.Path.GetFileName(FileName);
            States += Environment.NewLine;
            switch (State)
            {
                case CompilerState.CompilationStarting:
                    FileNames.Clear();
                    States = "";
                    break;
                case CompilerState.BeginCompileFile:
                    FileNames.Add(FileName);
                    break;
                case CompilerState.ReadPCUFile:
                    FileNames.Add(FileName);
                    FileNames.Add(System.IO.Path.ChangeExtension(FileName,".pas"));
                    break;
                case CompilerState.Ready:
                    foreach (Error error in VisualEnvironmentCompiler.Compiler.ErrorsList)
                        if (error is CompilerInternalError)
                        {
                            ReportText = DateTime.Now.ToString() + Environment.NewLine;
                            ReportText += GetInfo()+Environment.NewLine;
                            ReportText += "StatesList: " + Environment.NewLine + States + Environment.NewLine;
                            for (int i = 0; i < VisualEnvironmentCompiler.Compiler.ErrorsList.Count; i++)
                                ReportText += string.Format("Error[{0}]: {1}{2}", i, VisualEnvironmentCompiler.Compiler.ErrorsList[i].ToString(),Environment.NewLine);
                            CompilerInternalErrorReport.ErrorMessage.Text = error.ToString();
                            CompilerInternalErrorReport.ReportText = ReportText;
                            CompilerInternalErrorReport.FileNames = FileNames;
                            CompilerInternalErrorReport.VEC = VisualEnvironmentCompiler;
                            CompilerInternalErrorReport.ShowDialog();
                            return;
                        }
                    break;
            }
        }

        public string Name
        {
            get
            {
                return "InternalError Report Genegator";
            }
        }
        public string Version
        {
            get
            {
                return "1.2";
            }
        }
        public string Copyright
        {
            get
            {
                return "Copyright © 2005-2018 by Ivan Bondarev, Stanislav Mihalkovich";
            }
        }


        void Execute()
        {
            ErrorReport.VersionInfo.Text = GetInfo();
            ErrorReport.ShowDialog();
        }

        public void GetGUI(List<IPluginGUIItem> MenuItems, List<IPluginGUIItem> ToolBarItems)
        {
            PluginGUIItem Item = new PluginGUIItem(StringsPrefix + "NAME", StringsPrefix + "DESCRIPTION", ErrorReport.PluginImage.Image,  ErrorReport.PluginImage.BackColor, Execute);
            MenuItems.Add(Item);
        }

    }
}
