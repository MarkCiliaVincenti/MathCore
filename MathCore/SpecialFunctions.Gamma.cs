﻿using static System.Math;

// ReSharper disable UnusedMember.Global

namespace MathCore;

public static partial class SpecialFunctions
{
    /// <summary>Гамма-функция</summary>
    public static class Gamma
    {
        /// <summary>Вычисляет гамма-функцию для заданного значения x</summary>
        /// <param name="x">Значение, для которого вычисляется гамма-функция. Может быть как положительным, так и отрицательным.</param>
        /// <returns>Значение гамма-функции для x.</returns>
        /// <remarks>
        /// Если абсолютное значение x больше 33, используется метод Стирлинга для приближенного вычисления гамма-функции.
        /// Для значений x меньше 2, функция нормализуется путем деления на x до достижения значения 2.
        /// Для значений x больших или равных 3, функция умножается на x до достижения значения меньше 3.
        /// </remarks>
        [DST]
        public static double G(double x)
        {
            double z;
            var    sgn_gam = 1;
            var    q       = Abs(x);
            if (q > 33)
            {
                if (x >= 0) z = GStir(x);
                else
                {
                    double p                = (int)Floor(q);
                    var    i                = (int)Round(p);
                    if (i % 2 == 0) sgn_gam = -1;
                    z = q - p;
                    if (z > .5) z = q - ++p;
                    z = q * Sin(Consts.pi * z);
                    z = Abs(z);
                    z = PI / (z * GStir(q));
                }
                return sgn_gam * z;
            }
            z = 1;

            while (x >= 3) z *= --x;

            while (x < 0)
                if (x <= -.000000001) z /= x++;
                else
                    return z / ((1 + .5772156649015329 * x) * x);

            while (x < 2)
                if (x >= .000000001) z /= x++;
                else
                    return z / ((1 + .5772156649015329 * x) * x);

            if (Abs(x - 2) < Eps) return z;
            x -= 2;

            var pp = 1.60119522476751861407E-4;
            pp = 1.19135147006586384913E-3 + x * pp;
            pp = 1.04213797561761569935E-2 + x * pp;
            pp = 4.76367800457137231464E-2 + x * pp;
            pp = 2.07448227648435975150E-1 + x * pp;
            pp = 4.94214826801497100753E-1 + x * pp;
            pp = 9.99999999999999996796E-1 + x * pp;

            var qq = -2.31581873324120129819E-5;
            qq = 5.39605580493303397842E-4 + x * qq;
            qq = -4.45641913851797240494E-3 + x * qq;
            qq = 1.18139785222060435552E-2 + x * qq;
            qq = 3.58236398605498653373E-2 + x * qq;
            qq = -2.34591795718243348568E-1 + x * qq;
            qq = 7.14304917030273074085E-2 + x * qq;
            qq = 1.00000000000000000320 + x * qq;

            return z * pp / qq;
        }

        /// <summary>Вычисляет приближенное значение гамма-функции с использованием метода Стирлинга для больших значений x.</summary>
        /// <param name="x">Значение, для которого вычисляется гамма-функция. Должно быть положительным.</param>
        /// <returns>Приближенное значение гамма-функции для x.</returns>
        /// <remarks>Метод Стирлинга используется для вычисления гамма-функции, так как он обеспечивает хорошую точность для больших значений x.</remarks>
        [DST]
        private static double GStir(double x)
        {
            var w    = 1 / x;
            var stir = 7.87311395793093628397E-4;
            stir = -2.29549961613378126380E-4 + w * stir;
            stir = -2.68132617805781232825E-3 + w * stir;
            stir = 3.47222221605458667310E-3 + w * stir;
            stir = 8.33333333333482257126E-2 + w * stir;
            w    = 1 + w * stir;
            var y = Exp(x);

            if (x <= 143.01608) y = Pow(x, x - .5) / y;
            else
            {
                var v = Pow(x, .5 * x - .25);
                y = v * v / y;
            }
            return 2.50662827463100050242 * y * w;
        }

