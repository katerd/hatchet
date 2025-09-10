using System;
using System.Text;

namespace Hatchet;

internal class PrettyPrinter(StringBuilder stringBuilder)
{
    private const string LineEnding = "\n";
    private const int IndentCount = 2;

    private StringBuilder StringBuilder { get; } = stringBuilder;

    public int IndentLevel;

    public void Indent()
    {
        IndentLevel++;
    }

    public void Unindent()
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

    private static bool ShouldWriteWithDoubleQuotes(string inputAsString) =>
        ContainsSingleQuotes(inputAsString) &&
        !ContainsDoubleQuotes(inputAsString);

    private static bool ShouldWriteWithSingleQuotes(string inputAsString) =>
        ContainsDoubleQuotes(inputAsString) && 
        !ContainsSingleQuotes(inputAsString);

    private static bool ContainsNewLines(string inputAsString) =>
        inputAsString.Contains("\r") || 
        inputAsString.Contains("\n");

    private static bool ContainsSingleQuotes(string inputAsString) => inputAsString.Contains("'");
    private static bool ContainsDoubleQuotes(string inputAsString) => inputAsString.Contains("\"");
    private static bool ContainsSpaces(string inputAsString) => inputAsString.Contains(" ");
}
