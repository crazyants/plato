using System;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text
{

    public class TextParser : ITextParser
    {

        private string _text;

        private int _pos;

        public string Text => _text;

        public int Position => _pos;

        public int Remaining => _text.Length - _pos;

        public static char NullChar = (char) 0;

        public TextParser()
        {
            Reset(null);
        }
        
        public void Reset()
        {
            _pos = 0;
        }

        public void Reset(string text)
        {
            _text = text ?? String.Empty;
            _pos = 0;
        }

        public bool EndOfText => (_pos >= _text.Length);

        public char Peek()
        {
            return Peek(0);
        }

        public char Peek(int ahead)
        {
            var pos = (_pos + ahead);
            return pos < _text.Length ? _text[pos] : NullChar;
        }

        public string Extract(int start)
        {
            return Extract(start, _text.Length);
        }
        
        public string Extract(int start, int end)
        {
            return _text.Substring(start, end - start);
        }

        public void MoveAhead()
        {
            MoveAhead(1);
        }
        
        public void MoveAhead(int ahead)
        {
            _pos = Math.Min(_pos + ahead, _text.Length);
        }

        public void MoveTo(string s, bool ignoreCase = false)
        {
            _pos = _text.IndexOf(s, _pos, ignoreCase ?
                StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            if (_pos < 0)
                _pos = _text.Length;
        }
        
        public void MoveTo(char c)
        {
            _pos = _text.IndexOf(c, _pos);
            if (_pos < 0)
                _pos = _text.Length;
        }

        public void MoveTo(char[] chars)
        {
            _pos = _text.IndexOfAny(chars, _pos);
            if (_pos < 0)
                _pos = _text.Length;
        }

        public void MovePast(char[] chars)
        {
            while (IsInArray(Peek(), chars))
                MoveAhead();
        }
        
        protected bool IsInArray(char c, char[] chars)
        {
            foreach (var ch in chars)
            {
                if (c == ch)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void MoveToEndOfLine()
        {
            var c = Peek();
            while (c != '\r' && c != '\n' && !EndOfText)
            {
                MoveAhead();
                c = Peek();
            }
        }

        public void MovePastWhitespace()
        {
            while (Char.IsWhiteSpace(Peek()))
            {
                MoveAhead();
            }
        }

    }

}
