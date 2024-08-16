using System;
using Core.Generator;

namespace BoolGeneratorPlugin
{
    public class BoolGenerator : IGenerator
    {
        private readonly Random _random = new();

        public bool CanGenerate(Type t)
        {
            return t == typeof(bool);
        }

        public object Generate(Type t)
        {
            if (CanGenerate(t))
                return (_random.Next() & 1) == 0;
            throw new ArgumentException($"Cannot create object of type: {t}");
        }
    }
}