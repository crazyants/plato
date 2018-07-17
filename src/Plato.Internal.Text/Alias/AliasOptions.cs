using System;
using System.Collections.Generic;

namespace Plato.Internal.Text.Alias
{
    public class AliasOptions
    {

        public IDictionary<string, string> InitialReplacements { get; set; }

        public IDictionary<string, string> FinalReplacements { get; set; }

        public bool ForceLowerCase { get; set; }

        public bool CollapseWhiteSpace { get; set; }

        public string DeniedCharactersRegex { get; set; }

        public AliasOptions()
        {
            InitialReplacements = new Dictionary<string, string>
            {
                {" ", "-"},
            };

            ForceLowerCase = true;
            CollapseWhiteSpace = true;
            DeniedCharactersRegex = @"[^a-zA-Z0-9\-\._]";

            FinalReplacements = new Dictionary<string, string>
            {
                {"--", "-"}
            };
        }

    }

}
