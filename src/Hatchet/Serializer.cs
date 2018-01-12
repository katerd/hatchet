using System;
using System.Collections.Generic;
using System.Text;

namespace Hatchet
{
    internal class Serializer
    {
        private const string LineEnding = "\n";
        private const int IndentCount = 2;
        
        private StringBuilder StringBuilder { get; }
        public SerializeOptions SerializeOptions { get; }

        private readonly List<object> _metObjects;
        
        public int IndentLevel;


        public Serializer(StringBuilder stringBuilder, SerializeOptions serializeOptions)
        {
            StringBuilder = stringBuilder;
            SerializeOptions = serializeOptions;
            _metObjects = new List<object>();
        }

        public void PushObjectRef(object obj)
        {
            var type = obj.GetType();

            if (obj is string)
                return;
            
            if (type.IsValueType)
                return;
            
            if (_metObjects.Contains(obj))
                throw new CircularReferenceException(obj);
            _metObjects.Add(obj);
        }

        public void PopObjectRef(object obj)
        {
            _metObjects.Remove(obj);
        }

        public void AppendFormat(string str, params object[] args)
        {
            StringBuilder.AppendFormat(str, args);
        }

        public void Indent()
        {
            IndentLevel++;
        }

        public void Deindent()
        {
            IndentLevel--;
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

        public void AppendCloseBlock()
        {
            Append(' ', IndentLevel * IndentCount);
            Append("}");
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
            }
            else if (ContainsNewLines(inputAsString))
            {
                AppendFormat("![{0}]!", inputAsString);
            }
            else if (ShouldWriteWithSingleQuotes(inputAsString))
            {
                AppendFormat("'{0}'", inputAsString);
            }
            else if (ShouldWriteWithDoubleQuotes(inputAsString))
            {
                AppendFormat("\"{0}\"", inputAsString);
            }
            else if (ContainsSpaces(inputAsString))
            {
                AppendFormat("\"{0}\"", inputAsString.Replace("\"", "\\\""));
            }
            else
            {
                Append(inputAsString);
            }
        }

        private static bool ShouldWriteWithDoubleQuotes(string inputAsString)
        {
            return ContainsSingleQuotes(inputAsString) && !ContainsDoubleQuotes(inputAsString);
        }

        private static bool ShouldWriteWithSingleQuotes(string inputAsString)
        {
            return ContainsDoubleQuotes(inputAsString) && !ContainsSingleQuotes(inputAsString);
        }

        private static bool ContainsNewLines(string inputAsString)
        {
            var containsNewLines = inputAsString.Contains("\r") || inputAsString.Contains("\n");
            return containsNewLines;
        }

        private static bool ContainsSingleQuotes(string inputAsString)
        {
            var containsSingleQuotes = inputAsString.Contains("'");
            return containsSingleQuotes;
        }

        private static bool ContainsDoubleQuotes(string inputAsString)
        {
            var containsDoubleQuotes = inputAsString.Contains("\"");
            return containsDoubleQuotes;
        }

        private static bool ContainsSpaces(string inputAsString)
        {
            var containsSpaces = inputAsString.Contains(" ");
            return containsSpaces;
        }
    }
}