using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGBoost;

namespace Psycho.Validator
{
    class Fifther
    {
        public List<int> percentiles = new List<int>();
        public List<XGBClassifier> xgb { get; set; } = new List<XGBClassifier>();

        public void AddLevel(int questionNum, XGBClassifier regressor)
        {
            percentiles.Add(questionNum);
            xgb.Add(regressor);
        }

        public void Save(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, "qnums.perc"), JsonConvert.SerializeObject(percentiles));
            for (int pos = 0; pos < xgb.Count; pos++)
            {
                xgb[pos].SaveModelToFile(Path.Combine(path, $"{pos:000}.xgb"));
            }
        }

        public void Load(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);

            percentiles = JsonConvert.DeserializeObject<List<int>>(File.ReadAllText(Path.Combine(path, "qnums.perc")));

            var files = Directory.GetFiles(path, "*.xgb");
            xgb = (from f in files
                   let x = BaseXgbModel.LoadClassifierFromFile(f)
                   let fi = new FileInfo(f)
                   let fn = int.Parse(fi.Name.Replace(".xgb", ""))
                   orderby fn ascending
                   select x).ToList();
            foreach (var x in xgb)
            {
                x.SetParameter("num_class", 5);
            }
        }

        public float[] Predict(float[] input)
        {
            var retval = new List<float>();
            var inp = new float[][] { input };
            for (int pos = 0; pos < xgb.Count; pos++)
            {
                var r = xgb[pos].Predict(inp);
                retval.Add(r[0]);
            }
            return retval.ToArray();
        }

        public float[][] Predict(float[][] input, int dim)
        {
            var res = new float[input.Length][];
            for (int pos = 0; pos < input.Length; pos++)
            {
                res[pos] = new float[dim];
            }

            for (int x = 0; x < xgb.Count; x++)
            {
                var r = xgb[x].Predict(input);
                for (int pos = 0; pos < input.Length; pos++)
                {
                    res[pos][x] = r[pos];
                }
            }
            return res;
        }

        public float[][][] PredictDistr(float[][] input, int dim)
        {
            var res = new float[input.Length][][];
            for (int pos = 0; pos < input.Length; pos++)
            {
                res[pos] = new float[xgb.Count][];
                for (int i = 0; i < xgb.Count; i++)
                    res[pos][i] = new float[dim];
            }

            for (int x = 0; x < xgb.Count; x++)
            {
                var r = xgb[x].PredictDistr(input);
                for (int pos = 0; pos < input.Length; pos++)
                {
                    for (int i = 0; i < dim; i++)
                        res[pos][x][i] = r[pos][i];
                }
            }
            return res;
        }
    }
}
