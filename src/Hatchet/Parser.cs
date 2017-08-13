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

        public object Parse(ref string input)
        {
            Debug.WriteLine($"Parser Input '{input}'");

            _input = input;
            _index = 0;

            return ReadValue();
        }

        private object ReadValue()
        {
            while (true)
            {
                if (_index >= _input.Length)
                    return null;

                ChompWhitespace();

                if (Peek(Tokens.ListOpen))
                {
                    return ReadList();
                }

                if (Peek(Tokens.ObjectOpen))
                {
                    return ReadDefinitions();
                }

                if (Peek(Tokens.SingleQuote) || Peek(Tokens.DoubleQuote))
                {
                    return ReadString();
                }

                if (Peek(Tokens.BlockCommentOpen))
                {
                    ReadBlockComment();
                    ChompWhitespace();
                    if (_index == _input.Length)
                        return null;
                    continue;
                }

                if (Peek(Tokens.LineComment))
                {
                    ReadLineComment();
                    ChompWhitespace();
                    if (_index == _input.Length)
                        return null;
                    continue;
                }

                if (Peek(Tokens.TextBlockOpen))
                {
                    return ReadTextBlock();
                }

                return ReadNakedValue();
            }
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
            Debug.WriteLine("Open line comment");

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
            Debug.WriteLine("Open block comment");

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

            var listStartIndex = _index;

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

                if (_index == _input.Length)
                {
                    throw new HatchetException($"List opened at byte {listStartIndex} is not closed");
                }
            }
        }

        private Dictionary<string, object> ReadDefinitions()
        {
            Debug.WriteLine("Open object");

            Expect(Tokens.ObjectOpen);

            var obj = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            while (true)
            {
                var nameIndex = _index;

                var name = ReadValue() as string;
                if (name == null)
                {
                    Expect(Tokens.ObjectClose);
                    return obj;
                }

                // todo: verify that the name contains no unacceptable characters.
                
                var value = ReadValue();

                obj[name] = value ?? throw new HatchetException($"Property `{name}` defined at byte {nameIndex + 2} is missing a value");

                Debug.WriteLine("Object property `{0}` = `{1}`", name, value);
            }
        }

        private object ReadString()
        {
            var stringStartIndex = _index;

            Expect(Tokens.SingleQuote, Tokens.DoubleQuote);

            var quoteChar = _input[_index - 1];

            Debug.WriteLine("Reading string using quote char `{0}`", quoteChar);

            var stringBuilder = new StringBuilder();

            while (_input[_index] != (quoteChar))
            {
                Debug.WriteLine("Reading string char `{0}`", _input[_index]);

                // Handle escaped characters.
                if (Peek(Tokens.Escape))
                {
                    Debug.WriteLine("Skipping character");
                    _index++;
                }

                Debug.WriteLine("Writing string char `{0}`", _input[_index]);
                stringBuilder.Append(_input[_index]);
                _index++;

                if (_index >= _input.Length)
                {
                    throw new HatchetException($"String starting at byte {stringStartIndex + 1} is not terminated");
                }
            }

            _index++; // Chomp the end quotation char.

            var result = stringBuilder.ToString();

            Debug.WriteLine($"String = `{result}`"); 

            return result;
        }

        private string ReadNakedValue()
        {
            ChompWhitespace();

            var startIndex = _index;

            while (_index < _input.Length)
            {
                var c = _input[_index];

                // forward slash without a (preceding asterisk or additional
                // preceding forward slash) is valid as part of a value string.
                // for instances where a comment directly precedes a value, eg:
                // 9001//This is over 9000
                // we look-ahead to determine inclusion of the forward slash in
                // the value.
                if (!IsValidValueCharacter(c) || Peek("//") || Peek( "/*"))
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
            if (_index >= _input.Length)
                return false;

            return _input[_index] == c;
        }

        private void Expect(params char[] anyOfThese)
        {
            ChompWhitespace();

            var expectIndex = _index + 1;

            if (_index >= _input.Length)
            {
                if (anyOfThese.Length == 1)
                {
                    throw new HatchetException($"Expected `{anyOfThese[0]}` at byte {expectIndex}");
                }
                throw new HatchetException($"Expected any of `{string.Join(",", anyOfThese)}` at byte {expectIndex}");
            }

            if (anyOfThese.Any(c => _input[_index] == c))
            {
                _index++;
                return;
            }

            throw new HatchetException($"Expected any of `{string.Join(",", anyOfThese)}` at byte {expectIndex}");
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

        private static bool IsValidValueCharacter(char c)
        {
            return "1234567890:\\/-+_.qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(c) >= 0;
        }
    }
}