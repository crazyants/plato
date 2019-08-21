using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.References.Services
{
    public class ReferencesParser : IReferencesParser
    {

        private readonly IEntityStore<Entity> _entityStore;
        private readonly IReferencesTokenizer _tokenizer;
        private readonly IContextFacade _contextFacade;
        
        public ReferencesParser(
            IEntityStore<Entity> entityStore,
            IReferencesTokenizer tokenizer,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _tokenizer = tokenizer;
        }

        public async Task<string> ParseAsync(string input)
        {

            // We need input to parse
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Build tokens
            var tokens = _tokenizer.Tokenize(input);

            // Ensure we have tokens to parse
            if (tokens == null)
            {
                return input;
            }

            // Prevent multiple enumeration
            var tokenList = tokens.ToList();

            // Get all referenced entities
            var entities = await GetEntitiesAsync(tokenList);
            if (entities != null)
            {

                var entityList = entities.ToList();
                var sb = new StringBuilder();

                IToken currentToken = null;
                for (var i = 0; i < input.Length; i++)
                {
                    foreach (var token in tokenList)
                    {
                        // Token start
                        if (i == token.Start)
                        {
                            currentToken = token;
                            var entity = entityList.FirstOrDefault(e => e.Id.ToString().Equals(token.Value, StringComparison.Ordinal));
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
                                sb.Append("<a href=\"")
                                    .Append(url)
                                    .Append("\" ")
                                    .Append("data-provide=\"popper\" ")
                                    .Append("data-popper-url=\"")
                                    .Append(popperUrl)
                                    .Append("\" class=\"reference-link\">")
                                    .Append(entity.Title);
                            }
                        }
                    }

                    if (currentToken == null)
                    {
                        sb.Append(input[i]);
                    }
             
             
                    foreach (var token in tokenList)
                    {
                        if (i == token.End)
                        {
                            var entity = entityList.FirstOrDefault(e => e.Id.ToString().Equals(token.Value, StringComparison.Ordinal));
                            if (entity != null)
                            {
                                sb.Append("</a>");
                                currentToken = null;
                            }
                        }
                        
                    }

                }

                return sb.ToString();
            }

            return input;

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
                return await GetEntitiesAsync(tokens);
            }

            return null;
        }

        // ------------

        async Task<IEnumerable<Entity>> GetEntitiesAsync(IEnumerable<IToken> tokens)
        {

            var entityIds = GetDistinctTokenValues(tokens);
            if (entityIds?.Length > 0)
            {
                return await GetEntitiesByIdsAsync(entityIds.ToArray());
            }

            return null;

        }

        async Task<IEnumerable<Entity>> GetEntitiesByIdsAsync(string[] entityIds)
        {
            
            var entities = await _entityStore.QueryAsync()
                .Select<EntityQueryParams>(q =>
                {
                    q.Id.IsIn(entityIds.ToIntArray());
                })
                .ToList();

            return entities?.Data;

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
