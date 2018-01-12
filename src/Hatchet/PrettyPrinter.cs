using System.Text;

namespace Hatchet
{
    internal class PrettyPrinter
    {
        private const int IndentCount = 2;
        
        public StringBuilder StringBuilder { get; }
        public int IndentLevel;

        public PrettyPrinter(StringBuilder stringBuilder)
        {
            StringBuilder = stringBuilder;
        }

        public void Indent()
        {
            IndentLevel++;
        }

        public void Deindent()
        {
            IndentLevel--;
        }

        public void AppendCloseBlock()
        {
            Append(' ', IndentLevel * IndentCount);
            Append("}");
        }
        
        public void Append(string str)
        {
            StringBuilder.Append(str);
        }

        public void Append(char chr, int count)
        {
            StringBuilder.Append(chr, count);
        }

        public void Append(char chr)
        {
            StringBuilder.Append(chr);
        }

        public void Append(object obj)
        {
            StringBuilder.Append(obj);
        }
    }
}