        /// <summary>Вычисляет натуральный логарифм гамма-функции.</summary>
        /// <param name="x">Значение, для которого вычисляется натуральный логарифм гамма-функции. Должно быть положительным.</param>
        /// <returns>Приближенное значение натурального логарифма гамма-функции для x.</returns>
        /// <remarks>Метод Стирлинга используется для вычисления гамма-функции, так как он обеспечивает хорошую точность для больших значений x.</remarks>
        [DST]
        public static double LnG(double x) => LnG(x, out _);

        /// <summary>Вычисляет натуральный логарифм гамма-функции</summary>
        /// <param name="x">Значение, для которого вычисляется натуральный логарифм гамма-функции. Должно быть положительным.</param>
        /// <param name="sign">Знак гамма-функции.</param>
        /// <returns>Приближенное значение натурального логарифма гамма-функции для x.</returns>
        /// <remarks>Метод Стирлинга используется для вычисления гамма-функции, так как он обеспечивает хорошую точность для больших значений x.</remarks>
        [DST]
        public static double LnG(double x, out int sign)
        {
            double p;
            double q;
            double z;
            sign = 1;
            const double log_pi   = 1.14472988584940017414;
            const double lc_Ls2Pi = .91893853320467274178;
            if (x < -34)
            {
                q = -x;
                var w = LnG(q, out _);
                p = (int)Floor(q);
                var i = (int)Round(p);
                sign = i % 2 == 0 ? -1 : 1;
                z    = q - p;
                if (z > .5) z = ++p - q;
                return log_pi - Log(q * Sin(Consts.pi * z)) - w;
            }
            if (x < 13)
            {
                z = 1;
                p = 0;
                var u = x;

                while (u >= 3)
                {
                    u =  x + --p;
                    z *= u;
                }

                while (u < 2)
                {
                    z /= u;
                    u =  x + ++p;
                }

                if (z >= 0) sign = 1;
                else
                {
                    sign = -1;
                    z    = -z;
                }

                if (Abs(u - 2) < Eps) return Log(z);

                p -= 2;
                x += p;

                var b = -1378.25152569120859100;
                b = -38801.6315134637840924 + x * b;
                b = -331612.992738871184744 + x * b;
                b = -1162370.97492762307383 + x * b;
                b = -1721737.00820839662146 + x * b;
                b = -853555.664245765465627 + x * b;

                var c = 1.0;
                c = -351.815701436523470549 + x * c;
                c = -17064.2106651881159223 + x * c;
                c = -220528.590553854454839 + x * c;
                c = -1139334.44367982507207 + x * c;
                c = -2532523.07177582951285 + x * c;
                c = -2018891.41433532773231 + x * c;

                return Log(z) + x * b / c;
            }
            q = (x - .5) * Log(x) - x + lc_Ls2Pi;
            if (x > 100000000) return q;

            p = 1 / (x * x);
            if (x >= 1000)
                q += ((7.9365079365079365079365 * .0001 * p - 2.7777777777777777777778 * .001) * p
                    + .0833333333333333333333) / x;
            else
            {
                var a = 8.11614167470508450300 * .0001;
                a =  -(5.95061904284301438324 * .0001) + p * a;
                a =  7.93650340457716943945 * .0001 + p * a;
                a =  -(2.77777777730099687205 * .001) + p * a;
                a =  8.33333333333331927722 * .01 + p * a;
                q += a / x;
            }
            return q;
        }

        private const int __GammaN = 10;
        private const double __GammaR = 10.900511;
        private static readonly double[] __GammaDk =
        [
            2.48574089138753565546e-5,
            1.05142378581721974210,
            -3.45687097222016235469,
            4.51227709466894823700,
            -2.98285225323576655721,
            1.05639711577126713077,
            -1.95428773191645869583e-1,
            1.70970543404441224307e-2,
            -5.71926117404305781283e-4,
            4.63399473359905636708e-6,
            -2.71994908488607703910e-9
        ];

