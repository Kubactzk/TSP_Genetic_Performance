using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TSP_Genetic.TxtDataForPlotting
{
    internal class DataToTxt
    {
        public static void SaveBenchmarkData(string filePath, int[] sizes, List<double> benchmarkTimes)
        {
            // Calculate max, average, and min times for each size
            var results = new List<(int Size, double MaxTime, double AvgTime, double MinTime)>();

            // Assuming benchmarkTimes contains the times for each run of each size in sequence
            int runsPerSize = benchmarkTimes.Count / sizes.Length;

            for (int i = 0; i < sizes.Length; i++)
            {
                // Get the times for the current size
                var timesForSize = benchmarkTimes.Skip(i * runsPerSize).Take(runsPerSize).ToList();

                double max = timesForSize.Max();
                double avg = timesForSize.Average();
                double min = timesForSize.Min();

                results.Add((sizes[i], max, avg, min));
            }

            // Write the results to the text file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var result in results)
                {
                    writer.WriteLine($"{result.Size} {result.MaxTime} {result.AvgTime} {result.MinTime}");
                }
            }
        }
    }
}