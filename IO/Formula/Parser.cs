using System;
using System.Collections.Specialized;
using ExpertLib;
namespace ExpertLib.Formula
{
	public enum EnmOperators 
	{
		Nop,Plus,Minus,Mul,Div,UnMinus,UnPlus,Eq,Gr,
		Ls,GrEq,LsEq,NtEq,LeftPar,RightPar,Blank,
		Not,And,Or
	};

	public class Parser
	{
		private  OperatorStack scOp;
		public  ExecutionQueue eqResult;

		private static char [] strDividers = {'+','-','*','/','(',')','<','>','\n','\t',' ','!','&','|'} ;

		public Parser()
		{
		}

		public void ParseIt(string strForParsing)
		{
			scOp = new OperatorStack();
			eqResult = new ExecutionQueue();
			string strSrc = strForParsing;
			scOp.Clear();
			int pos = 0;
			bool isFirstOperator = true;
			EnmOperators opCurrent;
			while(pos < strSrc.Length)
			{
				if((opCurrent = IsOperator(strSrc, isFirstOperator, ref pos)) != EnmOperators.Nop)
				{
					if(opCurrent != EnmOperators.Blank)
					{
						OperatorStackManager(opCurrent);
						if(opCurrent == EnmOperators.RightPar)
							isFirstOperator = false;
						else
							isFirstOperator = true;
					}
					continue;
				}
				eqResult.Enqueue(GetOperand(strSrc, ref pos));
				isFirstOperator = false;
			}
			while(scOp.Count>0)
			{
				eqResult.Enqueue(new ExecutionItem(scOp.Pop()));
			}
		}

		private  EnmOperators IsOperator(string s,bool isFirstOperator,ref int pos)
		{
			EnmOperators curOperator = EnmOperators.Nop;
			switch (s[pos])
			{
				case ' ':
				case '\n':
				case '\t':
					pos++;
					curOperator = EnmOperators.Blank;
					break;
				case '+': 
					if(isFirstOperator) curOperator = EnmOperators.UnPlus;
					else 
					curOperator = EnmOperators.Plus;
					pos++;
					break;
				case '-': 
					if(isFirstOperator) curOperator = EnmOperators.UnMinus;
					else 
					curOperator = EnmOperators.Minus;
					pos++;
					break;
				case '*': 
					curOperator = EnmOperators.Mul;
					pos++;
					break;
				case '/': 
					curOperator = EnmOperators.Div;
					pos++;
					break;
				case '=':
					curOperator = EnmOperators.Eq;
					pos++;
					break;
				case '(':
					curOperator = EnmOperators.LeftPar;
					pos++;
					break;
				case ')':
					curOperator = EnmOperators.RightPar;
					pos++;
					break;
				case '&':
					curOperator = EnmOperators.And;
					pos++;
					break;
				case '|':
					curOperator = EnmOperators.Or;
					pos++;
					break;
				case '!':
					if((s.Length > (pos+1)) && (s[pos+1] == '='))
					{
						curOperator = EnmOperators.NtEq;
						pos+=2;
					}
					else
					{
						curOperator = EnmOperators.Not;
						pos++;
					};
					break;
				case '>':
					if((s.Length > (pos+1)) && (s[pos+1] == '='))
					{
						curOperator = EnmOperators.GrEq;
						pos+=2;
					}
					else
					{
						curOperator = EnmOperators.Gr;
						pos++;
					};
					break;
				case '<':
					if((s.Length > (pos+1)) && (s[pos+1] == '='))
					{
						curOperator = EnmOperators.LsEq;
						pos+=2;
					}
					else
						if((s.Length > (pos+1)) && (s[pos+1] == '>'))
						{
							curOperator = EnmOperators.NtEq;
							pos+=2;
						}
						else
						{
							curOperator = EnmOperators.Ls;
							pos++;
						};
					break;
			}
			return curOperator;
		}
		private  int GetPrivelege(EnmOperators op)
		{
			switch(op)
			{
				case EnmOperators.LeftPar:
					return 0;
				case EnmOperators.RightPar:
					return 1;
				case EnmOperators.Eq:
				case EnmOperators.NtEq:
				case EnmOperators.GrEq:
				case EnmOperators.LsEq:
				case EnmOperators.Gr:
				case EnmOperators.Ls:
					return 2;
				case EnmOperators.Plus:
				case EnmOperators.Minus:
				case EnmOperators.Or:
					return 3;
				case EnmOperators.Mul:
				case EnmOperators.Div:
				case EnmOperators.And:
					return 4;
				case EnmOperators.UnMinus:
				case EnmOperators.UnPlus:
				case EnmOperators.Not:
					return 5;
				default :
					return 5;			
			}
		}

