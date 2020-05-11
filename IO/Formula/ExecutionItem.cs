using System;
using System.Collections.Specialized;
using ExpertLib;
namespace ExpertLib.Formula
{
	/// <summary>
	/// Summary description for ExecutionItem.
	/// </summary>
	
	public enum ItemType 
	{
		itUnknow,itString,itDigit,itDate,itVariable,itFunction,itOperator,itBool
	}

	public class ExecutionItem
	{
		internal string itemString;
		internal ItemType itemType;
		internal EnmOperators  itemOperator;
		internal StringCollection itemParams;

		public ExecutionItem()
		{
			itemString = "";
			itemType = ItemType.itUnknow;
			itemOperator = EnmOperators.Blank;
			itemParams = null;
		}

		public ExecutionItem(bool b)
		{
			itemString = b.ToString();
			itemType = ItemType.itBool;
			itemOperator = EnmOperators.Blank;
			itemParams = null;
		}

		public ExecutionItem(ItemType itType, string s)
		{
			itemString = s;
			itemType = itType;
			itemOperator = EnmOperators.Blank;
			itemParams = null;
		}

		public ExecutionItem(EnmOperators op)
		{
			itemString = op.ToString();
			itemType = ItemType.itOperator;
			itemOperator = op;
			itemParams = null;
		}

		public ExecutionItem(string s, StringCollection param)
		{
			itemString = s;
			itemType = ItemType.itFunction;
			itemOperator = EnmOperators.Blank;
			itemParams = param;
		}

	}
}
