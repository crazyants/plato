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

        private AliasOptions AliasOptions { get; set; }
        
        #region "Constructor"

        public AliasCreator() : this(new AliasOptions())
        {
        }

        public AliasCreator(AliasOptions aliasOptions)
        {
            if (aliasOptions != null)
                AliasOptions = aliasOptions;
            else
                throw new ArgumentNullException("aliasOptions", "can't be null use default config or empty construct.");
        }

        #endregion

        #region "Implementation"

        public string Create(string str)
        {
            if (AliasOptions.ForceLowerCase)
                str = str.ToLower();

            str = CleanWhiteSpace(str, AliasOptions.CollapseWhiteSpace);
            str = ApplyReplacements(str, AliasOptions.InitialReplacements);
            str = RemoveDiacritics(str);
            str = DeleteCharacters(str, AliasOptions.DeniedCharactersRegex);
            str = ApplyReplacements(str, AliasOptions.FinalReplacements);

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

            for (var ich = 0; ich < stFormD.Length; ich++)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
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
