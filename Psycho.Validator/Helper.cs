using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psycho.Validator
{
    static class Helper
    {

        static public double Percentile(this IEnumerable<float> sequences, double excelPercentile)
        {
            var sequence = sequences.ToArray();
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1d)
                return sequence[0];
            else if (n == N)
                return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="intervals">Ordered by asc</param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public int GetIntervalIndex(this IList<double> intervals, double value)
        {
            if (intervals.Count == 0)
                return 0;
            var match = 0;
            for (int i = 0; i < intervals.Count; i++)
            {
                if (value > intervals[i])
                    match = i + 1;
                else
                    break;
            }
            return match;
        }
    }
}
