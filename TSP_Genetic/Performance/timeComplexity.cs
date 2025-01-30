using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Genetic.Performance
{
    internal class timeComplexity
    {
        public static double timeUsage<T>(Func<T> matrixOperation)
        {
            double totalMilliseconds = 0;
            int iterations = 4;

            /*How JIT Compilation Works:
                1. Compilation at Runtime: When a method is called for the first time, the JIT compiler translates the IL code of that method into optimized machine code.
                1. Caching: After the method is compiled, the native code is cached, so subsequent calls to the method can execute directly without recompilation.*/

            // Warm-up to mitigate JIT overhead 
            matrixOperation.Invoke();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                matrixOperation.Invoke();
            }
            finally
            {
                stopwatch.Stop();

                totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            }


            //var avg = totalMilliseconds / iterations;
            return totalMilliseconds;
        }
    }
}
