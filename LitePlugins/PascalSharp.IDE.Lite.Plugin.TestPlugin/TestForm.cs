using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PascalSharp.Compiler;
using PascalSharp.Internal.Localization;

namespace VisualPascalABCPlugins
{
    public partial class TestForm : Form
    {
        public IVisualEnvironmentCompiler VisualEnvironmentCompiler;
        
        public TestForm()
        {
            InitializeComponent();
            StringResources.SetTextForAllObjects(this, TestPlugin_VisualPascalABCPlugin.StringsPrefix);
        }

        private void SyntaxTreeVisualisatorForm_Load(object sender, EventArgs e)
        {

        }

        private void SyntaxTreeVisualisatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void SyntaxTreeVisualisatorForm_Shown(object sender, EventArgs e)
        {
            VisualEnvironmentCompiler.Compiler.OnChangeCompilerState += new ChangeCompilerStateEventDelegate(Compiler_OnChangeCompilerState);
        }

        void Compiler_OnChangeCompilerState(ICompiler sender, CompilerState State, string FileName)
        {
            //compiler states
        }
    }
}
