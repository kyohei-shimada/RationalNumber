using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new Math.RationalNumber(BigInteger.Pow(2, 102400), BigInteger.Pow(2, 102399), true);
            var c = new Math.RationalNumber(-3456, 4567, false);
            var d = new Math.RationalNumber(0, 1, false);
            /*System.Console.WriteLine(Math.RationalNumber.Add(b, c).ToString());
            System.Console.WriteLine(Math.RationalNumber.Subtract(b, c).ToString());
            System.Console.WriteLine((b + c).ToString());
            System.Console.WriteLine((b - c).ToString());
            Console.WriteLine(Math.RationalNumber.Abs(b).ToString());
            Console.WriteLine(Math.RationalNumber.Abs(c).ToString());
            //c._auto_reduction

            var n = (Math.RationalNumber)120;
             * */
            System.Console.WriteLine(b.ToString());
        }
    }
}
