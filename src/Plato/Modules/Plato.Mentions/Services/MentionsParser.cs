using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Mentions.Services
{
    
    public class MentionsParser : IMentionsParser
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IMentionsTokenizer _tokenizer;
        private readonly IContextFacade _contextFacade;

        public MentionsParser(
            IMentionsTokenizer tokenizer,
            IPlatoUserStore<User> platoUserStore,
                IContextFacade contextFacade)
        {
            _tokenizer = tokenizer;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
        }

        public async Task<string> ParseAsync(string input)
        {
            var tokens = _tokenizer.Tokenize(input);

            if (tokens == null)
            {
                return input;
            }

            var users = await GetUsersAsync(tokens);

            var sb = new StringBuilder();
            if (users != null)
            {

                var userList = users.ToList();

                for (var i = 0; i < input.Length; i++)
                {
                    
                    foreach (var token in tokens)
                    {

                        // Token start
                        if (i == token.Start)
                        {

                            var user = userList.FirstOrDefault(u => u.UserName.Equals(token.Value, StringComparison.Ordinal));

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

                                sb.Append("<a href=\"").Append(url).Append("\" ")
                                    .Append("data-provide=\"popper\" ")
                                    .Append("data-popper-url=\"").Append(popperUrl).Append("\" ")
                                    .Append("class=\"mention-link\">");

                            }
                       
                        }

                    }

                    sb.Append(input[i]);

                    foreach (var token in tokens)
                    {

                        if (i == token.End)
                        {
                            var user = userList.FirstOrDefault(u => u.UserName.Equals(token.Value, StringComparison.Ordinal));
                            if (user != null)
                            {
                                sb.Append("</a>");
                            }
                        }
                    }

                }



            }

            return sb.ToString();
        }
        
        public async Task<IEnumerable<User>> GetUsersAsync(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var tokens = _tokenizer.Tokenize(input);
            if (tokens != null)
            {
                return await GetUsersAsync(tokens);
            }

            return null;

        }

        // -----------

        async Task<IEnumerable<User>> GetUsersAsync(IList<MentionToken> tokens)
        {
            
            var usernames = GetDistinctTokenValues(tokens);
            if (usernames?.Length > 0)
            {
                return await GetUsersByUsernamesAsync(usernames.ToArray());
            }
            
            return null;

        }

        async Task<IDictionary<MentionToken, User>> GetTokensWithUserAsync(IList<MentionToken> tokens)
        {

            var usernames = GetDistinctTokenValues(tokens);
            if (usernames?.Length > 0)
            {
                var tokenAndUser = new Dictionary<MentionToken, User>();

                var users = await GetUsersByUsernamesAsync(usernames.ToArray());
                var userList = users.ToList();

                foreach (var token in tokens)
                {
                    var user = userList.FirstOrDefault(u => u.UserName.Equals(token.Value, StringComparison.Ordinal));
                    tokenAndUser.Add(token, user);
                }

                return tokenAndUser;

            }




            return null;

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

        string[] GetDistinctTokenValues(IEnumerable<MentionToken> tokens)
        {
            var output = new List<string>();
            foreach (var token in tokens)
            {
                if (!output.Contains(token.Value))
                    output.Add(token.Value);
            }
            return output?.ToArray();
        }
        
    }

}
