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
using PascalABCCompiler.Parsers;
using PascalSharp.Internal.CodeCompletion;
using KeywordKind = PascalABCCompiler.Parsers.KeywordKind;

namespace VisualPascalABC
{
	/// <summary>
	/// Класс, предназначенный для хранения последних выбранных методов в Intellisense
	/// </summary>
	public class CompletionDataDispatcher
    {
    	private static Hashtable dict = new Hashtable(StringComparer.OrdinalIgnoreCase);
    	
    	public static void AddLastUsedItem(ICompletionData item)
    	{
    		if (item != null && item.Text != null && item.Text.Length > 0)
    			dict[item.Text[0].ToString()] = item;
    	}
    	
    	public static ICompletionData GetLastUsedItem(char ch)
    	{
    		return dict[ch.ToString()] as ICompletionData;
    	}
    	
    	private static SymScope cur_sc;
    	private static Hashtable ht = new Hashtable();
    	
    	public static void AddMemberBeforeDot(SymScope sc)
    	{
    		cur_sc = sc;
    	}
    	
    	public static void BindMember(ICompletionData item)
    	{
    		if (cur_sc != null && item != null)
    			ht[cur_sc.GetFullName()] = item.Text;
    		cur_sc = null;
    	}
    	
    	public static string GetRecentUsedMember(SymScope sc)
    	{
    		return ht[sc.GetFullName()] as string;
    	}
    }
    
    public class KeywordChecker
    {
    	public static KeywordKind TestForKeyword(string Text, int i)
        {
            if (CodeCompletionController.CurrentParser != null && CodeCompletionController.CurrentParser.LanguageInformation !=null)
        	    return CodeCompletionController.CurrentParser.LanguageInformation.TestForKeyword(Text, i);
        	return KeywordKind.None;
        } 
        
    }
    
    /// <summary>
    /// Класс-диспетчер, прицепляющий документацию к методам и классам.
    /// Документация прицепляется в отдельном потоке
    /// </summary>
    public class DefaultDispatcher : AbstractDispatcher
    {
        private Dictionary<SymInfo, ICompletionData> dict = new Dictionary<SymInfo, ICompletionData>();
        private Dictionary<string, ICompletionData> dict2 = new Dictionary<string, ICompletionData>();
    	public void Reset()
    	{
    		dict.Clear();
            dict2.Clear();
    	}

        public void Add(SymInfo si, ICompletionData data)
    	{
    		if (!dict.ContainsKey(si))
    		    dict[si] = data;
            if (!string.IsNullOrEmpty(si.description) && !dict2.ContainsKey(si.description))
                dict2[si.description] = data;
    	}

        public override void Update(SymInfo si)
		{
    		ICompletionData val=null;
    		if (dict.TryGetValue(si,out val))
    		{
    			val.Description = si.description;
    		}
            else if (si.description != null && dict2.TryGetValue(si.description.Split('\n')[0], out val))
            {
                val.Description = si.description;
            }
		}
    }
}

