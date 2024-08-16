using Core.Infos;
using System.Linq;

namespace Models
{
    public class NamespaceInfoTree : Tree
    {
        public NamespaceInfoTree(NamespaceInfo info) :
            base(info.Name,
                info.Types
                .Select(t => new TypeInfoTree(t))
                .ToArray())
        {
        }
    }
}
