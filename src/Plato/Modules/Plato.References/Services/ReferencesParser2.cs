using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.References.Services
{
    public class ReferencesParser2 : IReferencesParser
    {

        private readonly IReferencesTokenizer _tokenizer;
        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Entity> _entityStore;

        public ReferencesParser2(
            IReferencesTokenizer tokenizer,
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore)
        {
            _tokenizer = tokenizer;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
        }

        public async Task<string> ParseAsync(string input)
        {
            var tokens = _tokenizer.Tokenize(input);

            if (tokens == null)
            {
                return input;
            }

            var tokenList = tokens.ToList();

            var entities = await GetEntitiesAsync(tokenList);

            var sb = new StringBuilder();
            if (entities != null)
            {

                var userList = entities.ToList();

                for (var i = 0; i < input.Length; i++)
                {

                    foreach (var token in tokenList)
                    {

                        // Token start
                        if (i == token.Start)
                        {
                            var entity = userList.FirstOrDefault(e => e.Id.ToString().Equals(token.Value, StringComparison.Ordinal));
                            if (entity != null)
                            {
                                var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                                {
                                    ["area"] = entity.ModuleId,
                                    ["controller"] = "Home",
                                    ["action"] = "Display",
                                    ["opts.id"] = entity.Id,
                                    ["opts.alias"] = entity.Alias
                                });

                                var popperUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
                                {
                                    ["area"] = "Plato.Entities",
                                    ["controller"] = "Home",
                                    ["action"] = "GetEntity",
                                    ["opts.id"] = entity.Id,
                                    ["opts.alias"] = entity.Alias
                                });
                                sb.Append("<a href=\"").Append(url).Append("\" ")
                                    .Append("data-provide=\"popper\" ")
                                    .Append("data-popper-url=\"").Append(popperUrl).Append("\" ")
                                    .Append("class=\"mention-link\">");
                            }
                        }
                    }

                    sb.Append(input[i]);

                    foreach (var token in tokenList)
                    {
                        if (i == token.End)
                        {
                            var entity = userList.FirstOrDefault(e => e.Id.ToString().Equals(token.Value, StringComparison.Ordinal));
                            if (entity != null)
                                sb.Append("</a>");
                        }
                    }

                }

            }

            return sb.ToString();
        }

        public async Task<IEnumerable<Entity>> GetEntitiesAsync(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var tokens = _tokenizer.Tokenize(input);
            if (tokens != null)
            {
                return await GetEntitiesAsync(tokens.ToList());
            }

            return null;
        }


        async Task<IEnumerable<Entity>> GetEntitiesAsync(IEnumerable<IToken> tokens)
        {

            var entityIds = GetDistinctTokenValues(tokens);
            if (entityIds?.Length > 0)
            {
                return await GetUsersByUsernamesAsync(entityIds.ToArray());
            }

            return null;

        }

        async Task<IEnumerable<Entity>> GetUsersByUsernamesAsync(string[] entityIds)
        {
            var output = new List<Entity>();
            foreach (var entityId in entityIds)
            {
                var ok = int.TryParse(entityId, out var id);
                if (ok)
                {
                    var user = await _entityStore.GetByIdAsync(id);
                    if (user != null)
                    {
                        output.Add(user);
                    }
                }
            }

            return output;

        }

        string[] GetDistinctTokenValues(IEnumerable<IToken> tokens)
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
