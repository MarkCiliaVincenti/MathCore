﻿using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MathCore.Statistic
{
    public static partial class Distributions
    {
        public static class Erlang
        {
            //public static double ModelA(double )

            private static double modelB_(double A, int n) => Math.Pow(A, n) / n.Factorial();
            public static double ModelB(double A, int N, int n)
            {
                Contract.Requires(N > 0);
                Contract.Requires(n >= 0);

                double P = 1, q = 1, Q = 0;

                for(var i = 1; i <= N; i++)
                {
                    Q += q *= A / i;
                    if(i == n) P = q;
                }

                return P / Q;

                //return modelB_(A, N) / Enumerable.Range(0, N).Aggregate(0d, (S, i) => S + modelB_(A, i));
            }
            public static double ModelB(double A, int N) => ModelB(A, N, N);

            public static double[] GetDistribution(double A, int N) => GetDistribution(A, N, N);
            public static double[] GetDistribution(double A, int N, int n_max)
            {
                var p = new double[n_max + 1];


                var Q = p[0] = 1;
                
                for(int i = 1, NN = Math.Max(n_max, N); i <= NN; i++)
                {
                    if(i <= n_max) p[i] = p[i-1] * A / i;
                    if(i <= N) Q += p[i];
                }

                return p.Divade(Q);
            }


            private static double modelC_(double A, int N) => Math.Pow(A, N) * N / (N.Factorial() * (N - A));
            public static double ModelC_OC(double A, int N) => 1 / (modelC_(A, N) + Enumerable.Range(0, N - 1).Aggregate(0d, (S, i) => modelB_(A, i)));
            public static double ModelC(double A, int N) => ModelC_OC(A, N) * modelC_(A, N);
        }
    }
}