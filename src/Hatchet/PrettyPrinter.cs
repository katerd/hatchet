using System;
using System.Text;

namespace Hatchet
{
    internal class PrettyPrinter
    {
        private StringBuilder StringBuilder { get; }

        public PrettyPrinter(StringBuilder stringBuilder)
        {
            StringBuilder = stringBuilder;
        }

        public void AppendFormat(string str, params object[] args)
        {
            StringBuilder.AppendFormat(str, args);
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

        public void AppendDateTime(object input)
        {
            var inputAsDateTime = (DateTime) input;
            AppendFormat("\"{0:O}\"", inputAsDateTime);
        }

        public void AppendString(string inputAsString)
        {
            if (string.Equals(inputAsString, ""))
            {
                Append("\"\"");
                return;
            }

            var containsSpaces = inputAsString.Contains(" ");
            var containsDoubleQuotes = inputAsString.Contains("\"");
            var containsSingleQuotes = inputAsString.Contains("'");
            var containsNewLines = inputAsString.Contains("\r") || inputAsString.Contains("\n");

            if (containsNewLines)
            {
                AppendFormat("![{0}]!", inputAsString);
                return;
            }

            if (containsDoubleQuotes && !containsSingleQuotes)
            {
                AppendFormat("'{0}'", inputAsString);
                return;
            }

            if (containsSingleQuotes && !containsDoubleQuotes)
            {
                AppendFormat("\"{0}\"", inputAsString);
                return;
            }

            if (containsSpaces)
            {
                AppendFormat("\"{0}\"", inputAsString.Replace("\"", "\\\""));
                return;
            }

            Append(inputAsString);
        }
    }
}