        /// <summary>
        /// Вычисляет натуральный логарифм гамма-функции. Метод использует
        /// разложение гамма-функции в рядом по 1/z.
        /// </summary>
        /// <param name="z">Значение, для которого вычисляется натуральный логарифм гамма-функции.</param>
        /// <returns>Приближенное значение натурального логарифма гамма-функции для z.</returns>
        private static double Ln(in double z)
        {
            if (z < 0.5)
            {
                var s = __GammaDk[0];
                for (var i = 1; i <= __GammaN; i++) 
                    s += __GammaDk[i] / (i - z);

                return Consts.LnPi
                    - Log(Sin(Consts.pi * z))
                    - Log(s)
                    - Consts.Ln2Sqrt_e_div_pi
                    - (0.5 - z) * Log((0.5 - z + __GammaR) / E);
            }
            else
            {
                var s = __GammaDk[0];
                for (var i = 1; i <= __GammaN; i++)
                    s += __GammaDk[i] / (z + i - 1.0);

                return Log(s)
                    + Consts.Ln2Sqrt_e_div_pi
                    + (z - 0.5) * Log((z - 0.5 + __GammaR) / E);
            }
        }

        /// <summary>Вычисляет обратное значение к регуляризованной нижней функции гамма-распределения</summary>
        /// <param name="a">Параметр гамма-распределения.</param>
        /// <param name="y0">Значение, для которого вычисляется обратное значение.</param>
        /// <returns>Обратное значение к регуляризованной нижней функции гамма-распределения.</returns>
        /// <remarks>
        ///     Метод использует интерполяцию или итерации Ньютона, чтобы найти корень уравнения lower_regularized(a, x) - y = 0.
        /// </remarks>
        public static double LowerRegularizedInv(double a, double y0)
        {
            const double epsilon   = 0.000000000000001;
            const double big       = 4503599627370496.0;
            const double threshold = 5 * epsilon;

            if (double.IsNaN(a) || double.IsNaN(y0))
                return double.NaN;

            if (a < 0 || a.EqualWithAccuracy(0.0)) throw new ArgumentOutOfRangeException(nameof(a), a, "a должно быть больше 0");
            if (y0 is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(y0), y0, "y0 должно быть в интервале [0..1]");
            if (y0.EqualWithAccuracy(0.0)) return 0d;
            if (y0.EqualWithAccuracy(1.0)) return double.PositiveInfinity;

            y0 = 1 - y0;

            var x_upper = big;
            var x_lower = 0d;
            var y_upper = 1d;
            var y_lower = 0d;

            // Initial Guess
            var d   = 1d / (9 * a);
            var y   = 1 - d - 0.98 * Consts.sqrt_2 * Erf.Inv(2.0 * y0 - 1.0) * Sqrt(d);
            var x   = a * y * y * y;
            var lgm = Ln(a);

            for (var i = 0; i < 20; i++)
            {
                if (x < x_lower || x > x_upper)
                {
                    d = 0.0625;
                    break;
                }

                y = 1 - LowerRegularized(a, x);
                if (y < y_lower || y > y_upper)
                {
                    d = 0.0625;
                    break;
                }

                if (y < y0)
                {
                    x_upper = x;
                    y_lower = y;
                }
                else
                {
                    x_lower = x;
                    y_upper = y;
                }

                d = (a - 1) * Log(x) - x - lgm;
                if (d < -709.78271289338399)
                {
                    d = 0.0625;
                    break;
                }

                d = -Exp(d);
                d = (y - y0) / d;
                if (Abs(d / x) < epsilon) return x;

                if (d > x / 4 && y0 < 0.05)
                    // Naive heuristics for cases near the singularity
                    d = x / 10;

                x -= d;
            }

            if (x_upper == big)
            {
                if (x <= 0) x = 1;

                while (x_upper == big)
                {
                    x = (1 + d) * x;
                    y = 1 - LowerRegularized(a, x);
                    if (y < y0)
                    {
                        x_upper = x;
                        y_lower = y;
                        break;
                    }

                    d += d;
                }
            }

            var dir = 0;
            d = 0.5;
            for (var i = 0; i < 400; i++)
            {
                x   = x_lower + d * (x_upper - x_lower);
                y   = 1 - LowerRegularized(a, x);
                lgm = (x_upper - x_lower) / (x_lower + x_upper);
                if (Abs(lgm) < threshold) return x;

                lgm = (y - y0) / y0;
                if (Abs(lgm) < threshold) return x;

                if (x <= 0d) return 0d;

                if (y >= y0)
                {
                    x_lower = x;
                    y_upper = y;
                    if (dir < 0)
                    {
                        dir = 0;
                        d   = 0.5;
                    }
                    else
                        d = dir > 1 ? 0.5 * d + 0.5 : (y0 - y_lower) / (y_upper - y_lower);

                    dir += 1;
                }
                else
                {
                    x_upper = x;
                    y_lower = y;
                    if (dir > 0)
                    {
                        dir = 0;
                        d   = 0.5;
                    }
                    else
                        d = dir < -1 ? 0.5 * d : (y0 - y_lower) / (y_upper - y_lower);

                    dir -= 1;
                }
            }

            return x;
        }

