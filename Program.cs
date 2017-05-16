using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO
{
    class Program
    {
        static void Main(string[] args)
        {
            //local minimum at x = 0.7071707
            //local maximum at x = -0.7071707
            Console.WriteLine("Enter a, b, eps:");
            double a = double.Parse(Console.ReadLine());
            double b = double.Parse(Console.ReadLine());
            double eps = double.Parse(Console.ReadLine());

            Func<double, double> f = x => 1 * Math.Pow(x, 5) + 0 * Math.Pow(x, 4) + 0.5 * Math.Pow(x, 3) + 0 * Math.Pow(x, 2) + -2 * x + 2;
            //Func<double, double> f = x => Math.Pow(x - 1, 2);
            Func<double, double> df = x => 5 * Math.Pow(x, 4) + 1.5 * Math.Pow(x, 2) - 2;
            //Func<double, double> df = x => 2 * (x - 1);
            Func<double, double> d2f = x => 20 * Math.Pow(x, 3) + 3 * x;
            //Func<double, double> d2f = x => 2;
            try
            {
                Console.WriteLine("Bisection: " + OptimizationMethods.Bisection(f, df, d2f, a, b, eps));
                Console.WriteLine("Golden ratio: " + OptimizationMethods.GoldenRatio(f, df, d2f, a, b, eps));
                Console.WriteLine("Parabola : " + OptimizationMethods.Parabola(f, df, d2f, a, b, eps));
                Console.WriteLine("Newton: " + OptimizationMethods.Newton(f, df, d2f, a, b, eps));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }
    }
}
