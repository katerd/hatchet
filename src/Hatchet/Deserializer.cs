using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Hatchet
{
    public class Deserializer
    {
        private int _index;
        private string _input;

        private char Chr { get { return _input[_index]; } }

        public object Parse(ref string input)
        {
            Debug.WriteLine(string.Format("Input '{0}'", input));

            _input = input;
            _index = 0;

            return ReadValue();
        }

        private object ReadValue()
        {
            ChompWhitespace();

            // list
            if (Chr == '[')
            {
                var list = ReadList();
                return list;
            }
                // object
            else if (Chr == '{')
            {
                var obj = ReadDefinitions();
                return obj;
            }
            // string
            else if (Chr == '"' || Chr == '\'')
            {
                var readString = ReadString();
                return readString;
            }
            // multi-line comment
            else if (PeekChars("/*"))
            {
                Debug.WriteLine("Open block comment");
            }
            // single line comment
            else if (PeekChars("//"))
            {
                Debug.WriteLine("Open line comment");
            }
            // text block
            else if (PeekChars("!["))
            {
                Debug.WriteLine("Open text block");
            }
            // parse as a string?
            else
            {
                Debug.WriteLine("I guess this is a string");
                return ReadName();
            }

            throw new Exception("No idea.");
        }

        private List<object> ReadList()
        {
            Debug.WriteLine("Open list");

            Expect('[');

            var list = new List<object>();

            while (true)
            {
                var r = ReadValue();

                list.Add(r);

                ChompWhitespace();

                if (PeekChars("]"))
                {
                    Expect(']');
                    return list;
                }
            }
        }

        private Dictionary<string, object> ReadDefinitions()
        {
            Debug.WriteLine("Open object");

            Expect('{');

            var obj = new Dictionary<string, object>();

            while (PeekName())
            {
                var name = ReadName();
                
                var value = ReadValue();
                obj[name] = value;

                Debug.WriteLine(string.Format("** `{0}` = `{1}`", name, value));

            }

            Debug.WriteLine("End of object definition");

            Expect('}');

            return obj;
        }

        private object ReadString()
        {
            Expect('\"', '\'');

            var quoteChar = _input[_index - 1];

            Debug.WriteLine("Reading string using quote char `{0}`", quoteChar);

            var stringBuilder = new StringBuilder();

            while (Chr != (quoteChar))
            {
                Debug.WriteLine(string.Format("Reading string char `{0}`", _input[_index]));

                // Handle escaped characters.
                if (PeekChars("\\"))
                {
                    Debug.WriteLine("Skipping character");
                    _index++;
                }


                Debug.WriteLine(string.Format("Writing string char `{0}`", Chr));
                stringBuilder.Append(Chr);
                _index++;

                
            }

            _index++; // Chomp the end quotation char.

            var result = stringBuilder.ToString();

            Debug.WriteLine("String = `{0}`", result); 

            return result;
        }

        private bool PeekChars(string chars)
        {
            for (var i = 0; i < chars.Length; i++)
            {
                if (_input[_index + i] != chars[i])
                    return false;
            }
            return true;
        }

        private void Expect(params char[] anyOfThese)
        {
            ChompWhitespace();
            if (anyOfThese.Any(c => Chr == c))
            {
                _index++;
                return;
            }
            throw new Exception(string.Format("Expected any of `{0}` but didn't find any", string.Join(",", anyOfThese)));
        }

        private void ChompWhitespace()
        {
            while (IsWhitespaceOrLineBreak(Chr))
            {
                _index++;
            }
        }

        private string ReadName()
        {
            ChompWhitespace();

            var startIndex = _index;

            while (_index < _input.Length)
            {
                var c = _input[_index];

                if (!IsValidNameCharacter(c))
                {
                    return _input.Substring(startIndex, _index - startIndex);
                }
                _index++;
            }
            throw new Exception("Fell off the edge of the file.");
        }

        private static bool IsWhitespaceOrLineBreak(char c)
        {
            return (c == ' ' || c == '\t' || c == '\n' || c == '\r');
        }

        private static bool IsWhitespace(char c)
        {
            return (c == ' ' || c == '\t');
        }

        private static bool IsValidNameCharacter(char c)
        {
            const string definitionChars = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM_";
            return definitionChars.IndexOf(c) >= 0;
        }

        private bool PeekName()
        {
            var i = _index;

            // step over whitespace
            while (i < _input.Length)
            {
                if (IsWhitespace(_input[i]))
                {
                    i++;
                }
                else
                {
                    break;
                }
            }

            while (i < _input.Length)
            {
                var c = _input[i];

                Debug.WriteLine("Peek name on char `{0}`", c);

                if (!IsValidNameCharacter(c))
                {
                    var result = i > _index;
                    Debug.WriteLine(result ? "Found name" : "Name is blank");
                    return result;
                }

                i++;
            }

            Debug.WriteLine("Did not find a name");
            return false;
        }
    }
}