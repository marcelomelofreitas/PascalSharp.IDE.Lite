﻿// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PascalABCCompiler.SyntaxTree;
using PascalSharp.Compiler;
using PascalSharp.Internal.Errors;
using PascalSharp.Internal.Localization;
using VisualPascalABC.Projects;
using VisualPascalABCPlugins;

namespace VisualPascalABC
{
    public class WorkbenchBuildService : IWorkbenchBuildService
    {
        public CompilerOptions CompilerOptions1 = new CompilerOptions();
        bool __ForRun;
        string __RuntimeServicesModule;
        bool __savePCU;
        List<Error> ErrorsList = new List<Error>();

        IWorkbench Workbench;
        IWorkbenchProjectService ProjectService;
        IWorkbenchDesignerService DesignerService;
        IWorkbenchDocumentService DocumentService;

        public WorkbenchBuildService()
        {
            Workbench = WorkbenchServiceFactory.Workbench;
            ProjectService = WorkbenchServiceFactory.ProjectService;
            DesignerService = WorkbenchServiceFactory.DesignerService;
            DocumentService = WorkbenchServiceFactory.DocumentService;
        }

        public CompilerOptions CompilerOptions
        {
            get
            {
                return CompilerOptions1;
            }
        }

        List<Error> IWorkbenchBuildService.ErrorsList
        {
            get
            {
                return ErrorsList;
            }
        }

        //kompilacija proekta
        public string Compile(IProjectInfo project, bool rebuild, string RuntimeServicesModule, bool ForRun, bool RunWithEnvironment)
        {
            ProjectService.SaveProject();
            Workbench.WidgetController.CompilingButtonsEnabled = false;
            Workbench.CompilerConsoleWindow.ClearConsole();
            CompilerOptions1.SourceFileName = project.Path;
            CompilerOptions1.OutputDirectory = project.OutputDirectory;
            CompilerOptions1.ProjectCompiled = true;
            if (project.ProjectType == ProjectType.WindowsApp)
                CompilerOptions1.OutputFileType = CompilerOptions.OutputType.WindowsApplication;
            ErrorsList.Clear();
            CompilerOptions1.Rebuild = rebuild;
            CompilerOptions1.Locale = StringResourcesLanguage.CurrentTwoLetterISO;
            CompilerOptions1.UseDllForSystemUnits = false;
            CompilerOptions1.RunWithEnvironment = RunWithEnvironment;
            bool savePCU = Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate;
            if (Path.GetDirectoryName(CompilerOptions1.SourceFileName).ToLower() == ((string)WorkbenchStorage.StandartDirectories[Constants.LibSourceDirectoryIdent]).ToLower())
                Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate = false;
            if (RuntimeServicesModule != null)
                CompilerOptions1.StandartModules.Add(new CompilerOptions.StandartModule(RuntimeServicesModule, CompilerOptions.StandartModuleAddMethod.RightToMain, LanguageId.C | LanguageId.PascalABCNET | LanguageId.CommonLanguage));

            string ofn = Workbench.VisualEnvironmentCompiler.Compile(CompilerOptions1);
            Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate = savePCU;

            if (RuntimeServicesModule != null)
                CompilerOptions1.RemoveStandartModuleAtIndex(CompilerOptions1.StandartModules.Count - 1);

            if (Workbench.VisualEnvironmentCompiler.Compiler.ErrorsList.Count != 0 || Workbench.VisualEnvironmentCompiler.Compiler.Warnings.Count != 0)
            {
                List<Error> ErrorsAndWarnings = new List<Error>();
                List<Error> Errors = Workbench.ErrorsManager.CreateErrorsList(Workbench.VisualEnvironmentCompiler.Compiler.ErrorsList);
                AddErrors(ErrorsAndWarnings, Errors);
                //if (!ForRun)
                AddWarnings(ErrorsAndWarnings, Workbench.VisualEnvironmentCompiler.Compiler.Warnings);
                Workbench.ErrorsListWindow.ShowErrorsSync(ErrorsAndWarnings, Errors.Count != 0 || (Workbench.VisualEnvironmentCompiler.Compiler.Warnings.Count != 0 && !ForRun));
            }
            return ofn;
        }

