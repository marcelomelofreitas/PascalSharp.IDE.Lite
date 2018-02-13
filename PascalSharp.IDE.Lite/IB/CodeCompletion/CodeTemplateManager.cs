﻿// Copyright (c) Ivan Bondarev, Stanislav Mihalkovich (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VisualPascalABC
{
	public class CodeTemplateManager
	{
		private Hashtable ht = new Hashtable();
		
		public CodeTemplateManager()
		{
			try
			{
				StreamReader sr = File.OpenText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName),"template.pct"));
				ParseFile(sr);
				sr.Close();
			}
			catch
			{

			}
		}
		
		private string get_minimal(List<string> names)
		{
			string min = names[0];
			for (int i=1; i<names.Count; i++)
				if (names[i].Length < min.Length)
				min = names[i];
			return min;
		}
		
		public string GetTemplateHeader(string name)
		{
			List<string> lst = new List<string>();
			foreach (string s in ht.Keys)
			if (string.Compare(name,s,true)==0)
				return s;
			else if (s.StartsWith(name, StringComparison.OrdinalIgnoreCase))
				lst.Add(s);
			if (lst.Count == 1)
				return lst[0];
			else
				if (lst.Count > 1)
				return get_minimal(lst);
			else return null;
		}
		
		public string GetTemplate(string name)
		{
			return ht[name] as string;
		}
		
		private void ParseFile(StreamReader sr)
		{
			StringBuilder sb = new StringBuilder();
			string last_templ=null;
			string tmp=null;
			last_templ = sr.ReadLine().Trim('[',']',' ','\t');
			while (!sr.EndOfStream)
			{
				tmp = sr.ReadLine();
				if (tmp.StartsWith("["))
				{
					ht[last_templ] = sb.ToString().TrimEnd('\r','\n');
					last_templ = tmp.Trim('[',']',' ','\t');
					sb.Remove(0,sb.Length);
				}
				else
					sb.AppendLine(tmp);
			}
			ht[last_templ] = sb.ToString().TrimEnd('\r','\n');
		}
	}
}
