using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerMind.stuff
{
    public static class det
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        static public double[] Transform(double[] x, double[,] U, double[] mean, int level)
        {
            double[] res = new double[level];

            for (int i = 0; i < level; i++)
                for (int j = 0; j < x.Length; j++)
                    res[i] += U[i, j] * (x[j] - mean[j]);
            return res;
        }

        static public int DetectBucket(double[] inp, double[,] centers)
        {
            double minsum = double.MaxValue;
            int candidat = -1;
            for (int c = 0; c < centers.GetLength(0); c++)
            {
                double sum = 0;
                for (int i = 0; i < centers.GetLength(1); i++)
                {
                    double tmp = inp[i] - centers[c, i];
                    sum += tmp * tmp;
                    if (sum > minsum) break;
                }
                if (sum < minsum)
                {
                    minsum = sum;
                    candidat = c;
                }
            }
            return candidat;
        }

        static public int DetectBucket(double[] inp, double[,] centers, out double[] U)
        {
            double minsum = double.MaxValue;
            int candidat = -1;
            double[] u = new double[centers.GetLength(0)];
            object o = new object();
            //for (int c = 0; c < centers.GetLength(0); c++)
            Parallel.For(0, centers.GetLength(0), c =>
            {
                double sum = 0;
                for (int i = 0; i < centers.GetLength(1); i++)
                {
                    double tmp = inp[i] - centers[c, i];
                    sum += tmp * tmp;
                    //if (sum > minsum) break;
                }
                lock (o)
                {
                    u[c] = sum;
                    if (sum < minsum)
                    {
                        minsum = sum;
                        candidat = c;
                    }
                }
            });
            U = u;
            return candidat;
        }

        static public int DetectBucket(int val, double[,] source, double[,] centers)
        {
            double minsum = double.MaxValue;
            int candidat = -1;
            for (int c = 0; c < centers.GetLength(0); c++)
            {
                double sum = 0;
                for (int i = 0; i < centers.GetLength(1); i++)
                {
                    double tmp = source[val, i] - centers[c, i];
                    sum += tmp * tmp;
                    if (sum > minsum) break;
                }
                if (sum < minsum)
                {
                    minsum = sum;
                    candidat = c;
                }
            }
            return candidat;
        }


        static public double EuclidianDistance(double[] inp, double[] c)
        {
            double dist = 0.0;
            for (int i = 0; i < c.Length; i++)
            {
                double x=inp[i] - c[i];
                dist += x * x;
            }
            return Math.Sqrt(dist);
        }

        static public double EuclidianDistance(float[] inp, float[] c)
        {
            double dist = 0.0;
            for (int i = 0; i < c.Length; i++)
            {
                double x = inp[i] - c[i];
                dist += x * x;
            }
            return Math.Sqrt(dist);
        }

        static public int GetMaxIndex(float[] inp)
        {
            float max = inp.Max();
            double dist = 0.0;
            for (int i = 0; i < inp.Length; i++)
                if (inp[i] == max)
                    return i;
            return -1;
        }

        static public double EuclidianDistance(int x, int y, double[,] c)
        {
            double dist = 0.0;
            for (int i = 0; i < c.GetLength(1); i++)
            {
                double tmp = c[x, i] - c[y, i];
                dist += tmp * tmp;
            }
            return Math.Sqrt(dist);
        }

        static public double EuclidianDistance(int x, int y, double[][] c)
        {
            double dist = 0.0;
            for (int i = 0; i < c[0].Length; i++)
            {
                double tmp = c[x][i] - c[y][i];
                dist += tmp * tmp;
            }
            return Math.Sqrt(dist);
        }

        static public int DetectBucket1D(double inp, double[] centers)
        {
            double minsum = double.MaxValue;
            int candidat = -1;
            for (int c = 0; c < centers.Length; c++)
            {

                double tmp = inp - centers[c];
                double sum = tmp * tmp;
                if (sum < minsum)
                {
                    minsum = sum;
                    candidat = c;
                }
            }
            return candidat;
        }

        static public int DetectBucket(double[] inp, double[][] centers)
        {
            double minsum = double.MaxValue;
            int candidat = -1;
            for (int c = 0; c < centers.GetLength(0); c++)
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
            return candidat;
        }

      
        static public void Membership(double[] x, double[,] centers,  double rate, out int[] B, out double[] W)
        {
            int classes = centers.GetLength(0);
            double[] res = new double[classes];
            double[] dist = new double[classes];

            List<int> rb = new List<int>();
            List<double> rw = new List<double>();
            double min = double.MaxValue;
            int indmin = 0;
            for (int k = 0; k < res.Length; k++)
            {
                double d = 0.0;
                for (int i = 0; i < x.Length; i++)
                {
                    double tmp = x[i] - centers[k, i];
                    d += tmp * tmp;
                }
                dist[k] = d;
                if (dist[k] < min)
                {
                    min = dist[k];
                    indmin = k;
                }
            }

            double sum = 0.0;
            min *= rate;
            for (short k = 0; k < res.Length; k++)
            {
                if (dist[k] < min)
                {
                    rb.Add(k);

                    rw.Add(dist[k]);
                    sum += dist[k];
                }
            }
            for (int i = 0; i < rw.Count; i++)
                rw[i] /= sum;
            

            B = rb.ToArray();
            W = rw.ToArray();
        }


    }

    public static class EnumerableExtensions
    {
        // Finds an item matching a predicate in the enumeration, much like List<T>.FindIndex()
        public static int FindIndex<T>(this IEnumerable<T> list, Predicate<T> finder)
        {
            int index = 0;
            foreach (var item in list)
            {
                if (finder(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    }
}
