using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.References.Services
{
  
    //public class ReferencesParser : IReferencesParser
    //{
        
    //    private const string SearchPattern = "#([^\\n|^\\s][0-9\\-_\\/]*)";

    //    public string ReplacePattern { get; set; }
    //        = "<a href=\"{url}\" data-popper-url=\"{popperUrl}\" data-provide=\"popper\" data-popper-css=\"w-500\"  data-entity-id=\"{entityId}\" class=\"reference-link\">#{entityId}</a>";

    //    private readonly IContextFacade _contextFacade;
    //    private readonly IEntityStore<Entity> _entityStore;
    
    //    public ReferencesParser(
    //        IEntityStore<Entity> entityStore, 
    //        IContextFacade contextFacade)
    //    {
    //        _entityStore = entityStore;
    //        _contextFacade = contextFacade;
    //    }
        
    //    #region "Implementation"

    //    public async Task<string> ParseAsync(string input)
    //    {
    //        var users = await GetEntitiesAsync(input);
    //        if (users != null)
    //        {
    //            input = await ConvertToUrlsAsync(input, users);
    //        }

    //        return input;

    //    }

    //    public async Task<IEnumerable<Entity>> GetEntitiesAsync(string input)
    //    {
    //        if (input == null)
    //        {
    //            throw new ArgumentNullException(nameof(input));
    //        }

    //        var entities = GetUniqueEntities(input);
    //        if (entities?.Length > 0)
    //        {
    //            return await GetUsersByUsernamesAsync(entities.ToArray());
    //        }

    //        return null;

    //    }

    //    #endregion

    //    #region "Private Methods"
        
    //    async Task<string> ConvertToUrlsAsync(string input, IEnumerable<IEntity> entities)
    //    {

    //        if (String.IsNullOrEmpty(input))
    //        {
    //            throw new ArgumentNullException(nameof(input));
    //        }

    //        if (entities == null)
    //        {
    //            throw new ArgumentNullException(nameof(entities));
    //        }

    //        var opts = RegexOptions.Multiline | RegexOptions.IgnoreCase;
    //        var regex = new Regex(SearchPattern, opts);
    //        if (regex.IsMatch(input))
    //        {
    //            var entityList = entities.ToList();
    //            var baseUrl = await _contextFacade.GetBaseUrlAsync();
    //            foreach (Match match in regex.Matches(input))
    //            {
    //                var entityId = match.Groups[1].Value;
    //                var ok = int.TryParse(entityId, out var id);
    //                if (ok)
    //                {
    //                    var entity = entityList.FirstOrDefault(u => u.Id  == id);
    //                    if (entity != null)
    //                    {

    //                        var url = _contextFacade.GetRouteUrl(new RouteValueDictionary()
    //                        {
    //                            ["area"] = entity.ModuleId,
    //                            ["controller"] = "Home",
    //                            ["action"] = "Display",
    //                            ["opts.id"] = entity.Id,
    //                            ["opts.alias"] = entity.Alias
    //                        });

    //                        var popperUrl = _contextFacade.GetRouteUrl(new RouteValueDictionary()
    //                        {
    //                            ["area"] = "Plato.Entities",
    //                            ["controller"] = "Home",
    //                            ["action"] = "GetEntity",
    //                            ["opts.id"] = entity.Id,
    //                            ["opts.alias"] = entity.Alias
    //                        });

    //                        // parse template
    //                        var sb = new StringBuilder(ReplacePattern);
    //                        sb.Replace("{url}", url);
    //                        sb.Replace("{popperUrl}", popperUrl);
    //                        sb.Replace("{entityId}", entity.Id.ToString());

    //                        // Replace match with template
    //                        input = input.Replace(match.Value, sb.ToString());
    //                    }
    //                }
                 
    //            }
    //        }

    //        return input;

    //    }
        
    //    async Task<IEnumerable<Entity>> GetUsersByUsernamesAsync(string[] entityIds)
    //    {
    //        var output = new List<Entity>();
    //        foreach (var entityId in entityIds)
    //        {
    //            var ok = int.TryParse(entityId, out var id);
    //            if (ok)
    //            {
    //                var user = await _entityStore.GetByIdAsync(id);
    //                if (user != null)
    //                {
    //                    output.Add(user);
    //                }
    //            }
    //        }

    //        return output;

    //    }

    //    string[] GetUniqueEntities(string input)
    //    {

    //        List<string> output = null;
    //        var regex = new Regex(SearchPattern, RegexOptions.IgnoreCase);
    //        if (regex.IsMatch(input))
    //        {
    //            output = new List<string>();
    //            foreach (Match match in regex.Matches(input))
    //            {
    //                var username = match.Groups[1].Value;
    //                if (!output.Contains(username))
    //                    output.Add(username);
    //            }
    //        }

    //        return output?.ToArray();

    //    }

    //    #endregion

    //}

}