        /// <summary>Вычисляет нижнюю регуляризованную гамма-функцию</summary>
        /// <param name="a">Первый параметр гамма-функции (a &gt; 0).</param>
        /// <param name="x">Второй параметр гамма-функции (x &gt; 0).</param>
        /// <returns>Значение нижней регуляризованной гамма-функции.</returns>
        /// <exception cref="ArgumentOutOfRangeException">a или x меньше 0.</exception>
        public static double LowerRegularized(double a, double x)
        {
            const double epsilon = 0.000000000000001;
            const double big     = 4503599627370496.0;
            const double big_inv = 2.22044604925031308085e-16;

            if (a < 0d)
                throw new ArgumentOutOfRangeException(nameof(a));

            if (x < 0d)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (a.EqualWithAccuracy(0.0))
            {

                if (x.EqualWithAccuracy(0.0)) //use right hand limit value because so that regularized upper/lower gamma definition holds.
                    return 1d;

                return 1d;
            }

            if (x.EqualWithAccuracy(0.0))
                return 0d;

            var ax = a * Log(x) - x - Ln(a);
            if (ax < -709.78271289338399)
                return a < x ? 1d : 0d;

            if (x <= 1 || x <= a)
            {
                var    r2   = a;
                double c2   = 1;
                double ans2 = 1;

                do
                {
                    r2   += 1;
                    c2   =  c2 * x / r2;
                    ans2 += c2;
                }
                while (c2 / ans2 > epsilon);

                return Exp(ax) * ans2 / a;
            }

            var c = 0;
            var y = 1 - a;
            var z = x + y + 1;

            double p3  = 1;
            var    q3  = x;
            var    p2  = x + 1;
            var    q2  = z * x;
            var    ans = p2 / q2;

            double error;

            do
            {
                c++;
                y += 1;
                z += 2;
                var yc = y * c;

                var p = p2 * z - p3 * yc;
                var q = q2 * z - q3 * yc;

                if (q != 0)
                {
                    var next_ans = p / q;
                    error = Abs((ans - next_ans) / next_ans);
                    ans   = next_ans;
                }
                else
                    // zero div, skip
                    error = 1;

                // shift
                p3 = p2;
                p2 = p;
                q3 = q2;
                q2 = q;

                // normalize fraction when the numerator becomes large
                if (Abs(p) > big)
                {
                    p3 *= big_inv;
                    p2 *= big_inv;
                    q3 *= big_inv;
                    q2 *= big_inv;
                }
            }
            while (error > epsilon);

            return 1d - Exp(ax) * ans;
        }
    }
}