        public string Compile(string FileName, bool rebuild, string RuntimeServicesModule, bool ForRun, bool RunWithEnvironment)
        {
            Workbench.WidgetController.CompilingButtonsEnabled = false;

            var UserOptions = Workbench.UserOptions;

            Workbench.CompilerConsoleWindow.ClearConsole();

            CompilerOptions1.SourceFileName = FileName;
            CompilerOptions1.Locale = StringResourcesLanguage.CurrentTwoLetterISO;
            if (Path.GetDirectoryName(FileName) == "" && WorkbenchStorage.WorkingDirectoryExsist)
                CompilerOptions1.OutputDirectory = WorkbenchStorage.WorkingDirectory;

            if (Workbench.UserOptions.UseOutputDirectory && Directory.Exists(UserOptions.OutputDirectory))
                CompilerOptions1.OutputDirectory = UserOptions.OutputDirectory;

            CompilerOptions1.Rebuild = rebuild;
            CompilerOptions1.RunWithEnvironment = RunWithEnvironment;
            //string runtimeModuleFileName = CompilerOptions1.SearchDirectory + "\\" + RuntimeServicesModule;
            //if (RuntimeServicesModule!=null && File.Exists(runtimeModuleFileName))
            //    CompilerOptions1.StandartModules.Add(new CompilerOptions.StandartModule(runtimeModuleFileName, CompilerOptions.StandartModuleAddMethod.RightToMain));
            if (RuntimeServicesModule != null)
                CompilerOptions1.StandartModules.Add(new CompilerOptions.StandartModule(RuntimeServicesModule, CompilerOptions.StandartModuleAddMethod.RightToMain, LanguageId.C | LanguageId.PascalABCNET | LanguageId.CommonLanguage));

            ErrorsList.Clear();

            //CompilerOptions1.SavePCUInThreadPull = true;
            bool savePCU = Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate;
            if (Path.GetDirectoryName(FileName).ToLower() == ((string)WorkbenchStorage.StandartDirectories[Constants.LibSourceDirectoryIdent]).ToLower())
                Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate = false;

            string ofn = Workbench.VisualEnvironmentCompiler.Compile(CompilerOptions1);

            Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate = savePCU;

            if (RuntimeServicesModule != null)
                CompilerOptions1.RemoveStandartModuleAtIndex(CompilerOptions1.StandartModules.Count - 1);

            if (Workbench.VisualEnvironmentCompiler.Compiler.ErrorsList.Count != 0 || Workbench.VisualEnvironmentCompiler.Compiler.Warnings.Count != 0)
            {
                List<Error> ErrorsAndWarnings = new List<Error>();
                List<Error> Errors = Workbench.ErrorsManager.CreateErrorsList(Workbench.VisualEnvironmentCompiler.Compiler.ErrorsList);
                AddErrors(ErrorsAndWarnings, Errors);
                //if (!ForRun)
                AddWarnings(ErrorsAndWarnings, Workbench.VisualEnvironmentCompiler.Compiler.Warnings);
                Workbench.ErrorsListWindow.ShowErrorsSync(ErrorsAndWarnings, Errors.Count != 0 || (Workbench.VisualEnvironmentCompiler.Compiler.Warnings.Count != 0 && !ForRun));
            }
            return ofn;
        }

