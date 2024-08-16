using Core.Generator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Faker
{
    public class FakerImpl : IFaker
    {
        private static readonly FakerImpl _inctanse = new();
        public static FakerImpl DefaultFaker => _inctanse;

        private readonly IEnumerable<IGenerator> _generators;

        public FakerImpl(params IGenerator[] generators)
        {
            var generatorType = typeof(IGenerator);
            _generators = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(a => a.GetTypes())
                       .Where(t => t.IsClass && t.GetInterfaces().Contains(generatorType))
                       .Select(t =>
                       {
                           try
                           {
                               return (IGenerator)Activator.CreateInstance(t);
                           }
                           catch
                           {
                               return null;
                           }
                       })
                       .Concat(generators)
                       .Where(g => g is not null)
                       .ToArray();
        }

        public object Create(Type t)
        {
            foreach (var generator in _generators)
            {
                if (generator.CanGenerate(t))
                    return generator.Generate(t);
            }

            CyclicDependency.Validate(t);

            var c = t.GetConstructors();
            Array.Sort(c, (v1, v2) => v2.GetParameters().Length - v1.GetParameters().Length);
            foreach (var constructor in c)
            {
                try
                {
                    var args = constructor.GetParameters()
                        .Select(p => p.ParameterType)
                        .Select(Create);

                    object obj = constructor.Invoke(args.ToArray());
                    foreach (var field in t.GetFields())
                    {
                        try
                        {
                            if (Equals(field.GetValue(obj), GetDefaultValue(field.FieldType)))
                            {
                                field.SetValue(obj, Create(field.FieldType));
                            }
                        }
                        catch
                        {
                        }
                    }
                    return obj;
                }
                catch
                {
                }
            }
            throw new Exception($"Cannot create object of type: {t}");
        }
        public T Create<T>() => (T)Create(typeof(T));

        private static object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}
