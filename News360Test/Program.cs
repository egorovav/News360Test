using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace News360Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //EquationTest();
            //return;

            using (FileStream _input_fs = new FileStream("input.txt", FileMode.Open))
            using (StreamReader _sr = new StreamReader(_input_fs))
            using (FileStream _output_fs = new FileStream("output.txt", FileMode.Create))
            using (StreamWriter _sw = new StreamWriter(_output_fs))
            {
                while (!_sr.EndOfStream)
                {
                    string _input = _sr.ReadLine();
                    Equation _equation = new Equation(_input);

                    string _output = _equation.ToCanonical();
                    _sw.WriteLine("{0}, {1}", _input, _output);
                }
            }

        }

        private static void EquationTest()
        {
            //string _input = "a(b+c)-((d+(e+j)(g-d))k+f)g+h-ab=a(b+c)-((d+(e+j)(g-d))k+f)g+h";//-ab";
            string _input = "x^2 + 3.5xy + y = y^2 - xy + y";

            Equation _equation = new Equation(_input);
            string _res = _equation.ToCanonical();
            Console.WriteLine(_res);
            Console.ReadLine();
        }
    }
}
