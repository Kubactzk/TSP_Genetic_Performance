using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Genetic.DataLogic
{
    internal class GenerateData
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
    }
}
