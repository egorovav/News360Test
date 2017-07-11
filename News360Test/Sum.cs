using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News360Test
{
    public class Sum
    {
        List<Summand> FSummands = new List<Summand>();
        public List<Summand> Summands
        {
            get { return this.FSummands; }
        }

        public Sum Inverse
        {
            get
            {
                Sum _inverseSum = new Sum();
                foreach (Summand _summ in this.FSummands)
                {
                    Summand _inverseSumm = new Summand(-(_summ.Coeff), _summ.Variables);
                    _inverseSum.FSummands.Add(_inverseSumm);
                }

                return _inverseSum;
            }
        }

        public void AddSummand(Summand aSummand)
        {
            this.FSummands.Add(aSummand);
        }

        public void AddSummandRange(IEnumerable<Summand> aSummands)
        {
            this.FSummands.AddRange(aSummands);
        }

        public static Sum operator *(Sum a, Sum b)
        {
            Sum _res = new Sum();

            foreach (Summand _summA in a.FSummands)
                foreach (Summand _summB in b.FSummands)
                    _res.AddSummand(_summA * _summB);

            _res.ToCanonical();

            return _res;
        }

        public static Sum operator +(Sum a, Sum b)
        {
            Sum _res = new Sum();

            _res.AddSummandRange(a.FSummands);
            _res.AddSummandRange(b.FSummands);

            _res.ToCanonical();
            return _res;
        }

        public void ToCanonical()
        {
            IEnumerable<Summand> _groupingSums =
                this.FSummands.GroupBy(x => x, (s, ss) => new Summand(ss.Sum(x => x.Coeff), ss.First().Variables), new SummandSimilarity());

            this.FSummands = _groupingSums.ToList();
        }

        public override string ToString()
        {
            StringBuilder _sb = new StringBuilder();
            List<Summand> _summands = this.FSummands.OrderBy(x => x.Variables.Count).ToList();
            foreach (Summand _summ in _summands)
                _sb.Append(_summ.ToString());

            string _decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            return _sb.ToString().TrimStart('+').Replace(_decimalSeparator, ".");
        }
    }
}
