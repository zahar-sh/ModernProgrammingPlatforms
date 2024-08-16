using System.Linq;
using Core.Infos;

namespace Models
{
    public class TypeInfoTree : Tree
    {
        public TypeInfoTree(TypeInfo info) :
            base(info.Definition,
                new Tree[] {
                    new Tree("Fields:",
                        info.FieldDefinitions
                        .Select(f => new Tree(f))
                        .ToArray()),
                    new Tree("Constructors:",
                        info.ConstructorDefinitions
                        .Select(c => new Tree(c))
                        .ToArray()),
                    new Tree("Methods:",
                        info.MethodDefinitions
                        .Select(m => new Tree(m))
                        .ToArray())
                })
        {
        }
    }
}
