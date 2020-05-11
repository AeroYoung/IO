using System;
using ExpertLib;
namespace ExpertLib.Formula
{
	/// <summary>
	/// ¼æÈİVBµÄ·ºÀàĞÍvariant
	/// </summary>
    public enum VariantType {vtUnknow,vtBool,vtInt,vtDouble,vtString,vtDateTime};
	
    
    public class VariantException: System.Exception{} 
	
	public class Variant
	{
		private VariantType varType; 
		public object oValue;

		public VariantType VarType{get{return varType;}}
		
		public Variant()
		{
			varType = VariantType.vtUnknow;
		}

		public Variant(object o) 
		{		
			if(o is bool) 
			{
				oValue = (bool)o;
				varType = VariantType.vtBool;
				return;
			}
			if(o is int) 
			{
				oValue = (int)o;
				varType = VariantType.vtInt;
				return;
			}
			if(o is double) 
			{
				oValue = (double)o;
				varType = VariantType.vtDouble;
				return;
			}
			if(o is DateTime) 
			{
				oValue = (DateTime)o;
				varType = VariantType.vtDateTime;
				return;
			}
			if(o is string) 
			{
				oValue = (string)o;
				varType = VariantType.vtString;
				return;
			}
			if(o is Variant)
			{
				oValue = ((Variant)o).oValue;
				varType = ((Variant)o).VarType;
				return;
			}
			if(o is System.Decimal) 
			{
				oValue = (int)(decimal)o;
				varType = VariantType.vtInt;
				return;
			}
			if(o is long) 
			{
				oValue = (int)(long)o;
				varType = VariantType.vtInt;
				return;
			}
			if(o is float) 
			{
				oValue = (double)(float)o;
				varType = VariantType.vtDouble;
				return;
			}
			throw(new CalcException("Invalid object type for Variant conversion"));
		}

		public Variant(Variant v): this ((object) v){}		
		public Variant(bool b): this ((object)b){} 
		public Variant(int i): this ((object)i){}
		public Variant(double d): this ((object)d){}
		public Variant(string s): this ((object)s){}
		public Variant(DateTime dt): this ((object)dt){}

		public override string ToString()
		{
			switch(varType)
			{
				case VariantType.vtUnknow : return ""; 
				case VariantType.vtBool : return ((bool)oValue).ToString();
				case VariantType.vtInt : return ((int)oValue).ToString();
				case VariantType.vtDouble : return ((double)oValue).ToString();
				case VariantType.vtString : return (string)oValue.ToString();
				case VariantType.vtDateTime : return ((DateTime)oValue).ToString();
				default : return "";
			}
		}

		public override int GetHashCode()
		{
			return oValue.GetHashCode();
		}
		public override bool Equals(object o)
		{
			return false;
		}
	
		public static implicit operator bool(Variant v)
		{
			switch(v.VarType)
			{
				case VariantType.vtBool:
					return (bool)v.oValue;
				default:
					throw new CalcException("Bad typecast from " + v.VarType + " to bool");
			}
		}

		public static implicit operator int(Variant v)
		{
			switch(v.VarType)
			{
				case VariantType.vtInt:
					return (int)v.oValue;
				case VariantType.vtDouble:
					return (int)(double)v.oValue;
				default:
					throw new CalcException("Bad typecast from " + v.VarType + " to int");
			}
		}
		public static implicit operator double(Variant v)
		{
			switch(v.VarType)
			{
				case VariantType.vtInt:
					return (double)(int)v.oValue;
				case VariantType.vtDouble:
					return (double)v.oValue;
				default:
					throw new CalcException("Bad typecast from " + v.VarType + " to double");
			}
		}
		public static implicit operator DateTime(Variant v)
		{
			switch(v.VarType)
			{
				case VariantType.vtDateTime:
					return (DateTime)v.oValue;
				default:
					throw new CalcException("Bad typecast from " + v.VarType + " to DateTime");
			}
		}

		public static implicit operator string(Variant v)
		{
			if(v.varType == VariantType.vtUnknow)
				return "";
			else
				return v.ToString();
		}

		public static implicit operator Variant(string s)
		{
			return new Variant(s);
		}

