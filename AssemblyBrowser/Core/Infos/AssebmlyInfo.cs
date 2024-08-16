using Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Core.Infos
{
    public class AssemblyInfo
    {
        public string Name { get; }
        public IEnumerable<NamespaceInfo> NameSpaces { get; }

        public AssemblyInfo(Assembly assembly)
        {
            Name = assembly.GetName().ToString();
            NameSpaces = assembly.GetNamespaces()
                .Select(pair => new NamespaceInfo(pair.Key, pair.Value))
                .ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("Name:").Append(Name).Append('\n');
            sb.Append("Namespaces:", NameSpaces);
            return sb.ToString();
        }
    }
}
