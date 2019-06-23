using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Mentions.Services
{

    public class MentionsParser : IMentionsParser
    {

        /*      
            @([^\n|^\s|^,|^\<]*.)(?=[\s|\n|,]|<\/[^a]>+)
           
            @ matches the character @ literally (case sensitive)
            1st Capturing Group ([^\n|^\s|^,|^\<]*.)
            Match a single character not present in the list below [^\n|^\s|^,|^\<]*
            * Quantifier — Matches between zero and unlimited times, as many times as possible, giving back as needed (greedy)
            \n matches a line-feed (newline) character (ASCII 10)
            |^ matches a single character in the list |^ (case sensitive)
            \s matches any whitespace character (equal to [\r\n\t\f\v ])
            |^,|^ matches a single character in the list |^, (case sensitive)
            \< matches the character < literally (case sensitive)
            . matches any character (except for line terminators)
            Positive Lookahead (?=[\s|\n|,]|<\/[^a]>+)
            Assert that the Regex below matches
            1st Alternative [\s|\n|,]
            Match a single character present in the list below [\s|\n|,]
            \s matches any whitespace character (equal to [\r\n\t\f\v ])
            | matches the character | literally (case sensitive)
            \n matches a line-feed (newline) character (ASCII 10)
            |, matches a single character in the list |, (case sensitive)
            2nd Alternative <\/[^a]>+
            < matches the character < literally (case sensitive)
            \/ matches the character / literally (case sensitive)
            Match a single character not present in the list below [^a]
            a matches the character a literally (case sensitive)
            >+ matches the character > literally (case sensitive)
            + Quantifier — Matches between one and unlimited times, as many times as possible, giving back as needed (greedy)

         */

        private const string SearchPattern = "@([^\\t|^\\r|^\\n|^\\s|^,|^\\<]*.)(?=[\\s|\\n|\\r|,]|<\\/[^a]>+)";

        public string ReplacePattern { get; set; }
            = "<a href=\"{url}\" data-popper-url=\"{popperUrl}\" data-provide=\"popper\" data-user-id=\"{userId}\" class=\"mention-link\">@{userName}</a>";

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;
    
        public MentionsParser(
            IPlatoUserStore<User> platoUserStore, 
            IContextFacade contextFacade)
        {
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
        }
        
        #region "Implementation"

        public async Task<string> ParseAsync(string input)
        {
            var users = await GetUsersAsync(input);
            if (users != null)
            {
                input = await ConvertToUrlsAsync(input, users);
            }

            return input;

        }

        public async Task<IEnumerable<User>> GetUsersAsync(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var usernames = GetUniqueUsernames(input);
            if (usernames?.Length > 0)
            {
                return await GetUsersByUsernamesAsync(usernames.ToArray());
            }

            return null;

        }

        #endregion

        #region "Private Methods"
        
        async Task<string> ConvertToUrlsAsync(string input, IEnumerable<IUser> users)
        {

            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }

            var opts = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            var regex = new Regex(SearchPattern, opts);
            if (regex.IsMatch(input))
            {
                var userList = users.ToList();
                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (Match match in regex.Matches(input))
                {

                    
                    var username = match.Groups[1].Value;
                    var user = userList.FirstOrDefault(u => u.UserName.Equals(username, StringComparison.Ordinal));
                    if (user != null)
                    {

                        var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Users",
                            ["controller"] = "Home",
                            ["action"] = "Display",
                            ["opts.id"] = user.Id,
                            ["opts.alias"] = user.Alias
                        });

                        var popperUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Users",
                            ["controller"] = "Home",
                            ["action"] = "GetUser",
                            ["opts.id"] = user.Id,
                            ["opts.alias"] = user.Alias
                        });
                        
                        // parse template
                        var sb = new StringBuilder(ReplacePattern);
                        sb.Replace("{url}", url);
                        sb.Replace("{popperUrl}", popperUrl);
                        sb.Replace("{userId}", user.Id.ToString());
                        sb.Replace("{userName}", user.UserName);
                        
                        // Replace match with template
                        input = input.Replace(match.Value, sb.ToString());
                    }
                }
            }

            return input;

        }
        
        async Task<IEnumerable<User>> GetUsersByUsernamesAsync(string[] usernames)
        {
            var users = new List<User>();
            foreach (var username in usernames)
            {
                if (!String.IsNullOrEmpty(username))
                {
                    var user = await _platoUserStore.GetByUserNameAsync(username);
                    if (user != null)
                    {
                        users.Add(user);
                    }
                }
            }

            return users;

        }

        string[] GetUniqueUsernames(string input)
        {

            List<string> output = null;
            var regex = new Regex(SearchPattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(input))
            {
                output = new List<string>();
                foreach (Match match in regex.Matches(input))
                {
                    var username = match.Groups[1].Value;
                    if (!output.Contains(username))
                        output.Add(username);
                }
            }

            return output?.ToArray();

        }

        #endregion

    }

}
