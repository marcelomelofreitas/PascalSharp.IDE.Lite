﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Resources;
using System.Reflection;

[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: AssemblyCompany("ic#code")]
[assembly: AssemblyProduct("SharpDevelop")]
[assembly: AssemblyCopyright("2000-2012 AlphaSierraPapa for the SharpDevelop Team")]
[assembly: AssemblyVersion(RevisionClass.Major + "." + RevisionClass.Minor + "." + RevisionClass.Build + "." + RevisionClass.Revision)]
[assembly: AssemblyInformationalVersion(RevisionClass.FullVersion)]
[assembly: NeutralResourcesLanguage("en-US")]

internal static class RevisionClass
{
	public const string Major = "4";
	public const string Minor = "2";
	public const string Build = "1";
	public const string Revision = "0";
	public const string VersionName = null;
	
	public const string FullVersion = Major + "." + Minor + "." + Build + ".0";
}
