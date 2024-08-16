using Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Core.Infos
{
    public class TypeInfo
    {
        public string Definition { get; }
        public IEnumerable<string> FieldDefinitions { get; }
        public IEnumerable<string> ConstructorDefinitions { get; }
        public IEnumerable<string> MethodDefinitions { get; }

        public TypeInfo(Type type)
        {
            BindingFlags flags =
                BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly;

            Definition = type.PrintDefinition();
            FieldDefinitions = type.GetFields(flags)
                .Select(field => field.PrintDefinition())
                .ToArray();
            ConstructorDefinitions = type.GetConstructors(flags)
                .Select(constructor => constructor.PrintDefinition())
                .ToArray();
            MethodDefinitions = type.GetMethods(flags)
                .Select(method => method.PrintDefinition())
                .ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("Definition:\n").Append(Definition).Append('\n');
            sb.Append("Fields:", FieldDefinitions);
            sb.Append("Constructors:", ConstructorDefinitions);
            sb.Append("Methods:", MethodDefinitions);
            return sb.ToString();
        }
    }
}
