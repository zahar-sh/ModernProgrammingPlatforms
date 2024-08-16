using System;
using Core.Faker;

namespace Core.Generator
{
    public class ArrayGenerator : IGenerator
    {
        private readonly Random rnd = new();

        public bool CanGenerate(Type t)
        {
            return t.IsArray;
        }

        public object Generate(Type t)
        {
            if (!CanGenerate(t))
                throw new ArgumentException($"Cannot create object of type: {t}");

            var length = rnd.Next(2, 10);
            var array = (Array)Activator.CreateInstance(t, length);

            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(FakerImpl.DefaultFaker.Create(t.GetElementType()), i);
            }

            return array;
        }
    }
}
