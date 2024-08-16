using System.Linq;
using System.Reflection;

namespace Core.Extensions
{
    public static class FieldExt
    {
        public static string PrintVisibility(this FieldInfo field)
        {
            if (field.IsPublic)
                return "public";
            if (field.IsPrivate)
                return "private";
            if (field.IsFamily)
                return "protected";
            if (field.IsAssembly)
                return "internal";
            if (field.IsFamilyOrAssembly)
                return "protected internal";
            if (field.IsFamilyAndAssembly)
                return "private internal";
            return "private";
        }

        public static string PrintTypeOfDefinition(this FieldInfo field)
        {
            return field.IsStatic ? "static" : "";
        }

        public static string PrintDefinition(this FieldInfo field)
        {
            var strings = new string[] {
                field.PrintVisibility(),
                field.PrintTypeOfDefinition(),
                field.FieldType.PrintDefinitionName(),
                field.Name
            }.Where(s => !string.IsNullOrEmpty(s));
            return string.Join(' ', strings);
        }
    }
}
