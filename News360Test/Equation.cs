using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace News360Test
{
    public class Equation
    {
        public Equation(string aEquationText)
        {
            string _decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            this.FData = aEquationText.Replace(" ", "").Replace("-", "+(-1)").Replace(".", _decimalSeparator);
            string[] _equationParts = this.FData.Split('=');
            this.FLeftPart = _equationParts[0];
            this.FRigthPart = _equationParts[1];
        }

        string FData;
        string FLeftPart;
        string FRigthPart;

        List<Variable> GetVariable(string aInput, string aPattern)
        {
            List<Variable> _vars = new List<Variable>();
            Regex _regex = new Regex(aPattern);

            MatchCollection _m = _regex.Matches(aInput);

            for (int i = 0; i < _m.Count; i++)
            {
                Match _match = _m[i];
                GroupCollection _groups = _match.Groups;
                string _varName = _groups["VarName"] != null ? _groups["VarName"].Value : String.Empty;
                string _varPow = _groups["VarPow"].Value != String.Empty ? _groups["VarPow"].Value : "1";

                int _pow = Int32.Parse(_varPow);

                Variable _var = new Variable(_varName[0], _pow);
                _vars.Add(_var);
            }

            return _vars;
        }

        List<Expression> GetSummandExpressionsAll(string aExpression)
        {
            string _pattern = @"(?<VarName>[a-z])(\^(?<VarPow>\d+))?";
            string _patternVar = String.Format(@"(?<Coeff>([-]?\d+(,\d+)?))?(?<Var>({0})*)", _pattern);
            Regex _regexVar = new Regex(_patternVar);

            MatchCollection _mcVar = _regexVar.Matches(aExpression);

            List<Expression> _res = new List<Expression>();

            for (int i = 0; i < _mcVar.Count; i++)
            {
                Match _m = _mcVar[i];
                string _summ = _m.Value;
                if (_summ != String.Empty)
                {
                    GroupCollection _groups = _mcVar[i].Groups;
                    string _coeff = _groups["Coeff"].Value != String.Empty ? _groups["Coeff"].Value : "1";

                    Summand _summand = new Summand(Double.Parse(_coeff.Replace(" ", "")));

                    string _varsCapture = _groups["Var"].Value;

                    List<Variable> _vars = GetVariable(_varsCapture, _pattern);
                    _summand.AddVariableRange(_vars);
                    _summand.ToCanonical();

                    SummandExpression _exp = new SummandExpression(_m.Index, _m.Index + _m.Length - 1, _summand);
                    _res.Add(_exp);
                }
            }

            return _res;
        }

        Expression ParseExpression(string aExpression)
        {
            Stack<Expression> _exps = new Stack<Expression>();
            List<Expression> _summExps = GetSummandExpressionsAll(aExpression);

            Expression _rootExp = new Expression(-1);
            _exps.Push(_rootExp);

            for (int i = 0; i < aExpression.Length; i++)
            {
                List<Expression> _curSumm = _summExps.Where(x => x.Start == i).ToList();
                if (_curSumm.Count > 0)
                {
                    Expression _top = _exps.Peek();
                    _top.AddSubExpression(_curSumm[0]);
                }

                char _currChar = aExpression[i];
                if (_currChar == '(')
                {
                    Expression _exp = new Expression(i);
                    Expression _top = _exps.Peek();
                    _top.AddSubExpression(_exp);
                    _exps.Push(_exp);
                }

                if (_currChar == ')')
                {
                    Expression _top = _exps.Pop();
                    _top.CloseExpression(i);
                }
            }

            _rootExp.CloseExpression(aExpression.Length);
            return _rootExp;
        }

        public string ToCanonical()
        {
            Expression _leftExp = ParseExpression(this.FLeftPart);
            _leftExp.ToCanonical();
            Expression _rightExp = ParseExpression(this.FRigthPart);
            _rightExp.ToCanonical();

            Sum _sum = _leftExp.ExpressionSum + _rightExp.ExpressionSum.Inverse;

            return String.Format("{0}=0", _sum);
        }
    }
}
