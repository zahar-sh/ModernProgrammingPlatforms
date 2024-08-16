using Core.Generator;
using System;

namespace DateGeneratorPlugin
{
    public class DateGenerator : IGenerator
    {
        private readonly Random _random = new();

        public bool CanGenerate(Type t)
        {
            return t == typeof(DateTime);
        }

        public object Generate(Type t)
        {
            if (CanGenerate(t))
                return new DateTime(_random.Next(1, 9999), _random.Next(1, 12), _random.Next(1, 28));
            throw new ArgumentException($"Cannot create object of type: {t}");
        }
    }
}
