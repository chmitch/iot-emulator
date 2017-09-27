using System;

namespace iot_emulator
{
    public class RandomDouble : IRandomGenerator
    {
        private static readonly Random _random = BuildRandomSource();

        public RandomDouble()
        {
        }

        public double GetRandomDouble()
        {
            lock (_random)
            {
                return _random.NextDouble();
            }
        }

        private static Random BuildRandomSource()
        {
            return new Random(Guid.NewGuid().GetHashCode());
        }
    }
}