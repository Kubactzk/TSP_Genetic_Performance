using System;
using System.Collections.Generic;
using System.Linq;
using TSP_Genetic.Algorithms;

namespace TSP_Genetic.Logic
{
    internal class Genetic
    {
        private static readonly Random random = new Random();
        (double, double)[] ints;
        (long, long)[] longs;
        (short, short)[] shorts;

        public Genetic((int, int)[] ints, (long, long)[] longs, (short, short)[] shorts, (double, double)[] doubles)
        {
            this.ints = doubles;
            this.longs = longs;
            this.shorts = shorts;
        }

        public List<GeneticModel[]> solve(int population_size, double mutation_rate, int genererations)
        {
            GeneticModel[] population = new GeneticModel[population_size];
            List<GeneticModel[]> result = new List<GeneticModel[]>();

            // Create initial population
            for (int i = 0; i < population_size; i++)
            {
                population[i] = Generate_random_population();
            }
            result.Add(population);

            // Perform genetic algorithm iterations
            for (int i = 0; i < population_size - 1; i++)
            {
                population = ChooseSurivors(population);
                population = Crossover(population, mutation_rate); // Apply mutation rate during crossover
                result.Add(population);
            }

            return result;
        }

        // Generate initial population
        public GeneticModel Generate_random_population()
        {
            GeneticModel model = new GeneticModel();
            int[] connections = new int[ints.Length];
            double[] weights = new double[ints.Length];

            // Generate a shuffled array of indices
            List<int> indices = Enumerable.Range(0, ints.Length).ToList();
            indices = indices.OrderBy(x => random.Next()).ToList();

            // Connect each point to the next shuffled point
            for (int i = 0; i < ints.Length; i++)
            {
                int to = indices[i];
                if (to == i) // Prevent self-connection by swapping with the next element
                {
                    to = indices[(i + 1) % ints.Length];
                    indices[i] = indices[(i + 1) % ints.Length];
                    indices[(i + 1) % ints.Length] = i;
                }

                connections[i] = to;
                weights[i] = CalculateDistance(ints[i], ints[to]);
            }

            model.connections = connections;
            model.weights = weights;
            model.fitness = weights.Sum(); //fitness is the sum of distances.

            return model;
        }

        private double CalculateDistance((double x1, double y1) point1, (double x2, double y2) point2)
        {
            double dx = point1.x1 - point2.x2;
            double dy = point1.y1 - point2.y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Select survivors based on fitness
        public GeneticModel[] ChooseSurivors(GeneticModel[] old_generation)
        {
            // Sort by fitness (best fitness at the start)
            var survivors = old_generation.OrderBy(x => x.fitness).Take(old_generation.Length / 2).ToArray();
            return survivors;
        }

        // Perform crossover operation to create new population
        public GeneticModel[] Crossover(GeneticModel[] survivors, double mutation_rate)
        {
            int half = survivors.Length;
            GeneticModel[] children = new GeneticModel[half * 2];

            // Carry forward the second half of the population as children
            for (int i = half; i < children.Length; i++)
            {
                children[i] = survivors[i - half];
            }

            // Crossover between survivors
            for (int i = 0; i < half; i += 2)
            {
                if (i + 1 >= half)
                {
                    // If there is an odd individual left, clone it directly as a child
                    children[i] = Clone(survivors[i]);
                    break;
                }

                GeneticModel parent1 = survivors[i];
                GeneticModel parent2 = survivors[i + 1];
                int length = parent1.connections.Length;
                int midway = random.Next(1, length); // Randomize crossover point

                GeneticModel child1 = new GeneticModel
                {
                    connections = new int[length],
                    weights = new double[length],
                    fitness = 0,
                };

                GeneticModel child2 = new GeneticModel
                {
                    connections = new int[length],
                    weights = new double[length],
                    fitness = 0,
                };

                HashSet<int> usedInChild1 = new HashSet<int>();
                HashSet<int> usedInChild2 = new HashSet<int>();

                // Copy the first part from parent1 to child1 and from parent2 to child2
                for (int j = 0; j < midway; j++)
                {
                    child1.connections[j] = parent1.connections[j];
                    usedInChild1.Add(parent1.connections[j]);

                    child2.connections[j] = parent2.connections[j];
                    usedInChild2.Add(parent2.connections[j]);
                }

                // Copy the second part from the opposite parent, ensuring uniqueness
                for (int j = midway; j < length; j++)
                {
                    int candidate1 = parent2.connections[j];
                    int candidate2 = parent1.connections[j];

                    // Ensure unique values in child1
                    while (usedInChild1.Contains(candidate1))
                    {
                        candidate1 = random.Next(0, length); // Adjust range if necessary
                    }
                    child1.connections[j] = candidate1;
                    usedInChild1.Add(candidate1);

                    // Ensure unique values in child2
                    while (usedInChild2.Contains(candidate2))
                    {
                        candidate2 = random.Next(0, length); // Adjust range if necessary
                    }
                    child2.connections[j] = candidate2;
                    usedInChild2.Add(candidate2);
                }

                // Calculate weights for both children
                for (int j = 0; j < length - 1; j++)
                {
                    child1.weights[j] = CalculateDistance(ints[child1.connections[j]], ints[child1.connections[j + 1]]);
                    child2.weights[j] = CalculateDistance(ints[child2.connections[j]], ints[child2.connections[j + 1]]);
                }

                // Calculate fitness for both children
                child1.fitness = child1.weights.Sum();
                child2.fitness = child2.weights.Sum();

                children[i] = child1;
                children[i + 1] = child2;
            }

            // Apply mutation with a given mutation rate
            for (int i = 0; i < children.Length; i++)
            {
                if (random.NextDouble() < mutation_rate)
                {
                    Mutate(children[i]);
                }
            }

            return children;
        }

        // Mutation method: randomly swap two connections
        private void Mutate(GeneticModel individual)
        {
            int length = individual.connections.Length;
            int index1 = random.Next(length);
            int index2 = random.Next(length);

            // Swap the connections at index1 and index2
            int temp = individual.connections[index1];
            individual.connections[index1] = individual.connections[index2];
            individual.connections[index2] = temp;

            // Recalculate weights and fitness after mutation
            for (int j = 0; j < length - 1; j++)
            {
                individual.weights[j] = CalculateDistance(ints[individual.connections[j]], ints[individual.connections[j + 1]]);
            }

            individual.fitness = individual.weights.Sum();
        }

        private GeneticModel Clone(GeneticModel original)
        {
            return new GeneticModel
            {
                connections = (int[])original.connections.Clone(),
                weights = (double[])original.weights.Clone(),
                fitness = original.fitness
            };
        }
    }
}
