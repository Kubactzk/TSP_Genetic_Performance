using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Genetic.TxtDataForPlotting
{
    internal class histToTxt
    {
        public static void SaveBenchmarkData(string filePath, int[] sizes, List<double> benchmarkTimes)
        {
            // Assuming benchmarkTimes contains the times for each run of each size in sequence
            int runsPerSize = benchmarkTimes.Count / sizes.Length;

            // Create a list to store all the times for each size
            var allTimes = new List<(int Size, double[] Times)>();

            for (int i = 0; i < sizes.Length; i++)
            {
                // Get the times for the current size
                var timesForSize = benchmarkTimes.Skip(i * runsPerSize).Take(runsPerSize).ToArray();

                allTimes.Add((sizes[i], timesForSize));
            }

            // Write the results to the text file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var result in allTimes)
                {
                    // Write the size followed by all the times
                    writer.Write($"{result.Size}");
                    foreach (var time in result.Times)
                    {
                        writer.Write($" {time}");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
