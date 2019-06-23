using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Text.Abstractions;

namespace Plato.Mentions.Services
{

    public interface IMentionTokenizer
    {
        IList<MentionToken> Tokenize(string input);

        Task<string> ParseAsync(string input);

    }

    public class MentionTokenizer : IMentionTokenizer
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;

        // Denotes the end of a @mention
        private readonly IList<char> _terminators = new List<char>()
        {
            ',',
            ' ',
            '\r',
            '\n'
        };

        public MentionTokenizer(
            IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }

        public IList<MentionToken> Tokenize(string input)
        {

            var position = 0;
            List<MentionToken> output = null;
            StringBuilder sb = null;

            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];

                if (c == '@')
                {
                    position = i;
                    sb = new StringBuilder();
                }

                if (sb != null)
                {
                    if (c != '@' && !_terminators.Contains(c))
                    {
                        sb.Append(c);
                    }

                    if (_terminators.Contains(c))
                    {
                        if (output == null)
                        {
                            output = new List<MentionToken>();
                        }
                        output.Add(new MentionToken()
                        {
                            Start = position,
                            End = position + sb.ToString().Length,
                            Value = sb.ToString()
                        });
                        position = 0;
                        sb = null;
                    }
                }

            }

            return output;

        }


        public async Task<string> ParseAsync(string input)
        {

            var tokens = Tokenize(input);

            if (tokens == null)
            {
                return input;
            }
            
            var users = await GetUsersAsync(input);
            if (users != null)
            {

            
                var sb = new StringBuilder();
                    for (var i = 0; i < input.Length; i++)
                    {
                        var c = input[i];

                        foreach (var token in tokens)
                        {

                            if (i == token.Start)
                            {
                                sb.Append("<a href=\"#\">");
                            }
                        }

                        sb.Append(c);

                        foreach (var token in tokens)
                        {

                            if (i == token.End)
                            {
                                sb.Append("</a>");
                            }
                        }

                }
             
           

            }

            return input;

        }

        public async Task<IEnumerable<User>> GetUsersAsync(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var tokens = Tokenize(input);
            var usernames = GetUniqueUsernames(tokens);
            if (usernames?.Length > 0)
            {
                return await GetUsersByUsernamesAsync(usernames.ToArray());
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

        string[] GetUniqueUsernames(IEnumerable<MentionToken> tokens)
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

    public class MentionToken
    {

        public int Start { get; set; }

        public int End { get; set; }

        public string Value { get; set; }

    }

}
