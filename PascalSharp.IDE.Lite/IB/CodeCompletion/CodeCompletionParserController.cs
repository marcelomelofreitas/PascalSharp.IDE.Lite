// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Document;
using PascalSharp.Internal.ParserTools;
using PascalSharp.Internal.CodeCompletion;
using PascalSharp.Internal.Localization;
using VisualPascalABC.Projects;

namespace VisualPascalABC
{
    public delegate void ParseInformationUpdatedDelegate(object obj, string fileName);

    public class CodeCompletionParserController : VisualPascalABCPlugins.ICodeCompletionService
    {
        public static Hashtable open_files = new Hashtable(StringComparer.OrdinalIgnoreCase);
        public VisualEnvironmentCompiler visualEnvironmentCompiler;
        private System.Threading.Thread th = null;
        private CodeCompletionProvider ccp;
        public event ParseInformationUpdatedDelegate ParseInformationUpdated;

        public void StopParseThread()
        {
            try
            {
                if (th != null)
                    th.Abort();
            }
            catch
            {
            }
        }

        public CodeCompletionParserController()
        {
            this.th = new System.Threading.Thread(new System.Threading.ThreadStart(ParseInThread));
            ccp = new CodeCompletionProvider();
        }

        public ICodeCompletionDomConverter GetConverter(string fileName)
        {
            return CodeCompletionController.comp_modules[fileName] as ICodeCompletionDomConverter;
        }

        public static string CurrentTwoLetterISO
        {
            get
            {
                return CodeCompletionController.currentLanguageISO;
            }
            set
            {
                CodeCompletionController.currentLanguageISO = value;
            }
        }

        public void Init()
        {
            CodeCompletionController.ParsersController.SourceFilesProvider = visualEnvironmentCompiler.SourceFilesProvider;
            CodeCompletionController.currentLanguageISO = StringResourcesLanguage.CurrentTwoLetterISO;
        }

        public void RenameFile(string OldFileName, string NewFileName)
        {
            if (string.Compare(OldFileName, NewFileName, true) != 0)
            {
                CodeCompletionController.comp_modules[NewFileName] = CodeCompletionController.comp_modules[OldFileName];
                if (CodeCompletionController.comp_modules.ContainsKey(OldFileName))
                    CodeCompletionController.comp_modules.Remove(OldFileName);
                open_files[NewFileName] = open_files[OldFileName];
                if (open_files.ContainsKey(OldFileName))
                    open_files.Remove(OldFileName);
            }
        }

        public void RegisterFileForParsing(string FileName)
        {
            open_files[FileName] = true;
            CodeCompletionController.SetParser(System.IO.Path.GetExtension(FileName));
            //ParseAllFiles();
        }

        public void CloseFile(string FileName)
        {
            if (CodeCompletionController.comp_modules[FileName] != null)
                CodeCompletionController.comp_modules.Remove(FileName);
            if (open_files[FileName] != null)
                open_files.Remove(FileName);
        }

        public void SetAsChanged(string FileName)
        {
            if (FileName != null)
                open_files[FileName] = true;
        }

        public void SetAllInProjectChanged()
        {
            try
            {
                Hashtable open_files2 = open_files.Clone() as Hashtable;
                foreach (string s in open_files2.Keys)
                {
                    if (ProjectFactory.Instance.CurrentProject.ContainsSourceFile(s))
                        open_files[s] = true;
                }
            }
            catch (Exception e)
            {

            }
        }

        public void RunParseThread()
        {
            th = new System.Threading.Thread(InternalParsing);
            th.Priority = System.Threading.ThreadPriority.BelowNormal;
            th.IsBackground = true;
            th.Start();
        }

        public void StopParsing()
        {
            try
            {
                VisualPABCSingleton.MainForm.StopTimer();
            }
            catch
            {

            }
        }

        private void InternalParsing()
        {
            while (true)
            {
                ParseInThread();
                System.Threading.Thread.Sleep(2000);
            }
        }

        private long mem_delta = 0;

