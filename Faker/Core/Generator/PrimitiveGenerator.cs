using System;

namespace Core.Generator
{
    public class PrimitiveGenerator : IGenerator
    {
        private readonly Random _random = new();

        public bool CanGenerate(Type t)
        {
            return t.IsPrimitive;
        }

        public object Generate(Type t)
        {
            if (CanGenerate(t))
                return Convert.ChangeType(_random.Next(10, 10_000) * _random.NextDouble(), t);
            throw new ArgumentException($"Cannot create object of type: {t}");
        }
    }
}
