using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TSP_Genetic.DataLogic
{
    internal class SaveDataToTxt
    {
        public void WriteToTxt<T1, T2, T3>(string path, string name, int[] sizes,
                                            List<(T1, T2)[]> data,
                                            List<(T1, T2, T3)[]> results)
        {
            string fullPath = Path.Combine(path, name);

            using (StreamWriter sw = new StreamWriter(fullPath))
            {
                // Write sizes
                if (sizes != null && sizes.Length > 0)
                {
                    sw.WriteLine(string.Join(" ", sizes));
                    sw.WriteLine();
                }

                // Write data
                for (int i = 0; i < data.Count; i++)
                {
                    foreach (var tuple in data[i])
                    {
                        sw.WriteLine($"{tuple.Item1} {tuple.Item2}");
                    }
                    sw.WriteLine();
                }

                // Write results
                for (int i = 0; i < results.Count; i++)
                {
                    foreach (var tuple in results[i])
                    {
                        sw.WriteLine($"{tuple.Item1}, {tuple.Item2}, {tuple.Item3}");
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}