        public void StartCompile(IProjectInfo project, bool rebuild, string RuntimeServicesModule, bool ForRun, bool RunWithEnvironment)
        {
            ProjectService.SaveProject();
            __RuntimeServicesModule = RuntimeServicesModule;
            __ForRun = ForRun;
            Workbench.CompilerConsoleWindow.ClearConsole();
            var UserOptions = Workbench.UserOptions;

            CompilerOptions1.SourceFileName = project.Path;
            CompilerOptions1.ProjectCompiled = true;
            CompilerOptions1.Locale = StringResourcesLanguage.CurrentTwoLetterISO;
            if (UserOptions.UseOutputDirectory && Directory.Exists(UserOptions.OutputDirectory))
                CompilerOptions1.OutputDirectory = UserOptions.OutputDirectory;

            CompilerOptions1.Rebuild = rebuild;
            CompilerOptions1.RunWithEnvironment = RunWithEnvironment;
            //string runtimeModuleFileName = CompilerOptions1.SearchDirectory + "\\" + RuntimeServicesModule;
            //if (RuntimeServicesModule!=null && File.Exists(runtimeModuleFileName))
            //    CompilerOptions1.StandartModules.Add(new CompilerOptions.StandartModule(runtimeModuleFileName, CompilerOptions.StandartModuleAddMethod.RightToMain));
            if (__RuntimeServicesModule != null)
                CompilerOptions1.StandartModules.Add(new CompilerOptions.StandartModule(RuntimeServicesModule, CompilerOptions.StandartModuleAddMethod.RightToMain, LanguageId.C | LanguageId.PascalABCNET | LanguageId.CommonLanguage));


            ErrorsList.Clear();

            //CompilerOptions1.SavePCUInThreadPull = true;
            __savePCU = Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate;
            //if (Path.GetDirectoryName(FileName).ToLower() == ((string)StandartDirectories[Constants.LibSourceDirectoryIdent]).ToLower())
            //    VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate = false;

            Workbench.VisualEnvironmentCompiler.Compiler.OnChangeCompilerState += CompilationOnChangeCompilerState;

            Workbench.VisualEnvironmentCompiler.StartCompile(CompilerOptions1);
        }

        public void StartCompile(string FileName, bool rebuild, string RuntimeServicesModule, bool ForRun, bool RunWithEnvironment)
        {
            __RuntimeServicesModule = RuntimeServicesModule;
            __ForRun = ForRun;
            Workbench.CompilerConsoleWindow.ClearConsole();
            var UserOptions = Workbench.UserOptions;
            CompilerOptions1.SourceFileName = FileName;
            CompilerOptions1.Locale = StringResourcesLanguage.CurrentTwoLetterISO;
            if (Path.GetDirectoryName(FileName) == "" && WorkbenchStorage.WorkingDirectoryExsist)
                CompilerOptions1.OutputDirectory = WorkbenchStorage.WorkingDirectory;

            if (UserOptions.UseOutputDirectory && Directory.Exists(UserOptions.OutputDirectory))
                CompilerOptions1.OutputDirectory = UserOptions.OutputDirectory;


            CompilerOptions1.Rebuild = rebuild;
            CompilerOptions1.RunWithEnvironment = RunWithEnvironment;
            //string runtimeModuleFileName = CompilerOptions1.SearchDirectory + "\\" + RuntimeServicesModule;
            //if (RuntimeServicesModule!=null && File.Exists(runtimeModuleFileName))
            //    CompilerOptions1.StandartModules.Add(new CompilerOptions.StandartModule(runtimeModuleFileName, CompilerOptions.StandartModuleAddMethod.RightToMain));
            if (__RuntimeServicesModule != null)
                CompilerOptions1.StandartModules.Add(new CompilerOptions.StandartModule(RuntimeServicesModule, CompilerOptions.StandartModuleAddMethod.RightToMain, LanguageId.C | LanguageId.PascalABCNET | LanguageId.CommonLanguage));


            ErrorsList.Clear();

            //CompilerOptions1.SavePCUInThreadPull = true;
            __savePCU = Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate;
            if (Path.GetDirectoryName(FileName).ToLower() == ((string)WorkbenchStorage.StandartDirectories[Constants.LibSourceDirectoryIdent]).ToLower())
                Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate = false;

            Workbench.VisualEnvironmentCompiler.Compiler.OnChangeCompilerState += CompilationOnChangeCompilerState;

            Workbench.VisualEnvironmentCompiler.StartCompile(CompilerOptions1);

        }

