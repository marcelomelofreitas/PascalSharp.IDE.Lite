﻿// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using PascalSharp.Compiler;
using PascalSharp.Internal.ParserTools;

namespace VisualPascalABC.Projects
{
	
	public class ProjectFactory
	{
		private ProjectInfo currentProject;
		private int uid=1;
		
		static ProjectFactory()
		{
			
		}
		
		public IProjectInfo CurrentProject
		{
			get
			{
				return currentProject;
			}
		}
		
		private static ProjectFactory instance;
		public static ProjectFactory Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ProjectFactory();
				}
				return instance;
			}
		}
		
		public string ProjectDirectory
		{
			get
			{
				return Path.GetDirectoryName(currentProject.Path);
			}
		}

        public IProjectInfo CreateProject(string projectName, string projectFileName, ProjectType projectType)
        {
            currentProject = new ProjectInfo();
            currentProject.name = projectName;
            currentProject.path = projectFileName;
            string dir = Path.GetDirectoryName(projectFileName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            currentProject.include_debug_info = true;

            currentProject.project_type = projectType;
            currentProject.source_files.Add(new SourceCodeFileInfo(projectName + ".pas", Path.Combine(dir, projectName + ".pas")));
            currentProject.references.Add(new ReferenceInfo("System", "System.dll"));
            if (projectType == ProjectType.WindowsApp)
            {
                currentProject.references.Add(new ReferenceInfo("System.Windows.Forms", "System.Windows.Forms.dll"));
                currentProject.references.Add(new ReferenceInfo("System.Drawing", "System.Drawing.dll"));
                //roman//
                currentProject.references.Add(new ReferenceInfo("System.Core", "System.Core.dll"));
                currentProject.references.Add(new ReferenceInfo("System.Data", "System.Data.dll"));
                currentProject.references.Add(new ReferenceInfo("System.Data.DataSetExtensions", "System.Data.DataSetExtensions.dll"));
                currentProject.references.Add(new ReferenceInfo("System.Xml", "System.Xml.dll"));
                currentProject.references.Add(new ReferenceInfo("System.Xml.Linq", "System.Xml.Linq.dll"));
                //roman//
            }
            currentProject.main_file = Path.Combine(dir, projectName + ".pas");
            currentProject.generate_xml_doc = false;
            currentProject.delete_exe = true;
            currentProject.delete_pdb = true;
            currentProject.major_version = 0;
            currentProject.minor_version = 0;
            currentProject.build_version = 0;
            currentProject.revision_version = 0;
            currentProject.output_directory = dir;
            StreamWriter sw = File.CreateText(Path.Combine(dir, projectName + ".pas"));
            currentProject.output_file_name = projectName + ".exe";
            if (projectType == ProjectType.ConsoleApp)
            {
                //sw.WriteLine("program "+projectName+";");
                //sw.WriteLine();
                //sw.WriteLine("uses System;");
                //sw.WriteLine();
                sw.WriteLine("begin");
                sw.WriteLine();
                sw.Write("end.");
            }
            else if (projectType == ProjectType.Library)
            {
                sw.WriteLine("library " + projectName + ";");
                sw.WriteLine();
                //sw.WriteLine("uses System;");
                //sw.WriteLine();
                sw.Write("end.");
                currentProject.output_file_name = projectName + ".dll";
            }
            sw.Close();
            currentProject.Save();
            //VisualPABCSingleton.MainForm.OpenFile(Path.Combine(dir,projectName+".pas"));
            return currentProject;
        }

        public IProjectInfo OpenProject(string projectFileName)
        {
            ProjectInfo _currentProject = new ProjectInfo();
            _currentProject.Load(projectFileName);
            currentProject = _currentProject;
            return currentProject;
        }
		
		public bool ProjectLoaded
		{
			get
			{
				return currentProject != null;
			}
		}
		
		private bool dirty = false;
		public bool Dirty
		{
			get
			{
				return dirty;
			}
			set
			{
				dirty = value;
				if (value == true)
				{
					VisualPABCSingleton.MainForm.SaveAllButtonsEnabled = true;
					WorkbenchServiceFactory.CodeCompletionParserController.SetAllInProjectChanged();
				}
				else
				{
					VisualPABCSingleton.MainForm.SaveAllButtonsEnabled = VisualPABCSingleton.MainForm.AllSaved();
				}
			}
		}

        public IFileInfo AddSourceFile(string fileName)
        {
            SourceCodeFileInfo fi = new SourceCodeFileInfo(fileName, Path.Combine(Path.GetDirectoryName(currentProject.Path), fileName));
            currentProject.source_files.Add(fi);
            Dirty = true;
            return fi;
        }
        
        public void AddNamespaceFileReference(string fileName)
        {
            var text = WorkbenchServiceFactory.Workbench.VisualEnvironmentCompiler.SourceFilesProvider(currentProject.main_file, SourceFileOperation.GetText) as string;
            text = "{$includenamespace " + Path.GetFileName(fileName) + "}"+Environment.NewLine + text;
            var doc = WorkbenchServiceFactory.DocumentService.GetDocument(currentProject.main_file);
            if (doc != null)
            {
            	doc.TextEditor.Text = text;
            }
            else
            {
            	File.WriteAllText(currentProject.main_file, text);
            }
        }
        
        public void RemoveNamespaceFileReference(string fileName)
        {
        	var text = WorkbenchServiceFactory.Workbench.VisualEnvironmentCompiler.SourceFilesProvider(currentProject.main_file, SourceFileOperation.GetText) as string;
        	text = text.Replace("{$includenamespace " + Path.GetFileName(fileName) + "}"+Environment.NewLine,"");
        	var doc = WorkbenchServiceFactory.DocumentService.GetDocument(currentProject.main_file);
            if (doc != null)
            {
            	doc.TextEditor.Text = text;
            }
            else
            {
            	File.WriteAllText(currentProject.main_file, text);
            }
        }

        public IReferenceInfo AddReference(string s)
        {
            ReferenceInfo ri = new ReferenceInfo(s, s + ".dll");
            currentProject.references.Add(ri);
            Dirty = true;
            return ri;
        }
		
		public void RemoveReference(IReferenceInfo ri)
		{
			currentProject.RemoveReference(ri);
			Dirty = true;
		}
		
		public void ExcludeFile(IFileInfo fi)
		{
			currentProject.ExcludeFile(fi);
			Dirty = true;
		}
		
		public void RenameFile(IFileInfo fi, string new_name)
		{
			fi.Name = new_name;
			Dirty = true;
		}
		
		public string GetUnitFileName()
		{
			return "Unit"+uid++ + ".pas";
		}

        public string GetFullUnitFileName()
        {
            return Path.Combine(Path.GetDirectoryName(currentProject.path), GetUnitFileName());
        }
		
		public void SaveProject()
		{
			if (currentProject != null)
			{
				currentProject.Save();
				Dirty = false;
			}
		}
		
		public void CloseProject()
		{
			currentProject = null;
			uid = 1;
			Dirty = false;
		}
	}
}
