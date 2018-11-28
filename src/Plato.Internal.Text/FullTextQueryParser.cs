using System;
using System.Collections.Generic;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text
{

    /*    
        abc	= FORMSOF(INFLECTIONAL, abc)
        ~abc = FORMSOF(THESAURUS, abc)
        +abc = "abc"
        "abc" = "abc"
        -abc = NOT FORMSOF(INFLECTIONAL, abc)
        abc def = (FORMSOF(INFLECTIONAL, abc) AND FORMSOF(INFLECTIONAL, def))
        abc or def = (FORMSOF(INFLECTIONAL, abc) OR FORMSOF(INFLECTIONAL, def))
        <abc def> = (FORMSOF(INFLECTIONAL, abc) NEAR FORMSOF(INFLECTIONAL, def))
        abc and (def or ghi) = (FORMSOF(INFLECTIONAL, abc) AND (FORMSOF(INFLECTIONAL, def) OR FORMSOF(INFLECTIONAL, ghi)))
    */

    public class FullTextQueryParser : IFullTextQueryParser
    {

        // Characters not allowed in unquoted search terms
        const string Punctuation = "~\"`!@#$%^&*()-+=[]{}\\|;:,.<>?/";

        public HashSet<string> StopWords { get; set; }
        
        private readonly ITextParser _parser;

        public FullTextQueryParser(ITextParser parser)
        {
            _parser = parser;
            StopWords = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        }

        #region "Implementation"

        public string ToFullTextSearchQuery(string query)
        {
            var node = FixUpExpressionTree(ParseNode(query, ConjunctionTypes.And), true);
            return (node != null) ? node.ToString() : string.Empty;
        }

        #endregion

        #region "Private Methods"

        INode ParseNode(string query, ConjunctionTypes defaultConjunction)
        {
            var termForm = TermForms.Inflectional;
            var termExclude = false;
            var conjunction = defaultConjunction;
            var resetState = true;
            INode root = null;

            _parser.Reset(query);
            while (!_parser.EndOfText)
            {
                if (resetState)
                {
                    // Reset modifiers
                    termForm = TermForms.Inflectional;
                    termExclude = false;
                    conjunction = defaultConjunction;
                    resetState = false;
                }

                _parser.MovePastWhitespace();
                string term;
                if (!_parser.EndOfText &&
                    !Punctuation.Contains(_parser.Peek().ToString()))
                {
                    // Extract query term
                    var start = _parser.Position;
                    _parser.MoveAhead();
                    while (!_parser.EndOfText &&
                           !Punctuation.Contains(_parser.Peek().ToString()) &&
                           !Char.IsWhiteSpace(_parser.Peek()))
                        _parser.MoveAhead();

                    // Allow trailing wildcard
                    if (_parser.Peek() == '*')
                    {
                        _parser.MoveAhead();
                        termForm = TermForms.Literal;
                    }

                    // Interpret token
                    term = _parser.Extract(start, _parser.Position);

                    if (String.Compare(term, "AND", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        conjunction = ConjunctionTypes.And;
                    }
                    else if (String.Compare(term, "OR", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        conjunction = ConjunctionTypes.Or;
                    }
                    else if (String.Compare(term, "NEAR", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        conjunction = ConjunctionTypes.Near;
                    }
                    else if (String.Compare(term, "NOT", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        termExclude = true;
                    }
                    else
                    {
                        root = AddNode(root, term, termForm, termExclude, conjunction);
                        resetState = true;
                    }

                    continue;
                }
                else if (_parser.Peek() == '"')
                {
                    // Match next term exactly
                    termForm = TermForms.Literal;
                    // Extract quoted term
                    term = ExtractQuote(_parser);
                    root = AddNode(root, term.Trim(), termForm, termExclude, conjunction);
                    resetState = true;
                }
                else
                {
                    INode node;
                    if (_parser.Peek() == '(')
                    {
                        // Parse parentheses block
                        term = ExtractBlock(_parser, '(', ')');
                        node = ParseNode(term, defaultConjunction);
                        root = AddNode(root, node, conjunction, true);
                        resetState = true;
                    }
                    else if (_parser.Peek() == '<')
                    {
                        // Parse angle brackets block
                        term = ExtractBlock(_parser, '<', '>');
                        node = ParseNode(term, ConjunctionTypes.Near);
                        root = AddNode(root, node, conjunction);
                        resetState = true;
                    }
                    else if (_parser.Peek() == '-')
                    {
                        // Match when next term is not present
                        termExclude = true;
                    }
                    else if (_parser.Peek() == '+')
                    {
                        // Match next term exactly
                        termForm = TermForms.Literal;
                    }
                    else if (_parser.Peek() == '~')
                    {
                        // Match synonyms of next term
                        termForm = TermForms.Thesaurus;
                    }
                }

                // Advance to next character
                _parser.MoveAhead();
            }

            return root;
        }
        
        INode FixUpExpressionTree(INode node, bool isRoot = false)
        {

            /*
                While our expression tree may be properly constructed, it may represent a query that
                is not supported by SQL Server. This method traverses the expression tree and corrects
                problem expressions as described below.

                 NOT term1 AND term2         Subexpressions swapped.
                 NOT term1                   Expression discarded.
                 NOT term1 AND NOT term2     Expression discarded if node is grouped (parenthesized)
							                 or is the root node; otherwise, the parent node may
							                 contain another subexpression that will make this one
							                 valid.
                 term1 OR NOT term2          Expression discarded.
                 term1 NEAR NOT term2        NEAR conjunction changed to AND.*

                * This method converts all NEAR conjunctions to AND when either subexpression is not
                an InternalNode with the form TermForms.Literal.
            */

            // Test for empty expression tree
            if (node == null) return null;

            // Special handling for internal nodes
            if (node is InternalNode internalNode)
            {
                // Fix up child nodes
                internalNode.Child1 = FixUpExpressionTree(internalNode.Child1);
                internalNode.Child2 = FixUpExpressionTree(internalNode.Child2);

                // Correct subexpressions incompatible with conjunction type
                if (internalNode.Conjunction == ConjunctionTypes.Near)
                {
                    // If either subexpression is incompatible with NEAR conjunction then change to AND
                    if (IsInvalidWithNear(internalNode.Child1) || IsInvalidWithNear(internalNode.Child2))
                        internalNode.Conjunction = ConjunctionTypes.And;
                }
                else if (internalNode.Conjunction == ConjunctionTypes.Or)
                {
                    // Eliminate subexpressions not valid with OR conjunction
                    if (IsInvalidWithOr(internalNode.Child1))
                        internalNode.Child1 = null;
                    if (IsInvalidWithOr(internalNode.Child2))
                        internalNode.Child1 = null;
                }

                // Handle eliminated child expressions
                if (internalNode.Child1 == null && internalNode.Child2 == null)
                {
                    // Eliminate parent node if both child nodes were eliminated
                    return null;
                }
                else if (internalNode.Child1 == null)
                {
                    // Child1 eliminated so return only Child2
                    node = internalNode.Child2;
                }
                else if (internalNode.Child2 == null)
                {
                    // Child2 eliminated so return only Child1
                    node = internalNode.Child1;
                }
                else
                {
                    // Determine if entire expression is an exclude expression
                    internalNode.Exclude = (internalNode.Child1.Exclude && internalNode.Child2.Exclude);
                    // If only first child expression is an exclude expression,
                    // then simply swap child expressions
                    if (!internalNode.Exclude && internalNode.Child1.Exclude)
                    {
                        var temp = internalNode.Child1;
                        internalNode.Child1 = internalNode.Child2;
                        internalNode.Child2 = temp;
                    }
                }
            }

            // Eliminate expression group if it contains only exclude expressions
            return ((node.Grouped || isRoot) && node.Exclude) ? null : node;
        }

        bool IsInvalidWithNear(INode node)
        {
            // NEAR is only valid with TerminalNodes with form TermForms.Literal
            return !(node is TerminalNode) || ((TerminalNode) node).TermForm != TermForms.Literal;
        }

        bool IsInvalidWithOr(INode node)
        {
            // OR is only valid with non-null, non-excluded (NOT) subexpressions
            return node == null || node.Exclude == true;
        }

        INode AddNode(
            INode root, 
            string term,
            TermForms termForm,
            bool termExclude,
            ConjunctionTypes conjunction)
        {
            if (term.Length > 0 && !IsStopWord(term))
            {
                var node = new TerminalNode
                {
                    Term = term,
                    TermForm = termForm,
                    Exclude = termExclude
                };
                root = AddNode(root, node, conjunction);
            }

            return root;
        }

        INode AddNode(
            INode root, 
            INode node,
            ConjunctionTypes conjunction,
            bool group = false)
        {
            if (node != null)
            {
                node.Grouped = group;
                if (root != null)
                    root = new InternalNode
                    {
                        Child1 = root,
                        Child2 = node,
                        Conjunction = conjunction
                    };
                else
                    root = node;
            }

            return root;
        }

        string ExtractBlock(
            ITextParser parser,
            char openChar,
            char closeChar)
        {
            // Track delimiter depth
            var depth = 1;

            // Extract characters between delimiters
            parser.MoveAhead();
            var start = parser.Position;
            while (!parser.EndOfText)
            {
                if (parser.Peek() == openChar)
                {
                    // Increase block depth
                    depth++;
                }
                else if (parser.Peek() == closeChar)
                {
                    // Decrease block depth
                    depth--;
                    // Test for end of block
                    if (depth == 0)
                        break;
                }
                else if (parser.Peek() == '"')
                {
                    // Don't count delimiters within quoted text
                    ExtractQuote(parser);
                }

                // Move to next character
                parser.MoveAhead();
            }

            return parser.Extract(start, parser.Position);
        }

        string ExtractQuote(ITextParser parser)
        {
            // Extract contents of quote
            parser.MoveAhead();
            var start = parser.Position;
            while (!parser.EndOfText && parser.Peek() != '"')
            {
                parser.MoveAhead();
            }
            return parser.Extract(start, parser.Position);
        }

        bool IsStopWord(string word)
        {
            return StopWords.Contains(word);
        }

        #endregion

        #region "Helpers"

        protected enum TermForms
        {
            Inflectional,
            Thesaurus,
            Literal,
        }

        protected enum ConjunctionTypes
        {
            And,
            Or,
            Near,
        }

        protected interface INode
        {

            /// <summary>
            /// Indicates this term (or both child terms) should be excluded from
            /// the results
            /// </summary>
            bool Exclude { get; set; }

            /// <summary>
            /// Indicates this term is enclosed in parentheses
            /// </summary>
            bool Grouped { get; set; }

        }

        /// <summary>
        /// Terminal (leaf) expression node class.
        /// </summary>
        private class TerminalNode : INode
        {

            // Interface members
            public bool Exclude { get; set; }

            public bool Grouped { get; set; }

            // Class members
            public string Term { private get; set; }

            public TermForms TermForm { get; set; }

            // Convert node to string
            public override string ToString()
            {
                var fmt = String.Empty;
                if (TermForm == TermForms.Inflectional)
                    fmt = "{0}FORMSOF(INFLECTIONAL, {1})";
                else if (TermForm == TermForms.Thesaurus)
                    fmt = "{0}FORMSOF(THESAURUS, {1})";
                else if (TermForm == TermForms.Literal)
                    fmt = "{0}\"{1}\"";
                return String.Format(fmt,
                    Exclude ? "NOT " : String.Empty,
                    Term);
            }
        }

        /// <summary>
        /// Internal (non-leaf) expression node class
        /// </summary>
        private class InternalNode : INode
        {
            // Interface members
            public bool Exclude { get; set; }

            public bool Grouped { get; set; }

            public INode Child1 { get; set; }

            public INode Child2 { get; set; }

            public ConjunctionTypes Conjunction { get; set; }

            // Convert node to string
            public override string ToString()
            {
                return String.Format(Grouped ? "({0} {1} {2})" : "{0} {1} {2}",
                    Child1.ToString(),
                    Conjunction.ToString().ToUpper(),
                    Child2.ToString());
            }
        }

        #endregion

    }

}
