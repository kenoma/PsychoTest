using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerMind.stuff;

namespace PokerMind.DataMining
{

    public enum kmeansType
    {
        Forgy,
        RandomPartition,
        kmeanspp
    }

    public static class kmeans
    {
        static object o = new object();

        public static double[][] clustering(double[,] x, int k, int restarts, kmeansType initType)
        {
            double[][] tx = new double[x.GetLength(0)][];
            for (int row = 0; row < x.GetLength(0); row++)
            {
                tx[row] = new double[x.GetLength(1)];
                for (int i = 0; i < tx[row].Length; i++)
                    tx[row][i] = x[row, i];
            }
            return clustering(tx, k, restarts, initType);
        }

        public static double[][] clustering(double[][] x, int k, int restarts, kmeansType initType)
        {
            double[][][] pool = new double[restarts][][];
            double[] error = new double[restarts];
            for (int r = 0; r < restarts; r++)
            {
                Console.WriteLine("Restart {0}", r);
                double er = 0.0;
                pool[r] = clustering(x, k, initType, out er);
                error[r] = er;
                Console.WriteLine("Error {0}", er);
            }
            double min = error.Min();
            int opt = error.FindIndex(z => z == min);
            return pool[opt];
        }

        public static double[][] clustering(double[][] x, int k, int restarts, kmeansType initType, out int[] a)
        {
            double[][][] pool = new double[restarts][][];
            double[] error = new double[restarts];
            int[][] ta = new int[restarts][];
            for (int r = 0; r < restarts; r++)
            {
                Console.WriteLine("Restart {0}", r);
                double er = 0.0;
                pool[r] = clustering(x, k, initType, out er, out ta[r]);
                error[r] = er;
                Console.WriteLine("Error {0}", er);
            }
            double min = error.Min();
            int opt = error.FindIndex(z => z == min);
            a= ta[opt];
            return pool[opt];
        }

        public static double[][] clustering(double[][] x, int k, kmeansType initType, out double error)
        {
            double[][] m = null;
            Console.WriteLine("Initialization...");
            switch (initType)
            {
                case kmeansType.Forgy: m = ForgyInit(x, k); break;
                case kmeansType.kmeanspp: m = kmeansppinit(x, k); break;
                case kmeansType.RandomPartition: m = RandomPartion(x, k); break;
            }
            int reassigned = 0;
            int iters = 0;
            int[] a = new int[x.Length];
            do
            {
                reassigned = Assignment(x, m, ref a);
                if (reassigned != 0)
                    m = Update(x, a, k);
                Console.Write("\rIteration {0}, reassigned {1}            ", iters++, reassigned);
            }
            while (reassigned != 0);
            error = Error(x, m, a);
            return m;
        }

        public static double[][] clustering(double[][] x, int k, kmeansType initType, out double error, out int[] clusterassignance)
        {
            double[][] m = null;
            Console.WriteLine("Initialization...");
            switch (initType)
            {
                case kmeansType.Forgy: m = ForgyInit(x, k); break;
                case kmeansType.kmeanspp: m = kmeansppinit(x, k); break;
                case kmeansType.RandomPartition: m = RandomPartion(x, k); break;
            }
            int reassigned = 0;
            int iters = 0;
            int[] a = new int[x.Length];
            do
            {
                reassigned = Assignment(x, m, ref a);
                if (reassigned != 0)
                    m = Update(x, a, k);
                Console.Write("\rIteration {0}, reassigned {1}       ", iters++, reassigned);
            }
            while (reassigned != 0);
            error = Error(x, m, a);
            clusterassignance = a;
            return m;
        }

        static private double Error(double[][] x, double[][] c, int[] a)
        {
            double sum = 0.0;
            for (int row = 0; row < x.Length; row++)
                sum += Euclidian2Distance(x[row], c[a[row]]);
            return sum / x.Length;
        }

        static private int Assignment(double[][] x, double[][] c, ref int[] a)
        {
            int[] ta = a.ToArray();
            int reassigned = 0;
            Parallel.For(0, x.Length, row =>
            {
                int clust = det.DetectBucket(x[row], c);
                lock (o)
                {
                    if (ta[row] != clust)
                        reassigned++;
                    ta[row] = clust;
                }
            });
            a = ta;
            return reassigned;
        }

        static private double[][] Update(double[][] x, int[] a, int k)
        {
            double[][] m = new double[k][];
            double[] S = new double[k];
            int vlen = x[0].Length;
            for (int c = 0; c < k; c++)
                m[c] = new double[vlen];

            for (int row = 0; row < x.Length; row++)
            {
                int cluster = a[row];
                S[cluster]++;
                for (int i = 0; i < vlen; i++)
                    m[cluster][i] += x[row][i];
            }

            for (int c = 0; c < k; c++)
                if (S[c] != 0)
                    for (int i = 0; i < vlen; i++)
                        m[c][i] /= S[c];
                else
                {
                    Random rnd = new Random(Environment.TickCount);
                    m[c] = x[rnd.Next(x.Length)].ToArray();
                }

            return m;
        }

