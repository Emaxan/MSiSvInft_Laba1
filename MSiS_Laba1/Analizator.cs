using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSiS_Laba1//TODO проверить пустые ифы
{
    public class Analizator
    {
        private readonly Regex _allInOneReg = new Regex(@"((\bcase\s*([^;|:])+\s*of\b)|(\bif(\s|\()+[^;\n]+(\s|\))+then\b)|(\b(for|while|repeat)\b)|(\bend(\.|;))|(\belse\b))");
        private int _stackSize = 1, _stackTop = -1, _deletedCount = 0, _cycleCount = 2, _caseCount = 2;
        private bool _oneOperation = false;
        private States _currentState = States.Null;
        private States[] _statesStack = new States[1];
        public string Text;
        private const int THEN = 0, ELSE = 1, EXIT = 0;
         
        public Analizator(string text)
        {
            Text = text.ToLower();
            DeleteCommentsAndLiterals();
            _statesStack[0] = States.Cycle;
        }

        public int DeleteCommentsAndLiterals()
        {
            var finder = new Regex(@"((//.*|{([^}])+})|(\'.*\'))");
            var matches = finder.Match(Text);
            while (matches.Success)
            {
                Text = Text.Remove(matches.Index, matches.Length);
                matches = finder.Match(Text);
            }
            return new Regex(@"[{\']").Match(Text).Success ? 1 : 0;
        }

        private void SetState(States state)
        {
            PUSH(_currentState);
            _currentState = state;
        }

        public int Work(out Point point)  
        {
            switch (_currentState)
            {
                case States.Null:
                    if (NullProcess(out point) != 0) return 1;
                    break;
                case States.Code:
                    if (CodeProcess(out point) != 0) return 1;
                    break;
                case States.If:
                    if (IfProcess(out point) != 0) return 1;
                    break;
                case States.Case:
                    if (CaseProcess(out point) != 0) return 1;
                    break;
                default://case States.Cycle:
                    if (CycleProcess(out point) != 0) return 1;
                    break;
            } 
            return 0;
        }

        private int IfProcess(out Point ifPoint)
        {
            ifPoint = new Point(PointClass.IfPoint, 2) {OutPoint = new Point[2]};
            var thenResult = new Regex(@"\bthen\b").Match(Text);
            if (!thenResult.Success) return 1; 
            _deletedCount += thenResult.Index;
            Text = Text.Remove(0, thenResult.Index);

            if (BranchProcess(ref ifPoint, THEN) != 0) return 1;

            var allRes = _allInOneReg.Match(Text);
            var elseRes = new Regex(@"\belse\b").Match(Text);

            if (elseRes.Success && elseRes.Index <= allRes.Index)
            {
                _deletedCount += elseRes.Index;
                Text = Text.Remove(0, elseRes.Index + elseRes.Value.Length);
                if (Text.Substring(0, "\r\n".Length) == "\r\n")
                {
                    _deletedCount += "\r\n".Length;
                    Text = Text.Remove(0, "\r\n".Length);
                }

                if (BranchProcess(ref ifPoint, ELSE) != 0) return 1;
            }
            else
                ifPoint.OutPoint[ELSE] = new Point(PointClass.CodePoint, 1){OutPoint = new Point[1]};
            var exitPoint = new Point(PointClass.Exit, 1);
            exitPoint.OutPoint = new Point[exitPoint.OutCount];
            ifPoint.OutPoint[THEN].OutPoint[EXIT] = exitPoint;
            ifPoint.OutPoint[ELSE].OutPoint[EXIT] = exitPoint;
            _currentState = POP();
            if (Text.Substring(0, "end".Length) == "end")
            {
                _currentState = POP();
                if (!_oneOperation)
                {
                    _deletedCount += "end;".Length;
                    Text = Text.Remove(0, "end;".Length);
                }
                _oneOperation = false;
            }
            return Work(out exitPoint.OutPoint[EXIT]);
        }

        private int BranchProcess(ref Point ifPoint, int branch)
        {
            var allRes = _allInOneReg.Match(Text);
            var beginRes = new Regex(@"\bbegin\b").Match(Text);
            if (beginRes.Success && allRes.Index > beginRes.Index)
            {
                SetState(States.Code);
                while (Text.Substring(0, "end;".Length) != "end;")
                    if (Work(out ifPoint.OutPoint[branch]) != 0) return 1;
            }
            else
            {
                allRes = _allInOneReg.Match(Text);
                var endRes = new Regex("\n").Match(Text);
                if(allRes.Index < endRes.Index)
                    SetState(States.Code);
                _oneOperation = true;
                while (allRes.Index < endRes.Index)
                {
                    if (Work(out ifPoint.OutPoint[branch]) != 0) return 1;
                    allRes = _allInOneReg.Match(Text);
                    endRes = new Regex("\n").Match(Text);
                }
                //_deletedCount += endRes.Index + 1;
                //Text = Text.Remove(0, endRes.Index + 1);
            }
            // ReSharper disable once InvertIf
            if (ifPoint.OutPoint[branch] == null)
            {
                ifPoint.OutPoint[branch] = new Point(PointClass.CodePoint, 1);
                ifPoint.OutPoint[branch].OutPoint = new Point[ifPoint.OutPoint[branch].OutCount];
            }
            return 0;
        }

        private int CaseProcess(out Point point)
        {
            point = new Point(PointClass.CasePoint, _caseCount);
            return 0;
        }

        private int CycleProcess(out Point point)
        {
            point = new Point(PointClass.CasePoint, _cycleCount);
            return 0;
        }

        private int CodeProcess(out Point point)
        {
            var allInOneResult = _allInOneReg.Match(Text);
            if (!allInOneResult.Success){ point = null; return 1; }
            var length = allInOneResult.Index + (allInOneResult.Length < 3 ? allInOneResult.Length : 0);
            _deletedCount += length;
            Text = Text.Remove(0, length);
            if (allInOneResult.Value.Substring(0, "if".Length) == "if")
                SetState(States.If);
            else if (allInOneResult.Value.Substring(0, "case".Length) == "case")
                SetState(States.Case);
            else if (allInOneResult.Value.Substring(0, "end;".Length) == "end;" || allInOneResult.Value.Substring(0, "end.".Length) == "end.")
            { point = null; return 0; }
            else if (allInOneResult.Value.Substring(0, "for".Length) == "for" || allInOneResult.Value.Substring(0, "while".Length) == "while" ||
                     allInOneResult.Value.Substring(0, "repeat".Length) == "repeat")
                SetState(States.Cycle);
            return Work(out point);
        }

        private int NullProcess(out Point point)
        {
            if (DeleteCommentsAndLiterals() != 0) { point = null; return 1; }
            var functions = FindCode();
            SetState(States.Code);
            for (var i = 0; i < (functions == null ? -1 : functions.Length); i++)
            {
                // ReSharper disable once PossibleNullReferenceException
                Text = Text.Remove(0, functions[i].Begin - _deletedCount);
                _deletedCount += (functions[i].Begin - _deletedCount);
                // ReSharper disable once InvertIf
                if (Work(out functions[i].Graf) != 0) { point = null; return 1; }
            }
            return Work(out point);
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private Function[] FindCode()
        {
            var forward = new Regex(@"(\bprocedure\b|\bfunction\b)\s*[\w][\w\d]*\s*(\(.*\)\s*|):?\s*([\w][\w\d]*|)\s*;\s*forward\s*;").Matches(Text);
            if (forward.Count>0)
                foreach (Match match in forward)
                    Text = Text.Remove(match.Index, match.Length);       

            var matches = new Regex(@"(\bprocedure\b|\bfunction\b)").Matches(Text);
            if (matches.Count <= 0) return null;
            var functions = new Function[matches.Count];
            for (var i = 0; i < matches.Count; i++)
            {
                // ReSharper disable once SimplifyConditionalTernaryExpression
                functions[i] = FunctionFinder(matches[i]);
            }
            return functions;
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private Function FunctionFinder(Match function)
        {
            int beginsCount = 0, index = 0;
            var matches = new Regex(@"(\bbegin\b|\bend\b)").Matches(Text,function.Index);
            var matchName = new Regex(@"\s.+[\(;]").Match(Text,function.Index);
            var nameEnd = 0;
            while (matchName.Value[nameEnd] != ';' && matchName.Value[nameEnd] != '(')  nameEnd++; 
            while (index < matches.Count)
            {
                if (matches[index++].Value == "begin")
                    beginsCount++;
                else
                    beginsCount--;
                if (beginsCount == 0) break;
            }
            return new Function
            {
                Begin = function.Index,
                End = matches[index - 1].Index+matches[index - 1].Length,
                Name = matchName.Value.Substring(1, nameEnd - 1).Trim()
            };
        }

        // ReSharper disable once InconsistentNaming
        private States POP()
        {
            var state = _statesStack[_stackTop--];
            if (_stackTop < _stackSize/2)
                Array.Resize(ref _statesStack, _stackSize /= 2);
            return state;
        }

        // ReSharper disable once InconsistentNaming
        private void PUSH(States state)
        {
            if (_stackTop == _stackSize - 1)
                Array.Resize(ref _statesStack, _stackSize *= 2);
            _statesStack[++_stackTop] = state;
        }

        public int MakkeybNumber()
        {
            var func = FindCode();
            string reg;

            if (func != null)
            {
                var count = func.Length;
                reg = func.Aggregate( @"((\bcase\s*([^;|:])+\s*of\b)|(\bif(\s|\()+[^;]+(\s|\))+then\b)|(\b(for|while|repeat)\b)", (current, f) => current + (@"|(\b" + f.Name + @"\b)")) + @")";
                while (count != 0)
                    for (var i = 0; i < func.Length; i++)
                    {
                        if(func[i].Makkeyb!=0)continue;
                        var startMeaning = func[i].Makkeyb;
                        func[i].Makkeyb = FunctionCount(Text.Substring(func[i].Begin, func[i].End - func[i].Begin), reg, func, i);
                        if (func[i].Makkeyb != startMeaning) count--;
                    }
            }
            else
                reg = @"((\bcase\s*([^;|:])+\s*of\b)|(\bif(\s|\()+[^;]+(\s|\))+then\b)|(\b(for|while|repeat)\b))";
            var number = FunctionCount(func == null ? Text : Text.Substring(func[func.Length - 1].End + 4), reg, func, -1);
            return number + 1;
        }

        private int FunctionCount(string text, string reg, Function[] func, int numb)
        {
            var number = 0;
            var matches = new Regex(reg).Matches(text, text.IndexOf('\n'));
            foreach (Match match in matches)
            {
                var find = false;
                if (func != null)
                    for (var i = 0; i < func.Length; i++)
                        if (match.Value == func[i].Name)
                        {
                            if (i != numb)
                                if (func[i].Makkeyb == 0) return 0;
                            number += func[i].Makkeyb;
                            find = true;
                            break;
                        }
                if (find) { continue; }
                else if (match.Value.Length<4||match.Value.Substring(0, 4) != "case")
                    number++;
                else
                    number += CaseCount(match.Index, text);

            }
            return number;
        }

        private int CaseCount(int match, string text) 
        {
            var number = 0;
            var beginsCount = 0;
            while (beginsCount != -1 && text.Substring(match, "end;".Length) != "end;")
            {
                if ((text[match] == ':' && text[match + 1] != '=') || text.Substring(match, "else".Length) == "else") number++;
                if (text.Substring(match, "begin".Length) == "begin")
                    beginsCount++;
                if (text.Substring(match, "end".Length) == "end")
                    beginsCount--;
                match++;
            }
            return number - 1;
        }

        public string AnalizeResult(Point point)
        {
            var p = 1;
            var vertexCount = 2;
            var edgeCount = 1;
            var result = "";
            if (Check(point, ref vertexCount, ref edgeCount) != 0) return "Error!";
                result += "Number of vertex: " + vertexCount + "\nNumber of edges: " + edgeCount + "\nMakkeyb Number: " + (edgeCount - vertexCount + 2*p);
            return result;
        }

        private int Check(Point point, ref int vertexCount, ref int edgeCount)
        {
            if (point == null||point.Checked) return 0;
            point.Checked = true;
            vertexCount++;
            edgeCount += point.OutCount;
            foreach (var poi in point.OutPoint)
                Check(poi, ref vertexCount, ref edgeCount);
            return 0;
        }
    }
    public enum States
        {
            Code,
            If,
            Case,
            Cycle,
            Null
        }

    struct Function
    {
        public int Begin, End, Makkeyb;
        public string Name;
        public Point Graf;
    }

    public enum PointClass
    {
        CodePoint,
        CasePoint,
        IfPoint, 
        CyclePoint,
        Exit
    }

    public class Point
    {
        public PointClass PointClass;
        public int OutCount;
        public Point[] OutPoint;
        public bool Checked;

        public Point(PointClass pointClass,int outCount)
        {
            Checked = false;
            PointClass = pointClass;
            OutCount = outCount;
        }
    }
}