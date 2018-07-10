using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.WebApi.Assets
{
    public class AssetProvider : IAssetProvider
    {

        private readonly IContextFacade _contextFacade;

        public AssetProvider(IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
        }

        public async Task<IEnumerable<AssetEnvironment>> GetAssetGroups()
        {

            var script = "$(function (win) { win.PlatoOptions = { url: '{url}', apiKey: '{apiKey}' } } (window));";
            script = script.Replace("{url}",  await _contextFacade.GetBaseUrl());
            script = script.Replace("{apiKey}", await GetApiKey());

            var htmlString = new HtmlString(script);

            IEnumerable<AssetEnvironment> result = new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        InlineContent = htmlString,
                        Type = AssetType.InlineJavaScript,
                        Section = AssetSection.Footer,
                        Priority = 99999,
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        InlineContent = htmlString,
                        Type = AssetType.InlineJavaScript,
                        Section = AssetSection.Footer,
                        Priority = 99999,
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        InlineContent = htmlString,
                        Type = AssetType.InlineJavaScript,
                        Section = AssetSection.Footer,
                        Priority = 99999,
                    }
                })

            };

            return result;

        }


        async Task<string> GetApiKey()
        {

            var settings = await _contextFacade.GetSiteSettingsAsync();

            if (settings == null)
            {
                return string.Empty;
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return settings.ApiKey;
            }

            if (user.Detail == null)
            {
                return settings.ApiKey;
            }

            if (String.IsNullOrWhiteSpace(user.Detail.ApiKey))
            {
                return settings.ApiKey;
            }
            
            return $"{settings.ApiKey}:{user.Detail.ApiKey}";


        }
    }
}
