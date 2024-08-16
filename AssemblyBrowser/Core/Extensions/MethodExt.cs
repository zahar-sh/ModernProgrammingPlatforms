using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Core.Extensions
{
    public static class MethodExt
    {
        public static string PrintVisibility(this MethodBase method)
        {
            if (method.IsPublic)
                return "public";
            if (method.IsPrivate)
                return "private";
            if (method.IsFamily)
                return "protected";
            if (method.IsAssembly)
                return "internal";
            if (method.IsFamilyOrAssembly)
                return "protected internal";
            if (method.IsFamilyAndAssembly)
                return "private internal";
            return "private";
        }

        public static string PrintTypeOfDefinition(this MethodBase method)
        {
            if (method.IsStatic)
                return "static";
            if (method.IsAbstract)
                return "abstract";
            if (method.IsVirtual)
                return "virtual";
            if (method.IsFinal)
                return "final";
            return "";
        }

        public static string PrintDefinitionName(this MethodBase method)
        {
            if (method.IsGenericMethod)
            {
                Type[] genericArgs = method.GetGenericArguments();
                StringBuilder sb = new(method.Name);
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
                if (method.IsConstructor)
                    return method.ReflectedType.PrintDefinitionName();
                if (method is MethodInfo m)
                    return m.ReturnType.PrintDefinitionName() + " " + method.Name;
                return method.Name;
            }
        }

        public static string PrintParams(this MethodBase method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0)
                return "()";
            StringBuilder sb = new();
            sb.Append('(');
            foreach (var parameter in parameters)
            {
                sb.Append(parameter.ParameterType.PrintDefinitionName())
                    .Append(' ')
                    .Append(parameter.Name)
                    .Append(',')
                    .Append(' ');
            }
            sb.Length -= 2;
            sb.Append(')');
            return sb.ToString();
        }

        public static string PrintDefinition(this MethodBase method)
        {
            var strings = new string[] {
                method.PrintVisibility(),
                method.PrintTypeOfDefinition(),
                method.PrintDefinitionName() + method.PrintParams()
            }.Where(s => !string.IsNullOrEmpty(s));
            return string.Join(' ', strings);
        }
    }
}
