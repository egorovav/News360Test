using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News360Test
{
    public class Expression
    {
        public Expression(int aStart)
        {
            this.FStart = aStart;
        }

        List<Expression> FSubExpressions = new List<Expression>();

        int FStart;
        public int Start
        {
            get { return this.FStart; }
        }

        protected int FEnd;
        public int End
        {
            get { return this.FEnd; }
        }

        int FLevel;
        public int Level
        {
            get { return this.FLevel; }
        }

        public bool IsClosed
        {
            get { return this.FEnd != 0; }
        }

        public virtual Sum ExpressionSum
        {
            get
            {
                Sum _sum = new Sum();
                foreach(Expression _exp in this.FSubExpressions)
                {
                    SummandExpression _summExp = (_exp as SummandExpression);
                    if (_summExp != null)
                        _sum.AddSummand(_summExp.Summand);
                }
                _sum.ToCanonical();
                return _sum;
            }
        }

        public void AddSubExpression(Expression aExp)
        {
            aExp.FLevel = this.FLevel + 1;
            this.FSubExpressions.Add(aExp);
        }

        public void CloseExpression(int aEnd)
        {
            this.FEnd = aEnd;
        }

        void MultSubExpressions(Expression a, Expression b)
        {
            Sum _summs = a.ExpressionSum * b.ExpressionSum;
            Expression _res = new Expression(Math.Min(a.Start, b.Start));
            foreach (Summand _summ in _summs.Summands)
                _res.AddSubExpression(new SummandExpression(0, 0, _summ));
            _res.CloseExpression(Math.Max(a.End, b.End));

            this.FSubExpressions.Insert(this.FSubExpressions.IndexOf(a), _res);
            this.FSubExpressions.Remove(a);
            this.FSubExpressions.Remove(b);
        }

        public bool ToCanonical()
        {
            bool _collectionChanged = false;

            foreach (Expression _expLeft in this.FSubExpressions)
            {
                foreach (Expression _expRigth in this.FSubExpressions)
                {
                    if (_expLeft.End + 1 == _expRigth.Start)
                    {
                        if (_expLeft.IsSimple && _expRigth.IsSimple)
                        {
                            this.MultSubExpressions(_expLeft, _expRigth);
                            _collectionChanged = true;
                            break;
                        }

                        if (!_expLeft.IsSimple)
                        {
                            _collectionChanged = _expLeft.ToCanonical();
                        }

                        if (!_expRigth.IsSimple)
                        {
                            _collectionChanged = _expRigth.ToCanonical();
                        }
                    }
                }
                if (_collectionChanged)
                    break;
            }

            if (_collectionChanged)
                    this.ToCanonical();
            else
            {
                List<Expression> _subExpression = new List<Expression>();
                foreach (Expression _subExp in this.FSubExpressions)
                {
                    if (_subExp.FSubExpressions.Count > 0)
                        _subExpression.AddRange(_subExp.FSubExpressions);
                    else
                        _subExpression.Add(_subExp);
                }

                this.FSubExpressions = _subExpression;
            }

            return _collectionChanged;
        }

        public bool IsSimple
        {
            get
            {
                if (this is SummandExpression)
                    return true;

                foreach (Expression _exp in this.FSubExpressions)
                    if (!(_exp is SummandExpression))
                        return false;

                return true;
            }
        }
    }

    public class SummandExpression : Expression
    {
        public SummandExpression(int aStart, int aEnd, Summand aSummand) :
            base(aStart)
        {
            this.FEnd = aEnd;
            this.FSummand = aSummand;
        }

        Summand FSummand;
        public Summand Summand
        {
            get { return this.FSummand; }
        }

        public override Sum ExpressionSum
        {
            get
            {
                Sum _sum = new Sum();
                _sum.AddSummand(this.FSummand);
                return _sum;
            }
        }

    }
}
