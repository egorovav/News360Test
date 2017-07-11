using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News360Test
{
    public class Variable
    {
        public Variable(char aVariableName)
        {
            this.FVariableName = aVariableName;
        }

        public Variable(char aVariableName, int aPow)
        {
            this.FVariableName = aVariableName;
            this.FPow = aPow;
        }

        char FVariableName;
        public char VariableName
        {
            get { return this.FVariableName; }
        }

        int FPow = 1;
        public int Pow
        {
            get { return this.FPow; }
        }

        public static Variable operator * (Variable a, Variable b)
        {
            if (a.FVariableName == b.FVariableName)
            {
                Variable _res = new Variable(a.FVariableName, a.FPow + b.FPow);
                return _res;
            }
            else
                return null;
        }


        public override bool Equals(object obj)
        {
            if ((obj as Variable) == null)
                return false;
            else
            {
                Variable _var = obj as Variable;
                return _var.FPow == this.FPow && _var.FVariableName == this.FVariableName;
            }
        }

        public static bool operator ==(Variable lhs, Variable rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Variable lhs, Variable rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return this.FPow;
        }

        public override string ToString()
        {
            string _powString = this.FPow != 1 ? String.Format("^{0}", this.FPow) : String.Empty;
            return String.Format("{0}{1}", this.FVariableName, _powString);
        }
    }
}
