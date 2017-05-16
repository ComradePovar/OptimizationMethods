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
            //Func<double[], double> function = u => Math.Pow(u[0] - u[1], 2) + u[0] - 5 * u[1];
            //Func<double[], double> dfdu1 = u => 2 * u[0] - 2 * u[1] + 1;
            //Func<double[], double> dfdu2 = u => -2 * u[0] + 2 * u[1] - 5;
            //double dfu1u1 = 2;
            //double dfu1u2 = -2;
            //double dfu2u1 = -2;
            //double dfu2u2 = 2;
            Func<double[], double> function = u => 3 * Math.Pow(u[0], 2) + 4 * u[0] * u[1] + 3 * Math.Pow(u[1], 2);
            Func<double[], double> dfdu1 = u => 6 * u[0] + 4 * u[1];
            Func<double[], double> dfdu2 = u => 6 * u[1] + 4 * u[0];
            double dfu1u1 = 6;
            double dfu1u2 = 4;
            double dfu2u1 = 4;
            double dfu2u2 = 6;
            OptimizationMethods opt = new OptimizationMethods(function, dfdu1, dfdu2, dfu1u1, dfu1u2, dfu2u1, dfu2u2);

            Console.WriteLine("Enter u1, u2, eps:");
            double u1 = double.Parse(Console.ReadLine());
            double u2 = double.Parse(Console.ReadLine());
            double eps = double.Parse(Console.ReadLine());

            Console.WriteLine("Division step: " + opt.DivisionStep(u1, u2, eps) + "\n");
            Console.WriteLine("Steepest descent: " + opt.SteepestDescent(u1, u2, eps) + "\n");
            Console.WriteLine("Newton: " + opt.Newton(u1, u2, eps) + "\n");
            Console.WriteLine("Penalties: " + opt.Penalties(u1, u2, eps) + "\n");

            
            Console.ReadKey();
        }
    }
}
