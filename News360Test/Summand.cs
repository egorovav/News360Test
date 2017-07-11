using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News360Test
{
    public class Summand
    {
        public Summand()
        {
        }

        public Summand(double aCoeff)
        {
            this.FCoeff = aCoeff;
        }

        public Summand(double aCoeff, List<Variable> aVars) :
            this(aCoeff)
        {
            this.FVariables = aVars;
        }

        double FCoeff = 1;
        public double Coeff
        {
            get { return this.FCoeff; }
        }

        List<Variable> FVariables = new List<Variable>();
        public List<Variable> Variables
        {
            get { return this.FVariables; }
        }

        public void AddVariable(Variable aVar)
        {
            this.FVariables.Add(aVar);
        }

        public void AddVariableRange(IEnumerable<Variable> aVars)
        {
            this.FVariables.AddRange(aVars);
        }

        public void ToCanonical()
        {
            List<Variable> _vars = new List<Variable>(this.FVariables);

            IEnumerable<Variable> _groupingVar = _vars.
                GroupBy(x => x.VariableName, (name, vars) => new Variable(name, vars.Sum(x => x.Pow))).
                OrderBy(x => x.VariableName);

            this.FVariables = _groupingVar.ToList();          
        }

        public static Summand operator * (Summand a, Summand b)
        {
            Summand _res = new Summand(a.FCoeff * b.FCoeff);

            List<Variable> _variablesMult = new List<Variable>(a.FVariables);
            _variablesMult.AddRange(b.FVariables);
            _res.FVariables = _variablesMult;
            _res.ToCanonical();

            return _res;
        }

        public bool IsSimilarTo(Summand aVar)
        {
            this.ToCanonical();
            aVar.ToCanonical();

            if (this.FVariables.Count != aVar.FVariables.Count)
                return false;

            for (int i = 0; i < this.FVariables.Count; i++)
                if (this.FVariables[i] != aVar.FVariables[i])
                    return false;

            return true;
        }

        public override string ToString()
        {
            if (this.FCoeff == 0)
                return String.Empty;

            string _coeffString = 
                Math.Abs(this.FCoeff) != 1 || this.FVariables.Count == 0 ? Math.Abs(this.FCoeff).ToString() : String.Empty;

            string _sign = this.FCoeff < 0 ? "-" : "+";
            StringBuilder _sb = new StringBuilder(_sign);
            _sb.Append(_coeffString);
            foreach (Variable _var in this.FVariables)
                _sb.Append(_var.ToString());

            return _sb.ToString();
        }
    }

    public class SummandSimilarity : IEqualityComparer<Summand>
    {

        public bool Equals(Summand x, Summand y)
        {
            return x.IsSimilarTo(y);
        }

        public int GetHashCode(Summand obj)
        {
            return 0;
        }
    }
}
