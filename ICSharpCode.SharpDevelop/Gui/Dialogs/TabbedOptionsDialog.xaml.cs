﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	/// Displays TabbedOptions in a dialog.
	/// </summary>
	public partial class TabbedOptionsDialog : Window
	{
		TabbedOptions tabbedOptions = new TabbedOptions();
		
		public TabbedOptionsDialog(IEnumerable<IOptionPanelDescriptor> optionPanels)
		{
			InitializeComponent();
			tabbedOptions.AddOptionPanels(optionPanels);
			grid.Children.Add(tabbedOptions);
		}
		
		void okButtonClick(object sender, RoutedEventArgs e)
		{
			foreach (IOptionPanel op in tabbedOptions.OptionPanels) {
				if (!op.SaveOptions())
					return;
			}
			this.DialogResult = true;
			Close();
		}
		
		void cancelButtonClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			foreach (IDisposable op in tabbedOptions.OptionPanels.OfType<IDisposable>()) {
				op.Dispose();
			}
		}
	}
}
