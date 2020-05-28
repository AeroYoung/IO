public enum Type
    {
        FUNC = 1,
        DATE = 2,
        NUMBER = 3,
        BOOLEAN = 4,
        STRING = 5
    }

    public class Operand
    {
        public Operand(Type type, object value)
        {
            this.Type = type;
            this.Value = value;
        }

        public Operand(string opd, object value)
        {
            this.Type = ConvertOperand(opd);
            this.Value = value;
        }
       
        public Type Type { get; set; }

        public string Key { get; set; }

        public object Value { get; set; }

        public static Type ConvertOperand(string opd)
        {
            if (opd.IndexOf("(") > -1)
            {
                return Type.FUNC;
            }
            else if (IsNumber(opd))
            {
                return Type.NUMBER;
            }
            else if (IsDate(opd))
            {
                return Type.DATE;
            }
            else
            {
                return Type.STRING;
            }
        }

        public static bool IsNumber(object value)
        {
            return Regex.IsMatch(value.ToString(), @"^[+-]?\d*[.]?\d*$");
        }

        public static bool IsDate(object value)
        {
            return DateTime.TryParse(value.ToString(), out _);
        }
    }

    public enum OptType
    {
        LB = 1,
        RB = 2,
        NOT = 3,
        PS = 5,
        NS = 7,
        TAN = 10,
        COT = 11,
        ATAN = 12,
        SIN = 13,
        COS = 14,
        Abs = 15,
        Sqrt = 16,
        Floor = 17,
        Ceiling = 18,
        Round = 19,
        Pow = 20,
        Cube = 21,
        Ln = 22,
        Log = 23,
        MUL = 130,
        DIV = 131,
        MOD = 132,
        ADD = 140,
        SUB = 141,
        LT = 150,
        LE = 151,
        GT = 152,
        GE = 153,
        ET = 160,
        UT = 161,
        AND = 170,
        OR = 171,
        CA = 180,
        END = 255,
        ERR = 256
        
    }

    public class Operator
    {
        public Operator(OptType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public OptType Type { get; set; }

        public string Value { get; set; }

        public static string AdjustOperator(string currentOpt, string currentExp, ref int currentOptPos)
        {
            #region 操作符 <>

            if (currentOpt == "<")
            {
                if (currentExp.Substring(currentOptPos, 2) == "<=")
                {
                    currentOptPos++;
                    return "<=";
                }

                if (currentExp.Substring(currentOptPos, 2) != "<>") return "<";
                currentOptPos++;
                return "<>";
            }
            else if (currentOpt == ">")
            {
                if (currentExp.Substring(currentOptPos, 2) == ">=")
                {
                    currentOptPos++;
                    return ">=";
                }

                return ">";
            }

            #endregion
            
            var firstChar = false; // currentOpt是否和函数的首字母匹配
            var funcs = new List<string>
            {
                "tan","cot", "atan", "sin", "cos",
                "abs", "sqrt",
                "floor", "ceiling",
                "round","pow","cube","ln","log"
            };

            foreach (var func in funcs)
            {
                var first = func.Substring(0, 1);
                if (currentExp.StartsWith(first, StringComparison.CurrentCultureIgnoreCase))
                    firstChar = true;

                if (!currentExp.StartsWith(func, StringComparison.CurrentCultureIgnoreCase)) 
                    continue;

                currentOptPos += func.Length - 1;
                return func;
            }

            return firstChar ? "error" : currentOpt;
        }
        
        public static OptType ConvertOperator(string opt)
        {
            opt = opt.ToLower(); // svn add 307

            switch (opt)
            {
                case "~": return OptType.NS;
                case "!": return OptType.NOT;
                case "+": return OptType.ADD;
                case "-": return OptType.SUB;
                case "*": return OptType.MUL;
                case "/": return OptType.DIV;
                case "%": return OptType.MOD;
                case "<": return OptType.LT;
                case ">": return OptType.GT;
                case "<=": return OptType.LE;
                case ">=": return OptType.GE;
                case "<>": return OptType.UT;
                case "=": return OptType.ET;
                case "&": return OptType.AND;
                case "|": return OptType.OR;
                case ",": return OptType.CA;
                case "@": return OptType.END;
                case "tan": return OptType.TAN;
                case "cot": return OptType.COT;
                case "atan": return OptType.ATAN;
                case "sin": return OptType.SIN; 
                case "cos": return OptType.COS; 
                case "abs": return OptType.Abs;
                case "sqrt": return OptType.Sqrt;
                case "floor": return OptType.Floor;
                case "ceiling": return OptType.Ceiling;
                case "round": return OptType.Round;
                case "pow": return OptType.Pow;
                case "cube": return OptType.Cube;
                case "ln": return OptType.Ln;
                case "log": return OptType.Log;

                default: return OptType.ERR;
            }
        }
        
        public static int ComparePriority(OptType optA, OptType optB)
        {
            if (optA == optB)
                return 0;

            //(*,/,%)
            if ((optA >= OptType.MUL && optA <= OptType.MOD) &&
                (optB >= OptType.MUL && optB <= OptType.MOD))
                return 0;

            //(+,-)
            if ((optA >= OptType.ADD && optA <= OptType.SUB) &&
                (optB >= OptType.ADD && optB <= OptType.SUB))
                return 0;

            //(<,<=,>,>=)
            if ((optA >= OptType.LT && optA <= OptType.GE) &&
                (optB >= OptType.LT && optB <= OptType.GE))
            {
                return 0;
            }
            //(=,<>)
            if ((optA >= OptType.ET && optA <= OptType.UT) &&
                (optB >= OptType.ET && optB <= OptType.UT))
            {
                return 0;
            }
            
            if (optA >= OptType.TAN && optA < OptType.MUL && optB >= OptType.TAN && optB < OptType.MUL)
                return 0;

            if (optA < optB)
                return 1;

            return -1;
        }
    }

    public class RPN
    {
        public Stack<object> Tokens { get; } = new Stack<object>();

        private string _RPNExpression;

        public string RPNExpression
        {
            get
            {
                if (_RPNExpression != null) return _RPNExpression;
                foreach (var item in Tokens)
                {
                    if (item is Operand operand)
                    {
                        _RPNExpression += operand.Value + ",";
                    }
                    if (item is Operator o)
                    {
                        _RPNExpression += o.Value + ",";
                    }
                }
                return _RPNExpression;
            }
        }

        public readonly List<string> m_Operators = new List<string>(new[]
        {
            "(", "tan", ")", "atan", "cot", "sin", "cos", "abs", "sqrt", "floor", "ceiling",
            "round", "pow","cube","ln","log",
            "~", "!", "*", "/", "%", "+", "-", "<", ">", "=", "&", "|", ",", "@"
        });

        private static bool IsMatching(string exp)
        {
            string opt = "";  

            for (int i = 0; i < exp.Length; i++)
            {
                string chr = exp.Substring(i, 1); 
                if ("\"'#".Contains(chr))
                {
                    if (opt.Contains(chr))  
                    {
                        opt = opt.Remove(opt.IndexOf(chr), 1);  
                    }
                    else
                    {
                        opt += chr;
                    }
                }
                else if ("()".Contains(chr)) 
                {
                    if (chr == "(")
                    {
                        opt += chr;
                    }
                    else if (chr == ")")
                    {
                        if (opt.Contains("("))
                        {
                            opt = opt.Remove(opt.IndexOf("("), 1);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return (opt == "");
        }

        private int FindOperator(string exp, string findOpt)
        {
            var opt = "";
            for (var i = 0; i < exp.Length; i++)
            {
                var chr = exp.Substring(i, 1);
                if ("\"'#".Contains(chr))
                {
                    if (opt.Contains(chr))
                    {
                        opt = opt.Remove(opt.IndexOf(chr), 1);
                    }
                    else
                    {
                        opt += chr;
                    }
                }

                if (opt != "") 
                    continue;

                if (findOpt != "")
                {
                    if (findOpt == chr)
                    {
                        return i;
                    }
                }
                else
                {
                    if (m_Operators.Exists(x => x.IndexOf(chr, StringComparison.CurrentCultureIgnoreCase) > -1))
                        return i;
                }
            }
            return -1;
        }

        public bool Parse(string exp)
        {
            Tokens.Clear();
            if (exp.Trim() == "")
                return false;
            else if (!IsMatching(exp))
                return false;

            exp = exp.Replace("（", "(");
            exp = exp.Replace("）", ")");
            exp = exp.Replace("！", "!");
            exp = exp.Replace("\\", "/");

            exp = exp.Replace("PI", $"({Math.PI})");

            if (exp.StartsWith("-") || exp.StartsWith("+"))
                exp = "0" + exp;

            exp = ReplaceNegative(exp);

            #region 解析

            var nums = new Stack<object>();
            var operators = new Stack<Operator>();
            var optType = OptType.ERR;             
            var curNum = "";                         
            var curOpt = "";
            var curPos = 0;
            
            curPos = FindOperator(exp, "");

            exp += "@"; 
            while (true)
            {
                curPos = FindOperator(exp, "");

                curNum = exp.Substring(0, curPos).Trim();
                curOpt = exp.Substring(curPos, 1);

                if (curNum != "")
                    nums.Push(new Operand(curNum, curNum));

                if (curOpt == "@")
                    break;

                if (curOpt == "(")
                {
                    operators.Push(new Operator(OptType.LB, "("));
                    exp = exp.Substring(curPos + 1).Trim();
                    continue;
                }

                if (curOpt == ")")
                {
                    while (operators.Count > 0)
                    {
                        if (operators.Peek().Type != OptType.LB)
                        {
                            nums.Push(operators.Pop());
                        }
                        else
                        {
                            operators.Pop();
                            break;
                        }
                    }
                    exp = exp.Substring(curPos + 1).Trim();
                    continue;
                }

                curOpt = Operator.AdjustOperator(curOpt, exp, ref curPos);
                optType = Operator.ConvertOperator(curOpt);

                if (operators.Count == 0 || operators.Peek().Type == OptType.LB)
                {
                    operators.Push(new Operator(optType, curOpt));
                    exp = exp.Substring(curPos + 1).Trim();
                    continue;
                }

                if (Operator.ComparePriority(optType, operators.Peek().Type) > 0)
                {
                    operators.Push(new Operator(optType, curOpt));
                }
                else
                {
                    while (operators.Count > 0)
                    {
                        if (Operator.ComparePriority(optType, operators.Peek().Type) <= 0 && 
                            operators.Peek().Type != OptType.LB)
                        {
                            nums.Push(operators.Pop());

                            if (operators.Count != 0) 
                                continue;

                            operators.Push(new Operator(optType, curOpt));
                            break;
                        }
                        else
                        {
                            operators.Push(new Operator(optType, curOpt));
                            break;
                        }
                    }

                }
                exp = exp.Substring(curPos + 1).Trim();
            }
            while (operators.Count > 0)
            {
                nums.Push(operators.Pop());
            }
            while (nums.Count > 0)
            {
                Tokens.Push(nums.Pop());
            }

            return true;

            #endregion
        }

        /// Replace negative and postive(-+) symbol to # @
        public string ReplaceNegative(string exp)
        {
            var len = exp.Length;
            var expT = exp.ToArray();
            var symbols = new[] {'+', '-', '*', '/', '^', '(', ',', '<', '>', '=' };

            for (var i = len - 1; i > -1; i--)
            {
                if (expT[i] == '-' && i > 1)
                    if (symbols.Contains(exp[i - 1]))
                        expT[i] = '~';

                if (expT[i] == '-' && i == 1) 
                    expT[i] = '~';

                if (expT[i] == '+' && i > 1)
                    if (symbols.Contains(exp[i - 1]))
                        expT[i] = '#';

                if (expT[i] == '+' && i == 1)
                    expT[i] = '#';
            }

            exp = string.Join("", expT).Replace("#", "");

            return exp;
        }

        public object Evaluate()
        {
            try
            {
                if (Tokens.Count == 0) return null;

                object value = null;
                var opds = new Stack<Operand>();
                Operand opdA;
                Operand opdB;
                foreach (var item in Tokens)
                {
                    if (item is Operand operand)
                    {
                        opds.Push(operand);
                    }
                    else
                    {
                        switch (((Operator)item).Type)
                        {
                            case OptType.LB:
                                continue;
                            case OptType.RB:
                                continue;
                            case OptType.PS:
                                continue;

                            case OptType.NS:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        -1.0 * double.Parse(opdA.Value.ToString())));
                                break;

                            case OptType.MUL:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER, double.Parse(opdB.Value.ToString()) * double.Parse(opdA.Value.ToString())));
                                break;

                            case OptType.DIV:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER, double.Parse(opdB.Value.ToString()) / double.Parse(opdA.Value.ToString())));
                                break;

                            case OptType.MOD:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER, double.Parse(opdB.Value.ToString()) % double.Parse(opdA.Value.ToString())));
                                break;

                            case OptType.ADD:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER, double.Parse(opdB.Value.ToString()) + double.Parse(opdA.Value.ToString())));
                                break;

                            case OptType.SUB:
                                opdA = opds.Pop();
                                opdB = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                            double.Parse(opdB.Value.ToString()) - double.Parse(opdA.Value.ToString())));
                                break;

                            case OptType.NOT: 
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,double.Parse($"{opdA.Value}") > 0 ? -1 : 1));
                                break;

                            case OptType.LT:
                                break;
                            case OptType.LE:
                                break;
                            case OptType.GT:
                                break;
                            case OptType.GE:
                                break;
                            case OptType.ET:
                                break;
                            case OptType.UT:
                                break;
                            case OptType.AND:
                                break;
                            case OptType.OR:
                                break;
                            case OptType.CA:
                                break;

                            case OptType.TAN:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER, 
                                        Math.Tan(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                                break;

                            case OptType.COT:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        1.0 / Math.Tan(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                                break;

                            case OptType.ATAN:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER, Math.Atan(double.Parse(opdA.Value.ToString()))));
                                break;
                            
                            case OptType.SIN:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Sin(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                                break;

                            case OptType.COS:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Cos(double.Parse(opdA.Value.ToString()) * Math.PI / 180)));
                                break;

                            case OptType.Abs:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Abs(double.Parse(opdA.Value.ToString()))));
                                }
                                break;

                            case OptType.Floor:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Floor(double.Parse(opdA.Value.ToString()))));
                                break;

                            case OptType.Ceiling:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Ceiling(double.Parse(opdA.Value.ToString()))));
                                break;

                            case OptType.Sqrt:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Sqrt(double.Parse(opdA.Value.ToString()))));
                                break;

                            case OptType.Round:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Round(double.Parse(opdA.Value.ToString()), 2)));
                                break;

                            case OptType.Pow:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Pow(double.Parse(opdA.Value.ToString()), 2)));
                                break;

                            case OptType.Cube:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Pow(double.Parse(opdA.Value.ToString()), 3)));
                                break;

                            case OptType.Ln:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Log(double.Parse(opdA.Value.ToString()))));
                                break;

                            case OptType.Log:
                                opdA = opds.Pop();
                                opds.Push(new Operand(Type.NUMBER,
                                        Math.Log10(double.Parse(opdA.Value.ToString()))));
                                break;

                            case OptType.END:
                                break;
                            case OptType.ERR:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (opds.Count == 1)
                    value = opds.Pop().Value;

                return value;
            }
            catch
            {
                return null;
            }
            
        }
    }