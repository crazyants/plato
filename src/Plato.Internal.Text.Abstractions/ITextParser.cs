namespace Plato.Internal.Text.Abstractions
{
    public interface ITextParser
    {
        string Text { get; }

        int Position { get; }

        int Remaining { get; }

        bool EndOfText { get; }

        void Reset();

        void Reset(string text);

        char Peek();

        char Peek(int ahead);

        string Extract(int start);

        string Extract(int start, int end);

        void MoveAhead();

        void MoveAhead(int ahead);

        void MoveTo(string s, bool ignoreCase = false);

        void MoveTo(char c);

        void MoveTo(char[] chars);

        void MovePast(char[] chars);

        void MoveToEndOfLine();

        void MovePastWhitespace();

    }

}