		private  void OperatorStackManager(EnmOperators op)
		{
			if(scOp.Count == 0)
			{
				scOp.Push(op);
				return;
			};
			if(op == EnmOperators.LeftPar)
			{
				scOp.Push(op);
				return;
			};
			if(op == EnmOperators.RightPar)
			{
				while(scOp.Count>0)
				{
					EnmOperators opStack = scOp.Pop();
					if(opStack != EnmOperators.LeftPar)
						eqResult.Enqueue(new ExecutionItem(opStack));
					else
						return;
				};
				return;
			};

			while(scOp.Count>0)
			{
				EnmOperators opStack = scOp.Peek();
				if(GetPrivelege(op) <= GetPrivelege(opStack))
					eqResult.Enqueue(new ExecutionItem(scOp.Pop()));
				else
					break;
			}
			scOp.Push(op);
		}

		private  ExecutionItem GetOperand(string strSrc,ref int pos)
		{
			char c = strSrc[pos];
			if(c == '\"')
			{
				return 	new ExecutionItem(ItemType.itString,GetString(strSrc, ref pos)); 
			};
			if(c == '\'')
			{
				return 	new ExecutionItem(ItemType.itString,GetString2(strSrc, ref pos)); 
			};
			if(c == '[')
			{
				return new ExecutionItem(ItemType.itVariable,GetVariable(strSrc,ref pos));
			}
			if(Char.IsNumber(c) || (c == '.'))
			{
				return new ExecutionItem(ItemType.itDigit,GetNumber(strSrc,ref pos));
			}
			if(c == '#')
			{
				return new ExecutionItem(ItemType.itDate,GetDate(strSrc, ref pos));
			}

			int i = strSrc.IndexOfAny(strDividers,pos);
			if (i<0) i=strSrc.Length;
			string s = strSrc.Substring(pos,i - pos);
			pos = i;
			if((pos < strSrc.Length) && (strSrc[pos] == '('))
			{
				return new ExecutionItem(s,GetFunction(strSrc, ref pos));
			}
			if(String.Compare(s,"true",true) == 0)
				return new ExecutionItem(true);
			if(String.Compare(s,"false",true) == 0)
				return new ExecutionItem(false);

			return new ExecutionItem(ItemType.itString,s);
		}
	
		private  string GetString(string strSrc,ref int pos)
		{
			int oldPos = pos;
			pos++;
			while((pos < strSrc.Length) && (strSrc[pos] != '\"'))
			{
				pos++;
			};
			pos++;
			return strSrc.Substring(oldPos + 1, pos - oldPos - 2);
		}
		private  string GetString2(string strSrc,ref int pos)
		{
			int oldPos = pos;
			pos++;
			while((pos < strSrc.Length) && (strSrc[pos] != '\''))
			{
				pos++;
			};
			pos++;
			return strSrc.Substring(oldPos + 1, pos - oldPos - 2);
		}
		private  string GetDate(string strSrc,ref int pos)
		{
			int oldPos = pos;
			pos++;
			while((pos < strSrc.Length) && (strSrc[pos] != '#')) 
			{
				pos++;
			};
			pos++;
			return strSrc.Substring(oldPos + 1, pos - oldPos - 2);
		}
		private  string GetVariable(string strSrc,ref int pos)
		{
			int oldPos = pos;
			int lrBr = 1;
			pos++;
			while((pos < strSrc.Length))
			{
				switch(strSrc[pos])
				{
					case '[': lrBr++; break;
					case ']': lrBr--; break;
				}
				pos++;
				if(lrBr == 0) break;
			};
			return strSrc.Substring(oldPos + 1,pos - oldPos - 2);
		}
		private  string GetNumber(string strSrc,ref int pos)
		{
			int oldPos = pos;
			while((pos < strSrc.Length) && (Char.IsDigit(strSrc[pos]) || (strSrc[pos] == '.'))) pos++;
			return strSrc.Substring(oldPos,pos-oldPos);
		}		

		private  StringCollection GetFunction(string strSrc,ref int pos)
		{
			int oldPos = ++pos;
			bool noStr = true;
			int parentLevel = 1;
			StringCollection scTemp = new StringCollection();

			while(!((strSrc[pos] == ')') && (parentLevel == 1) && noStr) &&
				(strSrc.Length > pos)) 
			{
				switch(strSrc[pos])
				{
					case '\"':
						noStr = !noStr;
						break;
					case '(':
						if(noStr) parentLevel++;
						break;
					case ')':
						if(noStr) parentLevel--;
						break;
					case ',':
						if((parentLevel == 1) && noStr)
						{
							scTemp.Add(strSrc.Substring(oldPos,pos-oldPos));
							oldPos = pos + 1;
							break;
						} 
						break;
				}
				pos++;
			}
			scTemp.Add(strSrc.Substring(oldPos,pos-oldPos));
			pos++;
			return scTemp;
		}		

	}
}
