// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Web.Services.Description;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog
{
	internal static class ServiceReferenceHelper
	{		
		public static string GetServiceName(ServiceDescription description)
		{
			if (description.Name != null) {
				return description.Name;
			} else if (description.RetrievalUrl != null) {
				Uri uri = new Uri(description.RetrievalUrl);
				if (uri.Segments.Length > 0) {
					return uri.Segments[uri.Segments.Length - 1];
				} else {
					return uri.Host;
				}
			}
			return String.Empty;
		}
		
		public static string GetReferenceName(Uri uri)
		{
			if (uri != null) {
				return uri.Host;
			}
			return String.Empty;
		}
	}
}
