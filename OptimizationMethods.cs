using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO
{
    public static class OptimizationMethods
    {
        public static double Bisection(Func<double, double> function, Func<double, double> df, Func<double, double> d2f, double a, double b, double eps)
        {
            int i, k;
            double value = bisection(function, df, d2f, a, b, eps, out i);
            Console.WriteLine($"Bisection iterations: {i}");

            return value;
        }

        public static double GoldenRatio(Func<double, double> function, Func<double, double> df, Func<double, double> d2f, double a, double b, double eps)
        {
            int i;
            double value = goldenRatio(function, df, d2f, a, b, eps, out i);
            Console.WriteLine("Golden ratio iterations: " + i);

            return value;
        }

        public static double Newton(Func<double, double> function, Func<double, double> df, Func<double, double> d2f, double a, double b, double eps)
        {
            int i;
            double value = newton(function, df, d2f, a, b, eps, out i);
            Console.WriteLine("Newton iterations: " + i);

            return value;
        }

        public static double Parabola(Func<double, double> function, Func<double, double> df, Func<double, double> d2f, double a, double b, double eps)
        {
            int i;
            double value = parabola(function, df, d2f, a, b, eps, out i);
            Console.WriteLine("Parabola iterations: " + i);

            return value;
        }

        private static bool isUnimodal(Func<double, double> df, Func<double, double> d2f, double a, double b, double step)
        {

            if (df(a) >= 0 && df(b) <= 0 || df(a) <= 0 && df(b) >= 0)
                while (a <= b)
                {
                    if (d2f(a) < 0)
                        return false;

                    a += step;
                }
            else
                return false;
            return true;
        }

        private static double goldenRatio(Func<double, double> function, Func<double, double> df, Func<double, double> d2f, double a, double b, double eps, out int i, int maxIterations = 10000)
        {
            if (!isUnimodal(df, d2f, a, b, eps))
                throw new ArgumentException("Function is not unimodal");

            double alpha = (Math.Sqrt(5) - 1) / 2;
            double alpha1 = (3 - Math.Sqrt(5)) / 2;
            double u1 = a + alpha1 * (b - a);
            double u2 = a + alpha * (b - a);
            
            for (i = 0; (b - a) >= eps && i < maxIterations; i++)
            {
                double j1 = function(u1), j2 = function(u2);
                if (j1 < j2)
                {
                    b = u2;
                    u2 = u1;
                    j2 = j1;
                    u1 = a + alpha1 * (b - a);
                    j1 = function(u1);
                }
                else if (j1 > j2)
                {
                    a = u1;
                    u1 = u2;
                    j1 = j2;
                    u2 = a + alpha * (b - a);
                    j2 = function(u2);
                }
                else
                {
                    b = u2;
                    a = u1;
                    u1 = a + alpha1 * (b - a);
                    u2 = a + alpha * (b - a);
                    j1 = function(u1);
                    j2 = function(u2);
                }
            }

            return (b + a) / 2;
        }

        private static double bisection(Func<double, double> function, Func<double, double> df, Func<double, double> d2f, double a, double b, double eps, out int i, int maxIterations = 10000)
        {
            if (!isUnimodal(df, d2f, a, b, eps))
                throw new ArgumentException("Function is not unimodal");

            double delta = 0.001;
            double A = a, B = b;
            for (i = 0; i < maxIterations && (B - A) >= eps; i++)
            {
                double u1 = (B + A - delta) / 2;
                double u2 = (B + A + delta) / 2;
                double j1 = function(u1);
                double j2 = function(u2);
                if (j1 < j2)
                {
                    B = u2;
                }
                else if (j1 > j2)
                {
                    A = u1;
                }
                else
                {
                    A = u1;
                    B = u2;
                }
            }
                
            return (B + A) / 2;
        }

        private static double newton(Func<double, double> function, Func<double, double> df, Func<double, double> d2f, double a, double b, double eps, out int i, int maxIterations = 10000)
        {
            int j;

            int iterationsCount = 2;
            while (true)
            {
                double u = goldenRatio(function, df, d2f, a, b, eps, out j, iterationsCount);

                for (i = 0; i < maxIterations; i++)
                {
                    double deriative = df(u);
                    if (Math.Abs(deriative) <= eps)
                    {
                        return u;
                    }

                    u = u - deriative / d2f(u);
                }

                iterationsCount++;
            }
        }

        private static double getW(double delta1, double delta2, double u1, double u2, double u3)
        {
            return u2 + (Math.Pow(u3 - u2, 2) * delta1 - Math.Pow(u2 - u1, 2) * delta2) / (2 * ((u3 - u2) * delta1 + (u2 - u1) * delta2));
        }
        private static double parabola(Func<double, double> function, Func<double, double> df, Func<double, double> d2f, double a, double b, double eps, out int i, int maxIterations = 10000)
        {
            if (!isUnimodal(df, d2f, a, b, eps))
                throw new ArgumentException("Function is not unimodal");

            double u1 = a;
            double u2 = (a + b) / 2;
            double u3 = b;

            i = 1;

            while (i < maxIterations)
            {
                double delta1 = function(u1) - function(u2);
                double delta2 = function(u3) - function(u2);
                double w = getW(delta1, delta2, u1, u2, u3);
                double fw = function(w);
                double fu2 = function(u2);

                if (w < u2)
                {
                    if (fw < fu2)
                    {
                        u3 = u2;
                        u2 = w;
                    }
                    else if (fw > fu2)
                    {
                        u1 = w;
                    }
                    else
                    {
                        if (function(u1) > fu2)
                        {
                            u3 = u2;
                            u2 = w;
                        }
                        else if (fu2 > function(u3))
                        {
                            u1 = w;
                        }
                    }
                }
                else if (w > u2)
                {
                    if (fw < fu2)
                    {
                        u1 = u2;
                        u2 = w;
                    }
                    else if (fw > fu2)
                    {
                        u3 = w;
                    }
                    else
                    {
                        if (function(u3) > fu2)
                        {
                            u1 = u2;
                            u2 = w;
                        }
                        else if (function(u1) > fu2)
                        {
                            u3 = w;
                        }
                    }
                }
                else
                {
                    double delta = 0.001;
                    while (delta >= eps)
                    {
                        double x1 = u2 - delta;
                        double x2 = u2 + delta;
                        if (function(x1) < fu2)
                        {
                            u2 = x1;
                            break;
                        }
                        else if (function(x2) < fu2)
                        {
                            u2 = x2;
                            break;
                        }
                        delta /= 10;
                    }

                    if (delta < eps)
                        return u2;
                }



                if (Math.Abs(w - getW(delta1, delta2, u1, u2, u3)) < eps)
                    return u2;
                i++;
            }

            return u2;
        }
    }
}