        static private double[][] ForgyInit(double[][] x, int k)
        {
            int[] num = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                num[i] = i;
            num.Shuffle();
            //x.Shuffle();
            double[][] m = new double[k][];
            for (int i = 0; i < k; i++)
                m[i] = x[num[i]].ToArray();
            return m;
        }

        static private double[][] RandomPartion(double[][] x, int k)
        {
            Random rnd = new Random();
            int[] a = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                a[i] = rnd.Next(k);
            double[][] m = Update(x, a, k);

            return m;
        }

        static private double[][] kmeansppinit(double[][] x, int k)
        {
            Random rnd = new Random(Environment.TickCount);
            List<double[]> m = new List<double[]>();
            m.AddRange(x.OrderBy(z => rnd.NextDouble()).Take(k * 10));

            return kmeansppiniti(m.ToArray(), k);
        }

        static private double[][] kmeansppiniti(double[][] x, int k)
        {
            Random rnd = new Random(Environment.TickCount);
            double[] theta = new double[x.Length];
            List<double[]> m = new List<double[]>();
            m.Add(x[rnd.Next(x.Length)]);

            for (int c = 1; c < k; c++)
            {
                Parallel.For(0, x.Length, row =>
                    {
                        double t = Theta(x[row], m);
                        lock (o)
                        {
                            theta[row] = t;
                        }
                    });
                double tmax = theta.Max();
                theta = theta.Select(z => z >= 0.8 * tmax ? z : 0.0).ToArray();
                int cand = probt(theta, rnd);
                m.Add(x[cand]);
                Console.Write("\r{0} seed...                   ", c);
            }

            return m.ToArray();
        }

        static public double Theta(double[] inp, List<double[]> centers)
        {
            double minsum = double.MaxValue;
            int candidat = -1;
            for (int c = 0; c < centers.Count; c++)
            {
                double sum = 0;
                for (int i = 0; i < centers[0].Length; i++)
                {
                    double tmp = inp[i] - centers[c][i];
                    sum += tmp * tmp;
                    if (sum > minsum) break;
                }
                if (sum < minsum)
                {
                    minsum = sum;
                    candidat = c;
                }
            }
            return minsum;
        }

       static private int probt(double[] par, Random rnd)
        {
            double aver = par.Average();
            if (rnd.NextDouble() > 0.5)
                par = par.Select(x => x >= aver ? x : 0).ToArray();
            double sum = par.Sum();
            
            if (sum <= 0) return 0;
            double rand = sum * rnd.NextDouble();// alglib.hqrnd.hqrnduniformr(rndstat);
            int j = -1;
            while (rand >= 0 && j < par.Length)
            {
                j++;
                rand -= par[j];
            }
            return j;
        }

       static public double Euclidian2Distance(double[] x, double[] y)
       {
           double dist = 0.0;
           for (int i = 0; i < x.Length; i++)
           {
               double tmp = x[i] - y[i];
               dist += tmp * tmp;
           }
           return dist;
       }

       static public int DetectCluster(double[] input, double[][] clusters)
       {
           return det.DetectBucket(input, clusters);
       }

    }



    public static class kmeans1D
    {
        static object o = new object();

        public static double[] clustering(double[] x, int k, int restarts, kmeansType initType, out int[] a)
        {
            double[][] pool = new double[restarts][];
            double[] error = new double[restarts];
            int[][] ta = new int[restarts][];
            for (int r = 0; r < restarts; r++)
            {
                Console.WriteLine("Restart {0}", r);
                double er = 0.0;
                pool[r] = clustering(x, k, initType, out er, out ta[r]);
                error[r] = er;
                Console.WriteLine("Error {0}", er);
            }
            double min = error.Min();
            int opt = error.FindIndex(z => z == min);
            a = ta[opt];
            return pool[opt];
        }

        public static double[] clustering(double[] x, int k, kmeansType initType, out double error)
        {
            double[] m = null;
            Console.WriteLine("Initialization...");
            switch (initType)
            {
                case kmeansType.Forgy: m = ForgyInit(x, k); break;
                case kmeansType.kmeanspp: m = kmeansppinit(x, k); break;
                case kmeansType.RandomPartition: m = RandomPartion(x, k); break;
            }
            int reassigned = 0;
            int iters = 0;
            int[] a = new int[x.Length];
            do
            {
                reassigned = Assignment(x, m, ref a);
                if (reassigned != 0)
                    m = Update(x, a, k);
                Console.Write("\rIteration {0}, reassigned {1}            ", iters++, reassigned);
            }
            while (reassigned != 0);
            error = Error(x, m, a);
            return m;
        }

