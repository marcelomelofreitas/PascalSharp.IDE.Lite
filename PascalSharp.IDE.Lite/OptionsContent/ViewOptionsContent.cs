// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PascalSharp.Internal.CompilerTools.Errors;
using PascalSharp.Internal.Localization;
using VisualPascalABCPlugins;


namespace VisualPascalABC.OptionsContent
{
    public partial class ViewOptionsContent : UserControl,IOptionsContent
    {
        Form1 MainForm;
        string strprefix = "VP_OC_VIEWOPTIONS_";
        public ViewOptionsContent(Form1 MainForm)
        {
            this.MainForm = MainForm;
            InitializeComponent();
            StringResources.SetTextForAllObjects(this, strprefix);
            foreach (string lng in StringResourcesLanguage.AccessibleLanguages)
            {
                languageSelect.Items.Add(lng);
            }
            if (StringResourcesLanguage.AccessibleLanguages.Count == 0)
                languageSelect.Enabled = false;
        }

        private bool alreadyShown;
        #region IOptionsContent Members

        public string ContentName
        {
            get
            {
                return StringResources.Get(strprefix + "NAME");
            }
        }
        public string Description
        {
            get
            {
                return StringResources.Get(strprefix + "DESCRIPTION");
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
                        languageSelect.SelectedItem = StringResourcesLanguage.CurrentLanguageName;
                        cbSaveFilesIfComilationOk.Checked = MainForm.UserOptions.SaveSourceFilesIfComilationOk;
                        cbPauseInRunModeIfConsole.Checked = MainForm.UserOptions.PauseInRunModeIfConsole;


                        cbErrorsStrategy.Items.Clear();
                        //cbErrorsStrategy.Items.Add(StringResources.Get(strprefix + "ES_ALL"));
                        cbErrorsStrategy.Items.Add(StringResources.Get(strprefix + "ES_FIRSTONLY"));
                        cbErrorsStrategy.Items.Add(StringResources.Get(strprefix + "ES_FIRSTSEMANTICANDSYNTAX"));
                        switch (MainForm.ErrorsManager.Strategy)
                        {
                            /*case ErrorsStrategy.All:
                                cbErrorsStrategy.SelectedItem = cbErrorsStrategy.Items[1];
                                break;*/
                            case ErrorsStrategy.FirstOnly:
                                cbErrorsStrategy.SelectedItem = cbErrorsStrategy.Items[0];
                                break;
                            case ErrorsStrategy.FirstSemanticAndSyntax:
                                cbErrorsStrategy.SelectedItem = cbErrorsStrategy.Items[1];
                                break;
                            default:
                                cbErrorsStrategy.SelectedItem = cbErrorsStrategy.Items[1];
                                break;
                        }

                        cbShowDebugPlayPauseButtons.Checked = MainForm.PlayPauseButtonsVisibleInPanel;
                        alreadyShown = true;
                    }
                    break;
                case OptionsContentAction.Ok:
                    UserOptions UsOpt = MainForm.UserOptions;
                    UsOpt.SaveSourceFilesIfComilationOk = cbSaveFilesIfComilationOk.Checked;
                    MainForm.ErrorsManager.Strategy = (ErrorsStrategy)cbErrorsStrategy.Items.IndexOf(cbErrorsStrategy.SelectedItem);
                    switch (cbErrorsStrategy.Items.IndexOf(cbErrorsStrategy.SelectedItem))
                    {
                    	case 0:
                    		MainForm.ErrorsManager.Strategy = ErrorsStrategy.FirstOnly;
                            break;
                        case 1:
                           	MainForm.ErrorsManager.Strategy = ErrorsStrategy.FirstSemanticAndSyntax;
                            break; 
                    }
                    /*switch (cbErrorsStrategy.Items.IndexOf(cbErrorsStrategy.SelectedItem))
                    {
                        case 0:
                            MainForm.ErrorsManager.Strategy = ErrorsStrategy.All;
                            break;
                        case 1:
                            MainForm.ErrorsManager.Strategy = ErrorsStrategy.FirstOnly;
                            break;
                        case 2:
                            MainForm.ErrorsManager.Strategy = ErrorsStrategy.FirstSemanticAndSyntax;
                            break;
                    }            
                    */
                    UsOpt.PauseInRunModeIfConsole = cbPauseInRunModeIfConsole.Checked;
                    MainForm.UpdateUserOptions();
                    MainForm.PlayPauseButtonsVisibleInPanel = cbShowDebugPlayPauseButtons.Checked;
                    alreadyShown = false;
                    WorkbenchServiceFactory.OptionsService.SaveOptions();
                    //this.Enabled = true;           
                    break;
                case OptionsContentAction.Cancel:
                    alreadyShown = false;
                    break;
            }

        }
        #endregion

        private void languageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringResourcesLanguage.CurrentLanguageName = (string)languageSelect.SelectedItem;
            CodeCompletionParserController.CurrentTwoLetterISO = StringResourcesLanguage.CurrentTwoLetterISO;
        }

        private void cbShowDebugPlayPauseButtons_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.PlayPauseButtonsVisibleInPanel = cbShowDebugPlayPauseButtons.Checked;
        }

    }
}