        public bool Build()
        {
            Workbench.OutputWindow.ClearTextBox();
            Workbench.ErrorsListWindow.ClearErrorList();
            DesignerService.GenerateAllDesignersCode();
            CompilerOptions1.UseDllForSystemUnits = false;
            if (!ProjectFactory.Instance.ProjectLoaded)
                return Compile(DocumentService.CurrentCodeFileDocument.FileName, false, null, false, false) != null;
            else
                return Compile(ProjectFactory.Instance.CurrentProject, false, null, false, false) != null;
        }

        public bool Build(string FileName)
        {
            Workbench.OutputWindow.ClearTextBox();
            Workbench.ErrorsListWindow.ClearErrorList();
            /*while (codeCompletionParserController.IsParsing())
                Thread.Sleep(10);*/
            CompilerOptions1.UseDllForSystemUnits = false;
            return Compile(FileName, false, null, false, false) != null;
        }

        public bool Rebuild()
        {
            Workbench.OutputWindow.ClearTextBox();
            Workbench.ErrorsListWindow.ClearErrorList();
            DesignerService.GenerateAllDesignersCode();
            CompilerOptions1.UseDllForSystemUnits = false;
            if (!ProjectFactory.Instance.ProjectLoaded)
                return Compile(DocumentService.CurrentCodeFileDocument.FileName, true, null, false, false) != null;
            else
                return Compile(ProjectFactory.Instance.CurrentProject, true, null, false, false) != null;
        }

        public void StartCompile(bool rebuild)
        {
            VisualPABCSingleton.MainForm.OutputWindow.outputTextBox.Clear();
            Workbench.ErrorsListWindow.ClearErrorList();
            DesignerService.GenerateAllDesignersCode();
            CompilerOptions1.UseDllForSystemUnits = false;
            if (!ProjectFactory.Instance.ProjectLoaded)
                StartCompile(DocumentService.CurrentCodeFileDocument.FileName, rebuild, null, false, false);
            else
                StartCompile(ProjectFactory.Instance.CurrentProject, rebuild, null, false, false);
        }

        void CompilationOnChangeCompilerState(ICompiler sender, CompilerState State, string FileName)
        {
            switch (State)
            {
                case CompilerState.CompilationFinished:
                    Workbench.VisualEnvironmentCompiler.Compiler.InternalDebug.PCUGenerate = __savePCU;

                    if (__RuntimeServicesModule != null)
                        CompilerOptions1.RemoveStandartModuleAtIndex(CompilerOptions1.StandartModules.Count - 1);

                    if (Workbench.VisualEnvironmentCompiler.Compiler.ErrorsList.Count != 0 || Workbench.VisualEnvironmentCompiler.Compiler.Warnings.Count != 0)
                    {
                        List<Error> ErrorsAndWarnings = new List<Error>();
                        List<Error> Errors = Workbench.ErrorsManager.CreateErrorsList(Workbench.VisualEnvironmentCompiler.Compiler.ErrorsList);
                        AddErrors(ErrorsAndWarnings, Errors);
                        //if (!ForRun)
                        AddWarnings(ErrorsAndWarnings, Workbench.VisualEnvironmentCompiler.Compiler.Warnings);
                        Workbench.ErrorsListWindow.ShowErrorsSync(ErrorsAndWarnings, Errors.Count != 0 || (Workbench.VisualEnvironmentCompiler.Compiler.Warnings.Count != 0 && !__ForRun));
                    }
                    Workbench.VisualEnvironmentCompiler.Compiler.OnChangeCompilerState -= CompilationOnChangeCompilerState;
                    break;
            }
        }

        public void AddWarnings(List<Error> Errors, List<CompilerWarning> warns)
        {
            foreach (CompilerWarning w in warns)
                Errors.Add(w);
        }

        public void AddErrors(List<Error> Errors, List<Error> ers)
        {
            foreach (Error e in ers)
                Errors.Add(e);
        }
    }
}
