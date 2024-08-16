using System;

namespace Core.Generator
{
    public interface IGenerator
    {
        bool CanGenerate(Type t);
        object Generate(Type t);
    }
}
