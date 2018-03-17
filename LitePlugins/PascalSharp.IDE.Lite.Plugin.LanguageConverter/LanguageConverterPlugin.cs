using System;
using System.Collections.Generic;
using System.Text;
using Converter;
using System.IO;
using PascalSharp.Compiler;
using PascalSharp.Internal.SemanticTree;

namespace VisualPascalABCPlugins
{
    public class LanguageConverter_VisualPascalABCPlugin : IVisualPascalABCPlugin
    {
        public static string StringsPrefix = "VPP_LNGCONVERT_";
        IVisualEnvironmentCompiler VisualEnvironmentCompiler;
        internal ISemanticNodeConverter currentLanguage;
        private TextFormatterForm TextFormatterForm;
        //public TextFormatter TextFormatter;
        public List<ISemanticNodeConverter> Languages;
        
        public string Name
        {
            get
            {
                return "Language Converter";
            }
        }
        public string Version
        {
            get
            {
                return "0.1";
            }
        }
        public string Copyright
        {
            get
            {
                return "Copyright © 2005-2018 by Ivan Bondarev, Stanislav Mihalkovich";
            }
        }

        public void OpenFilesInEnvironment(List<string> fileNames)
        {
            foreach (string name in fileNames)
                VisualEnvironmentCompiler.ExecuteAction(VisualEnvironmentCompilerAction.OpenFile, name);
        }

        public void ExecuteBuild()
        {
            MyCompilation = true;
            VisualEnvironmentCompiler.StandartCompiler.InternalDebug.CodeGeneration = false;
            CompilerType ct = VisualEnvironmentCompiler.DefaultCompilerType;
            VisualEnvironmentCompiler.DefaultCompilerType = CompilerType.Standart;
            VisualEnvironmentCompiler.ExecuteAction(VisualEnvironmentCompilerAction.Build, null);
            VisualEnvironmentCompiler.DefaultCompilerType = ct;
            VisualEnvironmentCompiler.StandartCompiler.InternalDebug.CodeGeneration = true;
        }

        public void ExecuteOptions()
        {
            TextFormatterForm.ShowDialog();
        }

        public LanguageConverter_VisualPascalABCPlugin(IWorkbench Workbench)
        {
            Languages = LoadLanguages();
            if (Languages.Count>0)
                currentLanguage = Languages[0];
            this.VisualEnvironmentCompiler = Workbench.VisualEnvironmentCompiler;
            //TextFormatter = new TextFormatter();
            TextFormatterForm = new TextFormatterForm();
            TextFormatterForm.Plugin = this;
            TextFormatterForm.Init();
            VisualEnvironmentCompiler.StandartCompiler.OnChangeCompilerState += new ChangeCompilerStateEventDelegate(Compiler_OnChangeCompilerState);
        }

        public void ConvertSourceText()
        {
            try
            {
                IProgramNode Root = VisualEnvironmentCompiler.StandartCompiler.SemanticTree;
                //SemanticNodeConverter semanticNodeConverter = new SemanticNodeConverter(new ListSyntRules(
                //    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName) + "\\Rules_CS.txt"), TextFormatter/*new TextFormatter()*/);

                //TextFormatterForm.TextFormatter=new TextFormatter();
                //TextFormatterForm.SaveTextFormater(currentLanguage.SourceTextBuilder.TextFormatter);
                //currentLanguage.SourceTextBuilder.TextFormatter = TextFormatterForm.TextFormatter;
                TextFormatterForm.SaveTextFormater(currentLanguage.SourceTextBuilder.TextFormatter);            
                SemanticTreeVisitor Visitor = new SemanticTreeVisitor(currentLanguage);//new SemanticNodeConverter(new ListSyntRules("Rules_CS.txt"), new TextFormatter()));
                Root.visit(Visitor);

                //ConvertTextViewer.Text = Visitor.GetConvertedText();
                OpenFilesInEnvironment(Visitor.nmspaceFiles);
            }
            catch (NotSupportedException e)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("Извините, генерация кода для узла {0} не поддерживается. Текст не был сгенерирован.", e.Message));
            }
            catch (Exception e)
            {
                VisualEnvironmentCompiler.ExecuteAction(VisualEnvironmentCompilerAction.AddTextToCompilerMessages, "LanguageConverter Exception: " + e.ToString());
            }
        }        

        bool MyCompilation = false;
        void Compiler_OnChangeCompilerState(ICompiler sender, CompilerState State, string FileName)
        {
            if (!MyCompilation) return;
            switch (State)
            {
                case CompilerState.CompilationStarting:
                    //this.Refresh();
                    break;
                case CompilerState.CompilationFinished:
                    ConvertSourceText();
                    MyCompilation = false;
                    break;
            }
        }        

        // стырено 
        private List<ISemanticNodeConverter> LoadLanguages()
        {
            List<ISemanticNodeConverter> Langs = new List<ISemanticNodeConverter>(); 
            // директория этого плагина           
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] dllfiles = di.GetFiles("*LanguageConverter.dll");
            System.Reflection.Assembly asssembly = null;
            Type constr = null;
            ISemanticNodeConverter pc = null;
            foreach (FileInfo fi in dllfiles)
            {
                asssembly = System.Reflection.Assembly.LoadFile(fi.FullName);
                try
                {
                    if (asssembly != null && asssembly.FullName != System.Reflection.Assembly.GetExecutingAssembly().FullName)
                    {
                        Type[] types = asssembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.Name.IndexOf("SemanticNodeConverter") >= 0)
                            {
                                Object obj = Activator.CreateInstance(type);
                                if (obj is ISemanticNodeConverter)
                                {
                                    Langs.Add(obj as ISemanticNodeConverter);                                    
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {                    
                }
            }
            return Langs;
        }

        public void GetGUI(List<IPluginGUIItem> MenuItems, List<IPluginGUIItem> ToolBarItems)
        {
            PluginGUIItem Item = new PluginGUIItem(StringsPrefix + "BUILD", StringsPrefix + "BUILD", TextFormatterForm.ImageBuild.Image, TextFormatterForm.ImageBuild.BackColor, ExecuteBuild);
            MenuItems.Add(Item);
            ToolBarItems.Add(Item);
            Item = new PluginGUIItem(StringsPrefix + "OPTIONS", StringsPrefix + "OPTIONS", TextFormatterForm.ImageOptions.Image, TextFormatterForm.ImageOptions.BackColor, ExecuteOptions);
            MenuItems.Add(Item);
            ToolBarItems.Add(Item);
        }

    }
}

