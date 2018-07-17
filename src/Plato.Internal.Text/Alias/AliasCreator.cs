using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text.Alias
{
    
    public class AliasCreator : IAliasCreator
    {

        private AliasOptions Options { get;  }
        
        #region "Constructor"

        public AliasCreator() : this(new AliasOptions())
        {
        }

        public AliasCreator(AliasOptions aliasOptions)
        {
            Options = aliasOptions ?? throw new ArgumentNullException(nameof(aliasOptions));
        }

        #endregion

        #region "Implementation"

        public string Create(string str)
        {
            if (Options.ForceLowerCase)
            {
                str = str.ToLower();
            }
                
            str = CleanWhiteSpace(str, Options.CollapseWhiteSpace);
            str = ApplyReplacements(str, Options.InitialReplacements);
            str = RemoveDiacritics(str);
            str = DeleteCharacters(str, Options.DeniedCharactersRegex);
            str = ApplyReplacements(str, Options.FinalReplacements);

            return str;
        }

        #endregion

        #region "Private Methods"

        string CleanWhiteSpace(string str, bool collapse)
        {
            return Regex.Replace(str, collapse ? @"\s+" : @"\s", " ");
        }

        string RemoveDiacritics(string str)
        {
            var stFormD = str.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var chr in stFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(chr);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(chr);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        string ApplyReplacements(string str, IDictionary<string, string> replacements)
        {
            var sb = new StringBuilder(str);

            foreach (var replacement in replacements)
            {
                sb.Replace(replacement.Key, replacement.Value);
            }

            return sb.ToString();

        }

        string DeleteCharacters(string str, string regex)
        {
            return Regex.Replace(str, regex, "");
        }

        #endregion

    }
}
