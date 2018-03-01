// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class WordCount : AbstractMenuCommand
	{
		public override void Run()
		{
			using (WordCountDialog wcd = new WordCountDialog()) {
				wcd.ShowDialog(WorkbenchSingleton.MainWin32Window);
			}
		}
	}
}
