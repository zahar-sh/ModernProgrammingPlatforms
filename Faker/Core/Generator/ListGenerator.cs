using Core.Faker;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Generator
{
    public class ListGenerator : IGenerator
    {
        private readonly Random _random = new();

        public bool CanGenerate(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }

        public object Generate(Type t)
        {
            if (!CanGenerate(t))
                throw new ArgumentException($"Cannot create object of type: {t}");

            var length = _random.Next(5, 20);
            var list = (IList)Activator.CreateInstance(t);
            for (int i = 0; i < length; i++)
            {
                list.Add(FakerImpl.DefaultFaker.Create(t.GetGenericArguments()[0]));
            }

            return list;
        }
    }
}
