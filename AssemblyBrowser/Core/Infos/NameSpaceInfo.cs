using Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Infos
{
    public class NamespaceInfo
    {
        public string Name { get; }
        public IEnumerable<TypeInfo> Types { get; }

        public NamespaceInfo(string name, IEnumerable<Type> types)
        {
            Name = name;
            Types = types.Select(type => new TypeInfo(type));
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("Name:").Append(Name).Append('\n');
            sb.Append("Defined types:", Types);
            return sb.ToString();
        }
    }
}
