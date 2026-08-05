using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace AsyncShowcase
{
    [MemoryDiagnoser]
    public class MultithreadingBenchmarks
    {
        private int[] _array = Array.Empty<int>();

        [GlobalSetup]
        public void Setup()
        {
            _array = new int[100];
            Random rnd = new Random(42);
            for (int i = 0; i < _array.Length; i++)
                _array[i] = rnd.Next(1, 20);
        }

        // ================= LEGACY (Thread + new Random) =================
        [Benchmark(Baseline = true)]
        public void Legacy_ProcessArray()
        {
            Thread t0 = new Thread(LegacyEvenIndices);
            Thread t1 = new Thread(LegacyOddSquares);
            t0.Start();
            t1.Start();
            t0.Join();
            t1.Join();
        }

        private void LegacyEvenIndices()
        {
            int sum = 0;
            for (int i = 0; i < _array.Length; i += 2)
            {
                sum += _array[i];
            }
        }

        private void LegacyOddSquares()
        {
            int sum = 0;
            for (int i = 1; i < _array.Length; i += 2)
            {
                sum += _array[i] * _array[i];
            }
        }

        // ================= MODERN (Task + Random.Shared) =================
        [Benchmark]
        public async Task Modern_ProcessArrayAsync()
        {
            var t0 = Task.Run(() => ModernEvenIndices());
            var t1 = Task.Run(() => ModernOddSquares());
            await Task.WhenAll(t0, t1);
        }

        private void ModernEvenIndices()
        {
            int sum = 0;
            for (int i = 0; i < _array.Length; i += 2)
            {
                sum += _array[i];
            }
        }

        private void ModernOddSquares()
        {
            int sum = 0;
            for (int i = 1; i < _array.Length; i += 2)
            {
                sum += _array[i] * _array[i];
            }
        }
    }
}
