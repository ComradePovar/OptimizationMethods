using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO
{
    public class OptimizationMethods
    {
        private double dfu2u2;
        private double dfu1u1;
        private double dfu1u2;
        private Func<double[], double> dfdu2;
        private Func<double[], double> dfdu1;
        private Func<double[], double> function;
        private double dfu2u1;

        private const int MAX_ITERATIONS = 10000;

        public OptimizationMethods(Func<double[], double> f, Func<double[], double> dfdu1, Func<double[], double> dfdu2, double dfu1u1, double dfu1u2, double dfu2u1, double dfu2u2)
        {
            this.function = f;
            this.dfdu1 = dfdu1;
            this.dfdu2 = dfdu2;
            this.dfu1u2 = dfu1u2;
            this.dfu1u1 = dfu1u1;
            this.dfu2u1 = dfu2u1;
            this.dfu2u2 = dfu2u2;
        }

        public double DivisionStep(double u1, double u2, double eps)
        {
            double[] values = divisionStep(u1, u2, eps, out int i);
            Console.WriteLine($"Division step iterations: " + i);

            Console.WriteLine("u1 = " + values[0] + ", u2 = " + values[1]);
            return function(values);
        }
        
        public double SteepestDescent(double u1, double u2, double eps)
        {
            double[] values = steepestDescent(u1, u2, eps, out int i);
            Console.WriteLine($"Steepest descent iterations: " + i);

            Console.WriteLine("u1 = " + values[0] + ", u2 = " + values[1]);
            return function(values);
        }

        public double Penalties(double u1, double u2, double eps)
        {
            double[] values = penalties(u1, u2, eps, out int i);
            Console.WriteLine($"Penalties iterations: " + i);

            Console.WriteLine("u1 = " + values[0] + ", u2 = " + values[1]);
            return function(values);
        }

        private double[] penalties(double u1, double u2, double eps, out int i)
        {
            i = 1;
            double c = 10.0;
            double k = 0;
            double r = 1.0;
            double[] u0 = new double[] { u1, u2 };
            Func<double[], double> dpu1 = u => dfdu1(u) - r / Math.Pow(u[0], 2);
            Func<double[], double> dpu2 = u => dfdu2(u) - r / Math.Pow(u[1], 2);
            Func<double[], double> dpu1u1 = u => dfu1u1 + r * 2.0 / Math.Pow(u[0], 3);
            Func<double[], double> dpu2u2 = u => dfu2u2 + r * 2.0 / Math.Pow(u[1], 3);
            Func<double[], double> lamda = u => 1.0 / (dpu1u1(u) * dpu2u2(u) - dfu1u2 * dfu2u1);


            while (i < MAX_ITERATIONS)
            {
                double gradLength = Math.Sqrt(Math.Pow(dpu1(u0), 2) + Math.Pow(dpu2(u0), 2));
                if (gradLength < eps)
                {
                    if (r < eps)
                    {
                        return u0;
                    }
                }
                else
                {
                    r /= 10.0;
                    double p1 = dpu1(u0);
                    double p2 = dpu2(u0);
                    double lamd = lamda(u0);
                    u0[0] = u0[0] - lamd * p1;
                    u0[1] = u0[1] - lamd * p2;
                }

                i++;
            }

            return u0;
        }

        public double Newton(double u1, double u2, double eps)
        {
            double[] values = newton(u1, u2, eps, out int i);
            Console.WriteLine($"Newton iterations: " + i);

            Console.WriteLine("u1 = " + values[0] + ", u2 = " + values[1]);
            return function(values);
        }

        private double[] newton(double u1, double u2, double eps, out int i)
        {
            i = 0;
            double[] u0 = new double[] { u1, u2 };
            while (i < MAX_ITERATIONS)
            {
                double gradLength = getGradientLength(u0);
                if (gradLength < eps)
                {
                    return u0;
                }

                double[] grads = getGradients(u0);
                double reverseDetH = 1.0 / (dfu1u1 * dfu2u2 - dfu1u2 * dfu2u1);
                double[,] hessianMatrix = new double[,]
                {
                    { reverseDetH*dfu2u2, reverseDetH*(-dfu1u2) },
                    { reverseDetH*(-dfu2u1), reverseDetH*dfu1u1 }
                };

                u0[0] = u0[0] - (grads[0] * hessianMatrix[0, 0] + grads[1] * hessianMatrix[1, 0]);
                u0[1] = u0[1] - (grads[0] * hessianMatrix[0, 1] + grads[1] * hessianMatrix[1, 1]);
                i++;
            }

            return u0;
        }
        private double[] steepestDescent(double u1, double u2, double eps, out int i)
        {
            i = 1;
            double[] u0 = new double[] { u1, u2 };
            while (i < MAX_ITERATIONS)
            {
                double gradLenght = getGradientLength(u0);
                if (gradLenght < eps)
                {
                    return u0;
                }

                double[] grads = getGradients(u0);
                double alpha = GetAlpha(u0, grads, eps, ref i);
                u0[0] = u0[0] - alpha * grads[0];
                u0[1] = u0[1] - alpha * grads[1];
                i++;
            }
            return u0;
        }
        private double GetAlpha(double[] u, double[] grads, double eps, ref int i)
        {
            double a = 1.0;
            double prevA = -a;
            double[] point = new double[]
            {
                u[0] - grads[0] * a,
                u[1] - grads[1] * a
            };

            while (Math.Abs(function(point)) > eps && a != prevA && i < MAX_ITERATIONS)
            {
                prevA = a;
                double dfu = -6 * grads[0] * point[0] - 4 * grads[0] * point[1] - 4 * grads[1] * point[0] - 6 * grads[1] * point[1];
                double dfu2 = 6 * Math.Pow(grads[0], 2) + 8 * grads[0] * grads[1] + 6 * Math.Pow(grads[1], 2);
                a -= dfu / dfu2;

                point[0] = u[0] - grads[0] * a;
                point[1] = u[1] - grads[1] * a;
                i++;
            }
            return a;
        }
        private double getGradientLength(params double[] u)
        {
            return Math.Sqrt(Math.Pow(dfdu1(u), 2) + Math.Pow(dfdu2(u), 2));
        }
        private double[] getGradients(params double[] u)
        {
            return new double[]
            {
                dfdu1(u),
                dfdu2(u)
            };
        }

        private double[] divisionStep(double u1, double u2, double eps, out int i)
        {
            double alpha = 1.0;
            i = 1;

            double[] u = new double[2];
            double[] u0 = new double[] { u1, u2 };
            double j0 = function(u0);
            while (i < MAX_ITERATIONS)
            {
                double test = function(u0);
                double gradLength = getGradientLength(u0);
                if (gradLength < eps)
                {
                    return u0;
                }

                while (true)
                {
                    double[] grads = getGradients(u0);
                    u[0] = u0[0] - alpha * grads[0];
                    u[1] = u0[1] - alpha * grads[1];

                    double j1 = function(u);
                    if (j1 < j0)
                    {
                        u0 = u;
                        j0 = j1;
                        break;
                    }
                    else
                    {
                        alpha /= 2.0;
                    }
                    i++;
                }
                i++;
            }
            return u0;
        }
    }
}
