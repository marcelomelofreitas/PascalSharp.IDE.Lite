// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PascalSharp.Internal.Localization;

namespace VisualPascalABC.OptionsContent
{
    public partial class IntelliseseOptionsContent : UserControl, IOptionsContent
    {
        Form1 MainForm;
        string strprefix = "VP_OC_INTELLISENSEOPTIONS_";
        public IntelliseseOptionsContent(Form1 MainForm)
        {
            this.MainForm = MainForm;
            InitializeComponent();
            StringResources.SetTextForAllObjects(this, strprefix);
        }

        private bool alreadyShown;

        #region IOptionsContent Members

        public string ContentName
        {
            get
            {
                return StringResources.Get(strprefix+"NAME");
            }
        }
        public string Description
        {
            get
            {
                return StringResources.Get(strprefix+"DESCRIPTION");
            }
        }

        public UserControl Content
        {
            get { return this; }
        }

        public void Action(OptionsContentAction action)
        {
            switch (action)
            {
                case OptionsContentAction.Show:
                    if (!alreadyShown)
                    {
                        cbAllowChecked = MainForm.UserOptions.AllowCodeCompletion;
                        cbCodeCompletionDot.Checked = MainForm.UserOptions.CodeCompletionDot;
                        cbCodeCompletionHint.Checked = MainForm.UserOptions.CodeCompletionHint;
                        cbCodeCompletionParams.Checked = MainForm.UserOptions.CodeCompletionParams;
                        cbCodeCompletionKeyPressed.Checked = MainForm.UserOptions.EnableSmartIntellisense;
                        nuNamespaceVisibleRange.Value = MainForm.UserOptions.CodeCompletionNamespaceVisibleRange;
                        cbIntellisencePanel.Checked = MainForm.UserOptions.ShowQuickClassBrowserPanel;
                        alreadyShown = true;
                    }
                    break;
                case OptionsContentAction.Ok:
                    MainForm.UserOptions.CodeCompletionDot = cbCodeCompletionDot.Checked;
                    MainForm.UserOptions.CodeCompletionHint = cbCodeCompletionHint.Checked;
                    MainForm.UserOptions.CodeCompletionParams = cbCodeCompletionParams.Checked;
                    MainForm.UserOptions.AllowCodeCompletion = cbAllowChecked;
                    MainForm.UserOptions.EnableSmartIntellisense = cbCodeCompletionKeyPressed.Checked;
                    MainForm.UserOptions.CodeCompletionNamespaceVisibleRange = Convert.ToInt32(nuNamespaceVisibleRange.Value);
                    MainForm.UserOptions.ShowQuickClassBrowserPanel = cbIntellisencePanel.Checked;
                    MainForm.UpdateUserOptions();
                    alreadyShown = false;
                    WorkbenchServiceFactory.OptionsService.SaveOptions();
                    break;
                case OptionsContentAction.Cancel:
                    alreadyShown = false;
                    break;
            }

        }
        #endregion

        private void cbAllowCodeCompletion_CheckedChanged(object sender, EventArgs e)
        {
            cbIntellisencePanel.Checked = cbCodeCompletionKeyPressed.Checked = cbCodeCompletionDot.Checked = cbCodeCompletionHint.Checked = cbCodeCompletionParams.Checked = cbAllowChecked;
            cbIntellisencePanel.Enabled = cbCodeCompletionKeyPressed.Enabled = nuNamespaceVisibleRange.Enabled = cbCodeCompletionDot.Enabled = cbCodeCompletionHint.Enabled = cbCodeCompletionParams.Enabled = cbAllowChecked;
        }
 
    }
}
