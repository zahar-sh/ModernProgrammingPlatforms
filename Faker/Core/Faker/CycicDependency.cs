using System;
using System.Linq;
using System.Text;

namespace Core.Faker
{
    class Node : IDisposable
    {
        public Type Type { get; set; }
        public Node Parent { get; set; }

        public void Dispose()
        {
            Type = null;
            Parent = null;
        }
    }

    public static class CyclicDependency
    {
        private static string PrintTrace(Type type, Node parent)
        {
            var sb = new StringBuilder();
            sb.Append(type.Name).Append('-');
            for (Node node = parent; node != null; node = node.Parent)
            {
                sb.Append(node.Type?.Name).Append('-');
            }
            sb.Length--;
            return sb.ToString();
        }
        public static void Validate(Type type)
        {
            Validate(type, null);
        }
        private static void Validate(Type type, Node parent)
        {
            if (type.IsPrimitive)
                return;
            for (Node node = parent; node != null; node = node.Parent)
            {
                if (Equals(node.Type, type))
                {
                    throw new Exception($"Type {type.Name} contains cyclic dependency: {PrintTrace(type, parent)}");
                }
            }
            using var currentNode = new Node()
            {
                Type = type,
                Parent = parent
            };
            var types = type.GetConstructors()
                .SelectMany(c => c.GetParameters())
                .Select(p => p.ParameterType)
                .Concat(type.GetFields()
                    .Select(field => field.FieldType));
            foreach (var currentType in types)
            {
                Validate(currentType, currentNode);
            }
        }
    }
}