        internal void ParseInThread()
        {
            try
            {
                Hashtable open_files2 = (Hashtable)open_files.Clone();
                Hashtable recomp_files = new Hashtable(StringComparer.OrdinalIgnoreCase);
                bool is_comp = false;
                foreach (string FileName in open_files2.Keys)
                {
                    //(ssyy) 18.05.08 Вставил проверку на null
                    object o = open_files[FileName];
                    if (o != null && (bool)o == true)
                    {
                        is_comp = true;
                        CodeCompletionController controller = new CodeCompletionController();
                        string text = visualEnvironmentCompiler.SourceFilesProvider(FileName, SourceFileOperation.GetText) as string;
                        if (string.IsNullOrEmpty(text))
                            text = "begin end.";
                        DomConverter tmp = CodeCompletionController.comp_modules[FileName] as DomConverter;
                        long cur_mem = Environment.WorkingSet;
                        DomConverter dc = controller.Compile(FileName, text);
                        mem_delta += Environment.WorkingSet - cur_mem;
                        open_files[FileName] = false;
                        if (dc.is_compiled)
                        {
                            //CodeCompletionController.comp_modules.Remove(FileName);
                            if (tmp != null && tmp.visitor.entry_scope != null)
                            {
                                tmp.visitor.entry_scope.Clear();
                                if (tmp.visitor.cur_scope != null)
                                    tmp.visitor.cur_scope.Clear();
                            }
                            CodeCompletionController.comp_modules[FileName] = dc;
                            recomp_files[FileName] = FileName;
                            open_files[FileName] = false;
                            if (ParseInformationUpdated != null)
                                ParseInformationUpdated(dc.visitor.entry_scope, FileName);
                        }
                        else if (CodeCompletionController.comp_modules[FileName] == null)
                            CodeCompletionController.comp_modules[FileName] = dc;
                    }
                }
                foreach (string FileName in open_files2.Keys)
                {
                    DomConverter dc = CodeCompletionController.comp_modules[FileName] as DomConverter;
                    SymScope ss = null;
                    if (dc != null)
                    {
                        if (dc.visitor.entry_scope != null) ss = dc.visitor.entry_scope;
                        else if (dc.visitor.impl_scope != null) ss = dc.visitor.impl_scope;
                        int j = 0;
                        while (j < 2)
                        {
                            if (j == 0)
                            {
                                ss = dc.visitor.entry_scope;
                                j++;
                            }
                            else
                            {
                                ss = dc.visitor.impl_scope;
                                j++;
                            }
                            if (ss != null)
                            {
                                for (int i = 0; i < ss.used_units.Count; i++)
                                {
                                    string s = ss.used_units[i].file_name;
                                    if (s != null && open_files2.ContainsKey(s) && recomp_files.ContainsKey(s))
                                    {
                                        is_comp = true;
                                        CodeCompletionController controller = new CodeCompletionController();
                                        string text = visualEnvironmentCompiler.SourceFilesProvider(FileName, SourceFileOperation.GetText) as string;
                                        DomConverter tmp = CodeCompletionController.comp_modules[FileName] as DomConverter;
                                        long cur_mem = Environment.WorkingSet;
                                        dc = controller.Compile(FileName, text);
                                        mem_delta += Environment.WorkingSet - cur_mem;
                                        open_files[FileName] = false;
                                        CodeCompletionController.comp_modules[FileName] = dc;
                                        if (dc.is_compiled)
                                        {
                                            /*if (tmp != null && tmp.stv.entry_scope != null)
                                            {
                                                tmp.stv.entry_scope.Clear();
                                                if (tmp.stv.cur_scope != null) tmp.stv.cur_scope.Clear();
                                            }*/
                                            CodeCompletionController.comp_modules[FileName] = dc;
                                            recomp_files[FileName] = FileName;
                                            ss.used_units[i] = dc.visitor.entry_scope;
                                            if (ParseInformationUpdated != null)
                                                ParseInformationUpdated(dc.visitor.entry_scope, FileName);
                                        }
                                        else if (CodeCompletionController.comp_modules[FileName] == null)
                                            CodeCompletionController.comp_modules[FileName] = dc;
                                    }
                                }
                            }
                        }
                    }
                }
                if (is_comp && mem_delta > 20000000 /*&& mem_delta > 10000000*/)
                //postavil delta dlja pamjati, posle kototoj delaetsja sborka musora
                {
                    mem_delta = 0;
                    GC.Collect();
                }
            }
            catch (Exception e)
            {

            }

        }

        public bool IsParsing()
        {
            return th != null && th.ThreadState == System.Threading.ThreadState.Running;
        }

        public void ParseAllFiles()
        {
            if (visualEnvironmentCompiler.UserOptions.AllowCodeCompletion)// && visualEnvironmentCompiler.compilerLoaded)
            {
                if (th.ThreadState != System.Threading.ThreadState.Running)
                {
                    th = new System.Threading.Thread(new System.Threading.ThreadStart(this.ParseInThread));
                    th.Priority = System.Threading.ThreadPriority.BelowNormal;
                    //th.IsBackground = true;
                    th.Start();
                }
                //if (th == null) 
                //	RunParseThread();
            }
        }
    }
}

