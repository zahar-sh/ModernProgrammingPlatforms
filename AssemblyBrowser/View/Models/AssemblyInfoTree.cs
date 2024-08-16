using Core.Infos;
using Models;
using System.Linq;

namespace View.Models
{
    public class AssemblyInfoTree : Tree
    {
        public AssemblyInfoTree(AssemblyInfo info) :
            base(info.Name,
                info.NameSpaces
                .Select(n => new NamespaceInfoTree(n))
                .ToArray())
        {
        }
    }
}
