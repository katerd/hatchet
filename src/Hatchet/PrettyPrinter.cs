using System.Text;

namespace Hatchet
{
    internal class PrettyPrinter
    {
        private const string LineEnding = "\n";
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
        
        public void AppendFormat(string str, params object[] args)
        {
            StringBuilder.AppendFormat(str, args);
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
        
        public void AppendOpenBlock()
        {
            Append("{");
            Append(LineEnding);
        }
        
        public void AppendEnum(object input)
        {
            var strRepr = input.ToString();
            if (strRepr.IndexOf(',') > 0)
            {
                AppendFormat("[{0}]", strRepr);
            }
            else
            {
                Append(strRepr);
            }
        }
    }
}