using System;
using System.Collections;
using ExpertLib;
namespace ExpertLib.Formula
{
    /// <summary>
    /// 公式计算类
    /// </summary>
	public static class Evaluator 
	{
        /// <summary>
        /// 求表达式值
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
		public static Variant Evaluate(string s)
		{
			ExecutionQueue eq = ParseIt(s);
			return CalcIt(eq);
		}

        /// <summary>
        /// 分析表达式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
		public static ExecutionQueue ParseIt(string s)
		{
			Parser p = new Parser();
			p.ParseIt(s);
			return p.eqResult;
		}

        /// <summary>
        /// 计算分析链
        /// </summary>
        /// <param name="eq"></param>
        /// <returns></returns>
		public static Variant CalcIt(ExecutionQueue eq)
		{
			Calculator c = new Calculator();
			return c.CalcIt(eq);
		}

        /// <summary>
        /// 公共计算变量哈希表
        /// </summary>
		public static Hashtable Variables
		{
			get
			{
				return Calculator.Variables;
			}
			set
			{
				Calculator.Variables = value;
			}
		}

	
	}
}
