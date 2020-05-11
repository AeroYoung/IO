using System;
using System.Collections;
using ExpertLib;

namespace ExpertLib.Formula
{
	/// <summary>
	/// Summary description for Calculator.
	/// </summary>
	/// 
	public class CalcException : Exception
	{
		public CalcException(){}
		public CalcException(string s) : base(s){}
	}

	public class Calculator
	{
		private CalcStack calcStack;
		public static Hashtable Variables;
		public  Calculator()
		{
			calcStack = new CalcStack();
		}

		public  Variant CalcIt(ExecutionQueue eq)
		{
			ExecutionItem ei;
			if(eq.Count == 0) return new Variant("");
			while(eq.Count > 0)
			{
				ei = eq.Dequeue();
				switch(ei.itemType)
				{
					case ItemType.itBool:
						if(String.Compare(ei.itemString,"True",true) == 0)
							calcStack.Push(new Variant(true));
						else
							calcStack.Push(new Variant(false));
						break;
					case ItemType.itString:
						calcStack.Push(new Variant(ei.itemString));
					break;
					case ItemType.itDigit:
						try
						{
							int i = int.Parse(ei.itemString);
							calcStack.Push(new Variant(i));
						}
						catch
						{	
							double d;							
							try {d = double.Parse(ei.itemString);}
							catch{throw(new CalcException("Bad digital format"));}
							calcStack.Push(new Variant(d));
						}
					break;
					case ItemType.itDate:
						DateTime dt;
						try {dt = DateTime.Parse(ei.itemString);}
						catch {throw(new CalcException("Bad date format"));}
						calcStack.Push(new Variant(dt));
					break;
					case ItemType.itOperator:
						DoOperator(ei.itemOperator,eq);
					break;
					case ItemType.itFunction:
						DoFunction(ei);
						break;
					case ItemType.itVariable:
						if(!Variables.ContainsKey(ei.itemString)) throw(new CalcException("Bad variable name !"));
						calcStack.Push(new Variant(Variables[ei.itemString]));
						break;
					default:
						throw(new CalcException("Bad item in execution queue"));
				}
			}
			return calcStack.Pop();
		}

		private void DoFunction(ExecutionItem ei)
		{
			Variant v;
			FunctionDesc fd = new FunctionDesc(ei.itemString);
			foreach(string s in ei.itemParams)
			{
				Parser p = new Parser();
				p.ParseIt(s);
				ExecutionQueue eq;
				eq = p.eqResult;
				Calculator c = new Calculator();
				fd.Add(c.CalcIt(eq));
			}
			v = EmbeddedFunction(fd);
			if(v.VarType != VariantType.vtUnknow) calcStack.Push(v);
			else throw(new CalcException("Bad function " + ei.itemString));
		}
		private void DoOperator(EnmOperators op, ExecutionQueue eq)
		{
			switch(op)
			{
				case EnmOperators.UnMinus:
				case EnmOperators.Nop:
				case EnmOperators.Not:
				case EnmOperators.UnPlus:
					if(calcStack.Count < 1) throw(new CalcException("Stack is empty on " + op.ToString()));	
					break;
				default:
					if(calcStack.Count < 2) throw(new CalcException("Stack is empty on " + op.ToString()));	
					break;
			}

			switch(op)
			{
				case EnmOperators.UnMinus:
				case EnmOperators.Not:
					calcStack.Push(-calcStack.Pop());
					break;
				case EnmOperators.UnPlus:
					break;
				case EnmOperators.Plus:
					calcStack.Push(calcStack.Pop() + calcStack.Pop());
					break;
				case EnmOperators.Minus:
					calcStack.Push(calcStack.Pop() - calcStack.Pop());
					break;
				case EnmOperators.Mul:
					calcStack.Push(calcStack.Pop() * calcStack.Pop());
					break;
				case EnmOperators.Div:
					calcStack.Push(calcStack.Pop() / calcStack.Pop());
					break;
				case EnmOperators.Gr:
					calcStack.Push(calcStack.Pop() > calcStack.Pop());
					break;	
				case EnmOperators.Ls:
					calcStack.Push(calcStack.Pop() < calcStack.Pop());
					break;	
				case EnmOperators.GrEq:
					calcStack.Push(calcStack.Pop() >= calcStack.Pop());
					break;	
				case EnmOperators.LsEq:
					calcStack.Push(calcStack.Pop() <= calcStack.Pop());
					break;	
				case EnmOperators.Eq:
					calcStack.Push(calcStack.Pop() == calcStack.Pop());
					break;	
				case EnmOperators.NtEq:
					calcStack.Push(calcStack.Pop() != calcStack.Pop());
					break;
				default:
					throw(new CalcException("Operator " + op.ToString() + " is not supported yet"));
			}
		}

		private Variant EmbeddedFunction(FunctionDesc fd)
		{
			
			switch(fd.functionName.ToUpper())
			{
				case "PI":
					return new Variant(System.Math.PI);
				case "NOW":
					return new Variant(System.DateTime.Now);
				case "SIN":
					if(fd.Count<1) throw new CalcException("Parameters count too small for SIN function");
					return new Variant(System.Math.Sin(fd[0]));
				case "COS":
					if(fd.Count<1) throw new CalcException("Parameters count too small for COS function");
					return new Variant(System.Math.Cos(fd[0]));
				case "ABS":
					if(fd.Count<1) throw new CalcException("Parameters count too small for ABS function");
					return new Variant((double)System.Math.Abs((double)fd[0]));
				case "SQRT":
					if(fd.Count<1) throw new CalcException("Parameters count too small for SQRT function");
					return new Variant(System.Math.Sqrt(fd[0]));
				case "IIF":
					if(fd.Count<3) throw new CalcException("Parameters count too small for IIF function");
					if((bool)fd[0])
						return fd[1];
					else
						return fd[2];
				case "FORMAT":
					if(fd.Count<2) throw new CalcException("Parameters count too small for FORMAT function");
					Variant v;	
					v = fd[1];
					switch(v.VarType)
					{
						case VariantType.vtBool:
							return new Variant(String.Format("{0:" + fd[0].ToString() + "}",(bool)v));
						case VariantType.vtInt:
							return new Variant(String.Format("{0:" + fd[0].ToString() + "}",(int)v));
						case VariantType.vtDouble:
							return new Variant(String.Format("{0:" + fd[0].ToString() + "}",(double)v));
						case VariantType.vtDateTime:
							return new Variant(String.Format("{0:" + fd[0].ToString() + "}",(DateTime)v));
						case VariantType.vtString:
							return new Variant(String.Format("{0:" + fd[0].ToString() + "}",(string)v));
						default:
							return new Variant("");
					}
				default: return new Variant();
			}
		}
	}
}
