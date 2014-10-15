using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Hatchet
{
    public class Parser
    {
        private int _index;
        private string _input;

        private char Chr
        {
            get
            {
                return _input[_index];
            }
        }

        public object Parse(ref string input)
        {
            Debug.WriteLine(string.Format("Input '{0}'", input));

            _input = input;
            _index = 0;

            return ReadValue();
        }

        private object ReadValue()
        {
            if (_index >= _input.Length)
                return null;

            ChompWhitespace();

            // list
            if (Chr == Tokens.ListOpen)
            {
                var list = ReadList();
                return list;
            }
            // object
            if (Chr == Tokens.ObjectOpen)
            {
                var obj = ReadDefinitions();
                return obj;
            }
            // string
            if (Chr == Tokens.SingleQuote || Chr == Tokens.DoubleQuote)
            {
                var readString = ReadString();
                return readString;
            }
            // multi-line comment
            if (Peek(Tokens.BlockCommentOpen))
            {
                Debug.WriteLine("Open block comment");

                ReadBlockComment();

                /* a second attempt at reading a value is needed 
                 * when the comment exists at the top of the input text. */
                ChompWhitespace();
                if (_index == _input.Length)
                    return null;

                return ReadValue();
            }
            // single line comment
            if (Peek(Tokens.LineComment))
            {
                Debug.WriteLine("Open line comment");

                ReadLineComment();

                /* a second attempt at reading a value is needed 
                 * when the comment exists at the top of the input text. */
                ChompWhitespace();
                if (_index == _input.Length)
                    return null;

                return ReadValue();
            }
            // text block
            if (Peek(Tokens.TextBlockOpen))
            {
                Debug.WriteLine("Open text block");
                return ReadTextBlock();
            }
            // parse as a string?
            else
            {
                Debug.WriteLine("I guess this is a string");
                return ReadNakedValue();
            }

            return null;
        }

        private string ReadTextBlock()
        {
            Expect(Tokens.TextBlockOpen);

            var sb = new StringBuilder();

            // read until we hit an asterisk with a forward slash.
            while (_index < _input.Length)
            {
                if (Peek(Tokens.TextBlockClose))
                {
                    Expect(Tokens.TextBlockClose);
                    return sb.ToString();
                }
                sb.Append(_input[_index]);
                _index++;
            }

            // todo: nicer exception
            throw new Exception("Unterminated text block.");
        }

        private void ReadLineComment()
        {
            Expect(Tokens.LineComment);

            while (_index < _input.Length)
            {
                Debug.WriteLine("Reading comment char `{0}` {1}", (byte)_input[_index], _input[_index]);

                if (_input[_index] == Tokens.Lf)
                {
                    _index += 1;
                    return;
                }
                if (_input[_index] == Tokens.Cr && _index < _input.Length - 2 && _input[_index+1] == Tokens.Lf)
                {
                    _index += 2;
                    return;
                }
                if (_input[_index] == Tokens.Cr)
                {
                    _index += 1;
                    return;
                }

                _index++;
            }
        }

        private void ReadBlockComment()
        {
            Expect(Tokens.BlockCommentOpen);

            // read until we hit an asterisk with a forward slash.
            while (_index < _input.Length)
            {
                if (Peek(Tokens.BlockCommentClose))
                {
                    Expect(Tokens.BlockCommentClose);
                    return;
                }
                _index++;
            }

            // todo: nicer exception
            throw new Exception("Unterminated block comment");

        }

        private List<object> ReadList()
        {
            Debug.WriteLine("Open list");

            Expect(Tokens.ListOpen);

            var list = new List<object>();

            while (true)
            {
                var value = ReadValue();

                if (value != null)
                {
                    list.Add(value);
                }

                ChompWhitespace();

                if (Peek(Tokens.ListClose))
                {
                    Expect(Tokens.ListClose);
                    return list;
                }
            }
        }

        private Dictionary<string, object> ReadDefinitions()
        {
            Debug.WriteLine("Open object");

            Expect(Tokens.ObjectOpen);

            var obj = new Dictionary<string, object>();

            while (true)
            {
                var name = ReadValue() as string;

                if (name == null)
                {
                    Expect(Tokens.ObjectClose);
                    return obj;
                }

                // todo: verify that the name contains no unacceptable characters.
                
                var value = ReadValue();
                obj[name] = value;

                Debug.WriteLine("Object property `{0}` = `{1}`", name, value);
            }
        }

        private object ReadString()
        {
            Expect(Tokens.SingleQuote, Tokens.DoubleQuote);

            var quoteChar = _input[_index - 1];

            Debug.WriteLine("Reading string using quote char `{0}`", quoteChar);

            var stringBuilder = new StringBuilder();

            while (Chr != (quoteChar))
            {
                Debug.WriteLine("Reading string char `{0}`", _input[_index]);

                // Handle escaped characters.
                if (Peek(Tokens.Escape))
                {
                    Debug.WriteLine("Skipping character");
                    _index++;
                }

                Debug.WriteLine("Writing string char `{0}`", Chr);
                stringBuilder.Append(Chr);
                _index++;
            }

            _index++; // Chomp the end quotation char.

            var result = stringBuilder.ToString();

            Debug.WriteLine(string.Format("String = `{0}`", result)); 

            return result;
        }

        private string ReadNakedValue()
        {
            ChompWhitespace();

            var startIndex = _index;

            while (_index < _input.Length)
            {
                var c = _input[_index];

                if (!IsValidValueCharacters(c))
                {
                    if (_index == startIndex)
                        return null;

                    return _input.Substring(startIndex, _index - startIndex);
                }
                _index++;
            }

            return _input.Substring(startIndex, _index - startIndex);
        }

        private bool Peek(string chars)
        {
            for (var i = 0; i < chars.Length; i++)
            {
                if (_input[_index + i] != chars[i])
                    return false;
            }
            return true;
        }

        private bool Peek(char c)
        {
            return _input[_index] == c;
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

        private void Expect(string thisString)
        {
            foreach (var c in thisString)
            {
                Expect(c);
            }
        }

        private void ChompWhitespace()
        {
            while (_index < _input.Length && IsWhitespaceOrLineBreak(_input[_index]))
            {
                _index++;
            }
        }

        private static bool IsWhitespaceOrLineBreak(char c)
        {
            return (c == ' ' || c == '\t' || c == '\n' || c == '\r');
        }

        private static bool IsValidValueCharacters(char c)
        {
            return "1234567890-.qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(c) >= 0;
        }
    }
}