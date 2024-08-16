using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Extensions
{
    public static class TextExt
    {
        private static readonly Regex regex = new("\\n+");
        public static string DeleteEmptyLines(this string s)
        {
            return regex.Replace(s, "\n");
        }

        public static StringBuilder Append(this StringBuilder sb, string name, IEnumerable enumerable)
        {
            sb.Append(name).Append('\n');

            int len = sb.Length;
            foreach (object o in enumerable)
            {
                sb.Append(o).Append('\n');
            }
            if (len == sb.Length)
            {
                sb.Length--;
                sb.Append('[').Append(']').Append('\n');
            }
            return sb;
        }
    }
}
