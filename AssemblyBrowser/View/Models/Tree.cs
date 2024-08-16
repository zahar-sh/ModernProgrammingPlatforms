using System.Collections.Generic;
using System.Text;
using Core.Extensions;

namespace Models
{
    public class Tree
    {
        public string Text { get; }
        public IEnumerable<Tree> SubTrees { get; }

        public Tree(string text, IEnumerable<Tree> trees = null)
        {
            Text = text;
            SubTrees = trees;
        }

        public override string ToString()
        {
            return SubTrees == null ?
                Text :
                new StringBuilder()
                .Append(Text, SubTrees)
                .ToString();
        }
    }
}