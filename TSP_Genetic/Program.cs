using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.WebSockets;
using TSP_Genetic.Algorithms;
using TSP_Genetic.DataLogic;
using TSP_Genetic.Logic;
using TSP_Genetic.Performance;
using TSP_Genetic.TxtDataForPlotting;

namespace TSP_Genetic
{
    internal class Program
    {
        private static readonly Random Random = new Random();

        // Method to generate a list of tuples with random values
        public static List<(T, T)> GenerateRandomTuples<T>(int length, Func<T> generator)
        {
            var list = new List<(T, T)>();

            for (int i = 0; i < length; i++)
            {
                list.Add((generator(), generator()));
            }

            return list;
        }

        public static void Main(string[] args)
        {
            int[] sizes = { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000};
            int[] populationSizes = { 100, 200, 300, 400, 500, 600, 700, 800, 1000, 1000 };
            int[] generations = { 100, 200, 300, 300, 400, 500, 500, 500, 900, 1000 };
            int numberOfRuns = 4;

            timeComplexity ts = new timeComplexity();

            // Containers for average fitness values
            float[] averageMaxFitness = new float[sizes.Length];
            float[] averageMinFitness = new float[sizes.Length];
            GeneticModel x = new GeneticModel();

            List<double> intBenchmark = new List<double>();
            List<double> intBenchmarkParallel = new List<double>();

            for (int run = 0; run < numberOfRuns; run++)
            {
                Console.WriteLine($"{run + 1} of {numberOfRuns}");
                // Containers for current run
                List<(int, int)[]> ints = new();
                List<(long, long)[]> longs = new();
                List<(short, short)[]> shorts = new();
                List<(double, double)[]> doubles = new();

                // Generate data for this run
                for (int i = 0; i < sizes.Length; i++)
                {
                    var randomIntTuples = GenerateRandomTuples(sizes[i], () => Random.Next(0, 100)).ToArray();
                    var randomLongTuples = GenerateRandomTuples(sizes[i], () => (long)Random.Next(0, 100)).ToArray();
                    var randomShortTuples = GenerateRandomTuples(sizes[i], () => (short)Random.Next(0, 100)).ToArray();
                    var randomDoubleTuples = GenerateRandomTuples(sizes[i], () => Random.NextDouble()).ToArray();
                    ints.Add(randomIntTuples);
                    longs.Add(randomLongTuples);
                    shorts.Add(randomShortTuples);
                    doubles.Add(randomDoubleTuples);
                }

                // Perform algorithm
                for (int i = 0; i < sizes.Length; i++)
                {
                    Genetic genetic = new Genetic(ints.ElementAt(i), longs.ElementAt(i), shorts.ElementAt(i), doubles.ElementAt(i));
                    GeneticParallel geneticParallel = new GeneticParallel(ints.ElementAt(i), longs.ElementAt(i), shorts.ElementAt(i), doubles.ElementAt(i));
                    intBenchmark.Add(timeComplexity.timeUsage(() => genetic.solve(populationSizes[i], 0.05, generations[i])));
                    intBenchmarkParallel.Add(timeComplexity.timeUsage(() => geneticParallel.Solve(populationSizes[i], 0.05, generations[i])));
                }
            }

            // Save benchmark data to text file
            string filePath = "D:\\Studia\\ISA-magister\\sem_2\\Obliczenia_wys_wydajnosci\\Projekt\\temat2\\pythonPlotting\\lined\\double\\benchmark_data.txt";
            string filePathParallel = "D:\\Studia\\ISA-magister\\sem_2\\Obliczenia_wys_wydajnosci\\Projekt\\temat2\\pythonPlotting\\lined\\double\\benchmark_data_parallel_3_cores.txt";
            //DataToTxt.SaveBenchmarkData(filePath, sizes, intBenchmark);
            DataToTxt.SaveBenchmarkData(filePathParallel, sizes, intBenchmarkParallel);
            /*histToTxt.SaveBenchmarkData(filePath, sizes, intBenchmark);
            histToTxt.SaveBenchmarkData(filePathParallel, sizes, intBenchmarkParallel);*/

            Console.WriteLine($"Benchmark data saved to {filePath}");
           // Console.WriteLine($"Benchmark data saved to {intBenchmark.Count}");
        }
    }
}