		public static Variant operator -(Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtInt:			
					return new Variant(-(int)a.oValue);
				case VariantType.vtDouble:
					return new Variant(-(double)a.oValue);
				case VariantType.vtBool:
					return new Variant(!(bool)a.oValue);
				default: 
					throw(new CalcException("Bad operand type for UnMinus operator"));
			}
		}	

		public static Variant operator !(Variant a)
		{
			return new Variant(-a);
		}	

		public static Variant operator +(Variant a)
		{
			return new Variant(a);
		}	

		public static Variant operator +(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtBool: 
					switch(b.varType)
					{
						case VariantType.vtString: return new Variant(a.ToString() + b.ToString());
						default: 
							throw(new CalcException("Bad 2-nd operand type with plus operator"));
					}
				case VariantType.vtInt:			
					switch(b.varType)
					{
						case VariantType.vtBool: 
							throw(new CalcException("Bad 2-st operand bool type with plus operator"));
						case VariantType.vtInt: 
							return new Variant((int)a.oValue + (int)b.oValue);
						case VariantType.vtDouble: 
							return new Variant((int)a.oValue + (double)b.oValue);
						case VariantType.vtString: 
							return new Variant(a.ToString() + b.ToString());
						case VariantType.vtDateTime: 
							throw(new CalcException("Bad 2-st operand datetime type with plus operator"));		
						default: 
							throw(new CalcException("Bad 2-nd operand type with plus operator"));
					}
				case VariantType.vtDouble:
					switch(b.varType)
					{
						case VariantType.vtBool: 
							throw(new CalcException("Bad 2-st operand bool type with plus operator"));
						case VariantType.vtInt: 
							return new Variant((double)a.oValue + (int)b.oValue);
						case VariantType.vtDouble: 
							return new Variant((double)a.oValue + (double)b.oValue);
						case VariantType.vtString: 
							return new Variant(a.ToString() + b.ToString());
						case VariantType.vtDateTime: 
							throw(new CalcException("Bad 2-st operand datetime type with plus operator"));		
						default: 
							throw(new CalcException("Bad 2-nd operand type with plus operator"));
					}
				case VariantType.vtString:
					switch(b.varType)
					{
						case VariantType.vtBool: 
						case VariantType.vtInt: 
						case VariantType.vtDouble: 
						case VariantType.vtString: 
						case VariantType.vtDateTime: 
							return new Variant(a.ToString() + b.ToString());
						default: 
							throw(new CalcException("Bad 2-nd operand type with plus operator"));
					}
				default: 
					throw(new CalcException("Bad 1-st operand type with plus operator"));
			}
		}

		public static Variant operator -(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtInt:			
					switch(b.varType)
					{
						case VariantType.vtInt: 
							return new Variant((int)a.oValue - (int)b.oValue);
						case VariantType.vtDouble: 
							return new Variant((int)a.oValue - (double)b.oValue);
						default: 
							throw(new CalcException("Bad 2-nd operand type with plus operator"));
					}
				case VariantType.vtDouble:
					switch(b.varType)
					{
						case VariantType.vtInt: 
							return new Variant((double)a.oValue + (int)b.oValue);
						case VariantType.vtDouble: 
							return new Variant((double)a.oValue + (double)b.oValue);
						default: 
							throw(new CalcException("Bad 2-nd operand type with minus operator"));
					}
				default: 
					throw(new CalcException("Bad 1-st operand type with plus operator"));
			}
		}	
		public static Variant operator *(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtInt:			
				switch(b.varType)
				{
					case VariantType.vtInt: 
						return new Variant((int)a.oValue * (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((int)a.oValue * (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with mul operator"));
				}
				case VariantType.vtDouble:
				switch(b.varType)
				{
					case VariantType.vtInt: 
						return new Variant((double)a.oValue * (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((double)a.oValue * (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with mul operator"));
				}
				default: 
					throw(new CalcException("Bad 1-st operand type with mul operator"));
			}
		}		
		public static Variant operator /(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtInt:			
				switch(b.varType)
				{
					case VariantType.vtInt: 
						if((int)b.oValue == 0) return new Variant(0);
						else return new Variant((int)a.oValue / (int)b.oValue);
					case VariantType.vtDouble: 
						if((double)b.oValue == 0) return new Variant(0.0);
						else return new Variant((int)a.oValue / (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with div operator"));
				}
				case VariantType.vtDouble:
				switch(b.varType)
				{
					case VariantType.vtInt: 
						if((int)b.oValue == 0) return new Variant(0.0);
						else return new Variant((double)a.oValue / (int)b.oValue);
					case VariantType.vtDouble: 
						if((double)b.oValue == 0) return new Variant(0.0);
						else return new Variant((double)a.oValue / (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with div operator"));
				}
				default: 
					throw(new CalcException("Bad 1-st operand type with div operator"));
			}
		}		
		public static Variant operator >(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtBool:
				switch(b.varType)
				{
					case VariantType.vtBool:
						if((bool)a.oValue && !(bool)b.oValue)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with > operator"));
				}
				case VariantType.vtInt:			
				switch(b.varType)
				{
					case VariantType.vtBool: 
						throw(new CalcException("Bad 2-st operand bool type with > operator"));
					case VariantType.vtInt: 
						return new Variant((int)a.oValue > (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((int)a.oValue > (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with > operator"));
				}
				case VariantType.vtDouble:
				switch(b.varType)
				{
					case VariantType.vtInt: 
						return new Variant((double)a.oValue > (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((double)a.oValue > (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with > operator"));
				}
				case VariantType.vtString:
				switch(b.varType)
				{
					case VariantType.vtString:
						if(String.Compare(a.ToString(),b.ToString(),true) > 0)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with > operator"));
				}
				case VariantType.vtDateTime:
				switch(b.varType)
				{
					case VariantType.vtDateTime: 
						return new Variant((DateTime)a.oValue > (DateTime)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with > operator"));
				}
				default: 
					throw(new CalcException("Bad 1-st operand type with > operator"));
			}
		}

		public static Variant operator <(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtBool:
				switch(b.varType)
				{
					case VariantType.vtBool:
						if(!(bool)a.oValue && (bool)b.oValue)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with < operator"));
				}
				case VariantType.vtInt:			
				switch(b.varType)
				{
					case VariantType.vtBool: 
						throw(new CalcException("Bad 2-st operand bool type with < operator"));
					case VariantType.vtInt: 
						return new Variant((int)a.oValue < (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((int)a.oValue < (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with < operator"));
				}
				case VariantType.vtDouble:
				switch(b.varType)
				{
					case VariantType.vtInt: 
						return new Variant((double)a.oValue < (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((double)a.oValue < (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with < operator"));
				}
				case VariantType.vtString:
				switch(b.varType)
				{
					case VariantType.vtString:
						if(String.Compare(a.ToString(),b.ToString(),true) < 0)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with < operator"));
				}
				case VariantType.vtDateTime:
				switch(b.varType)
				{
					case VariantType.vtDateTime: 
						return new Variant((DateTime)a.oValue < (DateTime)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with < operator"));
				}
				default: 
					throw(new CalcException("Bad 1-st operand type with < operator"));
			}
		}

		public static Variant operator >=(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtBool:
				switch(b.varType)
				{
					case VariantType.vtBool:
						if(!(bool)a.oValue && (bool)b.oValue)
							return new Variant(false);
						else
							return new Variant(true);
					default: 
						throw(new CalcException("Bad 2-nd operand type with >= operator"));
				}
				case VariantType.vtInt:			
				switch(b.varType)
				{
					case VariantType.vtBool: 
						throw(new CalcException("Bad 2-st operand bool type with >= operator"));
					case VariantType.vtInt: 
						return new Variant((int)a.oValue >= (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((int)a.oValue >= (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with >= operator"));
				}
				case VariantType.vtDouble:
				switch(b.varType)
				{
					case VariantType.vtInt: 
						return new Variant((double)a.oValue >= (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((double)a.oValue >= (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with >= operator"));
				}
				case VariantType.vtString:
				switch(b.varType)
				{
					case VariantType.vtString:
						if(String.Compare(a.ToString(),b.ToString(),true) >= 0)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with >= operator"));
				}
				case VariantType.vtDateTime:
				switch(b.varType)
				{
					case VariantType.vtDateTime: 
						return new Variant((DateTime)a.oValue >= (DateTime)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with >= operator"));
				}
				default: 
					throw(new CalcException("Bad 1-st operand type with >= operator"));
			}
		}
		public static Variant operator <=(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtBool:
				switch(b.varType)
				{
					case VariantType.vtBool:
						if((bool)a.oValue && !(bool)b.oValue)
							return new Variant(false);
						else
							return new Variant(true);
					default: 
						throw(new CalcException("Bad 2-nd operand type with <= operator"));
				}
				case VariantType.vtInt:			
				switch(b.varType)
				{
					case VariantType.vtBool: 
						throw(new CalcException("Bad 2-st operand bool type with <= operator"));
					case VariantType.vtInt: 
						return new Variant((int)a.oValue <= (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((int)a.oValue <=(double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with <= operator"));
				}
				case VariantType.vtDouble:
				switch(b.varType)
				{
					case VariantType.vtInt: 
						return new Variant((double)a.oValue <= (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((double)a.oValue <= (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with <= operator"));
				}
				case VariantType.vtString:
				switch(b.varType)
				{
					case VariantType.vtString:
						if(String.Compare(a.ToString(),b.ToString(),true) <= 0)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with <= operator"));
				}
				case VariantType.vtDateTime:
				switch(b.varType)
				{
					case VariantType.vtDateTime: 
						return new Variant((DateTime)a.oValue <= (DateTime)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with <= operator"));
				}
				default: 
					throw(new CalcException("Bad 1-st operand type with <= operator"));
			}
		}

		public static Variant operator ==(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtBool:
				switch(b.varType)
				{
					case VariantType.vtBool:
						if((bool)a.oValue ^ (bool)b.oValue)
							return new Variant(false);
						else
							return new Variant(true);
					default: 
						throw(new CalcException("Bad 2-nd operand type with == operator"));
				}
				case VariantType.vtInt:			
				switch(b.varType)
				{
					case VariantType.vtBool: 
						throw(new CalcException("Bad 2-st operand bool type with == operator"));
					case VariantType.vtInt: 
						return new Variant((int)a.oValue == (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((int)a.oValue ==(double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with == operator"));
				}
				case VariantType.vtDouble:
				switch(b.varType)
				{
					case VariantType.vtInt: 
						return new Variant((double)a.oValue == (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((double)a.oValue == (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with == operator"));
				}
				case VariantType.vtString:
				switch(b.varType)
				{
					case VariantType.vtString:
						if(String.Compare(a.ToString(),b.ToString(),true) == 0)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with == operator"));
				}
				case VariantType.vtDateTime:
				switch(b.varType)
				{
					case VariantType.vtDateTime: 
						return new Variant((DateTime)a.oValue == (DateTime)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with == operator"));
				}
				default: 
					throw(new CalcException("Bad 1-st operand type with == operator"));
			}
		}
		public static Variant operator !=(Variant b,  Variant a)
		{
			switch(a.varType)
			{
				case VariantType.vtBool:
				switch(b.varType)
				{
					case VariantType.vtBool:
						if((bool)a.oValue ^ (bool)b.oValue)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with != operator"));
				}
				case VariantType.vtInt:			
				switch(b.varType)
				{
					case VariantType.vtBool: 
						throw(new CalcException("Bad 2-st operand bool type with != operator"));
					case VariantType.vtInt: 
						return new Variant((int)a.oValue != (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((int)a.oValue !=(double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with != operator"));
				}
				case VariantType.vtDouble:
				switch(b.varType)
				{
					case VariantType.vtInt: 
						return new Variant((double)a.oValue != (int)b.oValue);
					case VariantType.vtDouble: 
						return new Variant((double)a.oValue != (double)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with != operator"));
				}
				case VariantType.vtString:
				switch(b.varType)
				{
					case VariantType.vtString:
						if(String.Compare(a.ToString(),b.ToString(),true) != 0)
							return new Variant(true);
						else
							return new Variant(false);
					default: 
						throw(new CalcException("Bad 2-nd operand type with != operator"));
				}
				case VariantType.vtDateTime:
				switch(b.varType)
				{
					case VariantType.vtDateTime: 
						return new Variant((DateTime)a.oValue <= (DateTime)b.oValue);
					default: 
						throw(new CalcException("Bad 2-nd operand type with != operator"));
				}
				default: 
					throw(new CalcException("Bad 1-st operand type with != operator"));
			}
		}


	}	
}
