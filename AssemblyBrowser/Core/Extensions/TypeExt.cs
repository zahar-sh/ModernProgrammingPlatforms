using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Extensions
{
    public static class TypeExt
    {
        public static string PrintVisibility(this Type type)
        {
            if (type.IsPublic || type.IsNestedPublic)
                return "public";
            if (type.IsNestedPrivate)
                return "private";
            if (type.IsNestedFamily)
                return "protected";
            if (type.IsNestedAssembly)
                return "internal";
            if (type.IsNestedFamORAssem)
                return "protected internal";
            if (type.IsNestedFamANDAssem)
                return "private internal";
            return "private";
        }
        public static string PrintTypeOfClassExtension(this Type type)
        {
            if (type.IsAbstract)
                return type.IsSealed ? "static" : "abstract";
            if (type.IsSealed)
                return "sealed";
            return "";
        }
        public static string PrintSemantics(this Type type)
        {

            if (type.IsClass)
                return "class";
            if (type.IsEnum)
                return "enum";
            if (type.IsInterface)
                return "interface";
            if (type.IsValueType && !type.IsPrimitive)
                return "struct";
            return "";
        }

        public static string PrintDefinitionName(this Type type)
        {
            if (type.IsGenericType)
            {
                Type[] genericArgs = type.GetGenericArguments();
                StringBuilder sb = new(type.Name);
                sb.Length--;
                sb.Length -= genericArgs.Length.ToString().Length;
                sb.Append('<');
                if (genericArgs.Length != 0)
                {
                    foreach (Type t in genericArgs)
                    {
                        sb.Append(t.PrintDefinitionName())
                            .Append(',')
                            .Append(' ');
                    }
                    sb.Length -= 2;
                }
                sb.Append('>');
                return sb.ToString();
            }
            else
            {
                return type.Name;
            }
        }

        public static string PrintDefinition(this Type type)
        {
            var strings = new string[] {
                type.PrintVisibility(),
                type.PrintTypeOfClassExtension(),
                type.PrintSemantics(),
                type.PrintDefinitionName()
            }.Where(s => !string.IsNullOrEmpty(s));

            string declaration = string.Join(' ', strings);
            List<string> list = new();
            list.Add(type.BaseType?.PrintDefinitionName());
            list.AddRange(type.GetInterfaces().Select(i => i.PrintDefinitionName()));
            list.RemoveAll(s => string.IsNullOrEmpty(s));
            if (list.Count != 0)
            {
                declaration += ": " + string.Join(", ", list);
            }
            return declaration;
        }
    }
}
