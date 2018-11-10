using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Mentions.Services
{

   
    public interface IMentionsParser
    {
        Task<string> ParseAsync(string input);
    }

    public class MentionsParser : IMentionsParser
    {

        // pattern to match
        private string _pattern = "(?<!.[\\t|\\s{4}|&|;].)#([0-9]+)";

        private readonly IPlatoUserStore<User> _platoUserStore;

        public MentionsParser(IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }


        #region "Implementation"

        public async Task<string> ParseAsync(string input)
        {

            var usernames = GetUniqueUsernames(input);
            if (usernames?.Length > 0)
            {
                var users = await GetUsersByUsernamesAsync(usernames.ToArray());
                if (users != null)
                {
                    var usersList = users.ToList();
                    if (usersList.Count > 0)
                    {
                        input = ConvertMentionstoUrls(input, usersList);
                    }
                }

            }

            return input;

        }

        #endregion

        #region "Private Methods"
        
        private string ConvertMentionstoUrls(
            string input,
            IEnumerable<IUser> users)
        {

            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }
            
            var userList = users.ToList();
            var regex = new Regex(_pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(input))
            {
                var baseUrl = "";
                var opts = RegexOptions.Multiline | RegexOptions.IgnoreCase;

                foreach (Match match in regex.Matches(input))
                {
                    var username = match.Groups[1].Value;

                        var user = userList.FirstOrDefault(u =>u.UserName.Equals(username));
                        if (user != null)
                        {
                            var text = $"<a href=\"{baseUrl + user.Id}/{user.Alias}\">{user.UserName}</a>";
                            input = Regex.Replace(input, _pattern, text, opts);
                        }
                  
                }

            }

            return input;

        }
        
        private async Task<IEnumerable<IUser>> GetUsersByUsernamesAsync(string[] usernames)
        {
            var users = new List<User>();
            foreach (var username in usernames)
            {
                var user = await _platoUserStore.GetByUserNameAsync(username);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;

        }

        string[] GetUniqueUsernames(string input)
        {

            List<string> output = null;
            var regex = new Regex(_pattern, RegexOptions.IgnoreCase);
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