        public static double[] clustering(double[] x, int k, kmeansType initType, out double error, out int[] clusterassignance)
        {
            double[] m = null;
            Console.WriteLine("Initialization...");
            switch (initType)
            {
                case kmeansType.Forgy: m = ForgyInit(x, k); break;
                case kmeansType.kmeanspp: m = kmeansppinit(x, k); break;
                case kmeansType.RandomPartition: m = RandomPartion(x, k); break;
            }
            int reassigned = 0;
            int iters = 0;
            int[] a = new int[x.Length];
            do
            {
                reassigned = Assignment(x, m, ref a);
                if (reassigned != 0)
                    m = Update(x, a, k);
                Console.Write("\rIteration {0}, reassigned {1}       ", iters++, reassigned);
            }
            while (reassigned != 0);
            error = Error(x, m, a);
            clusterassignance = a;
            return m;
        }

        static private double Error(double[] x, double[] c, int[] a)
        {
            double sum = 0.0;
            for (int row = 0; row < x.Length; row++)
                sum += Euclidian2Distance(x[row], c[a[row]]);
            return sum / x.Length;
        }

        static private int Assignment(double[] x, double[] c, ref int[] a)
        {
            int[] ta = a.ToArray();
            int reassigned = 0;
            Parallel.For(0, x.Length, row =>
            {
                int clust = det.DetectBucket1D(x[row], c);
                lock (o)
                {
                    if (ta[row] != clust)
                        reassigned++;
                    ta[row] = clust;
                }
            });
            a = ta;
            return reassigned;
        }

        static private double[] Update(double[] x, int[] a, int k)
        {
            double[] m = new double[k];
            double[] S = new double[k];

            for (int row = 0; row < x.Length; row++)
            {
                int cluster = a[row];
                S[cluster]++;
                m[cluster] += x[row];
            }

            for (int c = 0; c < k; c++)
                if (S[c] != 0)
                    m[c] /= S[c];
                else
                {
                    Random rnd = new Random(Environment.TickCount);
                    m[c] = x[rnd.Next(x.Length)];
                }

            return m;
        }

        static private double[] ForgyInit(double[] x, int k)
        {
            int[] num = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                num[i] = i;
            num.Shuffle();
            //x.Shuffle();
            double[] m = new double[k];
            for (int i = 0; i < k; i++)
                m[i] = x[num[i]];
            return m;
        }

        static private double[] RandomPartion(double[] x, int k)
        {
            Random rnd = new Random();
            int[] a = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                a[i] = rnd.Next(k);
            double[] m = Update(x, a, k);

            return m;
        }

        static private double[] kmeansppinit(double[] x, int k)
        {
            Random rnd = new Random(Environment.TickCount);
            List<double> m = new List<double>();
            m.AddRange(x.OrderBy(z => rnd.NextDouble()).Take(k * 10));

            return kmeansppiniti(m.ToArray(), k);
        }

        static private double[] kmeansppiniti(double[] x, int k)
        {
            Random rnd = new Random(Environment.TickCount);
            double[] theta = new double[x.Length];
            List<double> m = new List<double>();
            m.Add(x[rnd.Next(x.Length)]);

            for (int c = 1; c < k; c++)
            {
                Parallel.For(0, x.Length, row =>
                {
                    double t = Theta(x[row], m);
                    lock (o)
                    {
                        theta[row] = t;
                    }
                });
                double tmax = theta.Max();
                theta = theta.Select(z => z >= 0.8 * tmax ? z : 0.0).ToArray();
                int cand = probt(theta, rnd);
                m.Add(x[cand]);
                Console.Write("\r{0} seed...                   ", c);
            }

            return m.ToArray();
        }

        static public double Theta(double inp, List<double> centers)
        {
            double minsum = double.MaxValue;
            int candidat = -1;
            for (int c = 0; c < centers.Count; c++)
            {
                double tmp = inp - centers[c];
                double sum = tmp * tmp;

                if (sum < minsum)
                {
                    minsum = sum;
                    candidat = c;
                }
            }
            return minsum;
        }

        static private int probt(double[] par, Random rnd)
        {
            double aver = par.Average();
            if (rnd.NextDouble() > 0.5)
                par = par.Select(x => x >= aver ? x : 0).ToArray();
            double sum = par.Sum();

            if (sum <= 0) return 0;
            double rand = sum * rnd.NextDouble();// alglib.hqrnd.hqrnduniformr(rndstat);
            int j = -1;
            while (rand >= 0 && j < par.Length)
            {
                j++;
                rand -= par[j];
            }
            return j;
        }

        static public double Euclidian2Distance(double x, double y)
        {
            double tmp = x - y;
            return tmp * tmp;
        }
    }

 

}
