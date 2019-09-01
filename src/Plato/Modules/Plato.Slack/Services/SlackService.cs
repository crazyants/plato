using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Internal.Net.Abstractions;
using Plato.Slack.Models;

namespace Plato.Slack.Services
{
    
    public class SlackService : ISlackService
    {
        
        private readonly SlackOptions _slackOptions;
        private readonly IHttpClient _httpClient;

        private const string ByFormat = "json";

        public SlackService(IHttpClient httpClient,
            IOptions<SlackOptions> slackOptions)
        {
            _slackOptions = slackOptions.Value;
            _httpClient = httpClient;
        }
        
        public async Task<Response> PostAsync(string text)
        {

            // Validate

            if (string.IsNullOrEmpty(_slackOptions.WebHookUrl))
            {
                return new FailResponse(string.Empty, string.Empty)
                {
                    Error = "No Slack Webhoot URL has been configured. Ensure IOptions<SlackOptions> is registered."
                };
            }

            if (string.IsNullOrEmpty(text))
            {
                return new FailResponse(string.Empty, string.Empty)
                {
                    Error = "Some text to post is required!"
                };
            }
            
            // Prepare request
            var parameters = new Dictionary<string, string>
            {
                {"text", text}
            };

            // Attempt to post
            var result = await _httpClient.RequestAsync(
                HttpMethod.Post,
                new Uri(_slackOptions.WebHookUrl),
                parameters,
                "application/json");

            if (result.Succeeded)
            {
                // Build parsed response
                return Response.Parse(result.Response, ByFormat);
            }

            // Return failure as no response was received
            return new FailResponse(result.Response, ByFormat)
            {
                Error = result.Error
            };

        }

    }

}
