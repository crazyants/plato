using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.Internal.Scripting
{

    public class ScriptManager : IScriptManager
    {
        const string ScriptKey = "js_";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ScriptManager> _logger;

        public ScriptManager(
            ILogger<ScriptManager> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        #region "Implementation"

        public ScriptBlocks GetScriptBlocks(ScriptSection section)
        {
            var key = ScriptKey + section;
            var context = _httpContextAccessor.HttpContext;
         
            if (!context.Items.ContainsKey(key))
            {
                return null;
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Found script blocks on context with key {key}");
            }

            return (context.Items[key] as ScriptBlocks);
            
        }

        // Register a ScriptBlock on the HttpContext for a specific section
        public void RegisterScriptBlock(ScriptBlock block, ScriptSection section)
        {
            
            var key = ScriptKey + section;

            ScriptBlocks blocks = null;
            var context = _httpContextAccessor.HttpContext;
            if (context.Items.ContainsKey(key))
            {
                blocks = context.Items[key] as ScriptBlocks;
            }

            if (blocks == null)
            {
                blocks = new ScriptBlocks();
                context.Items.Add(key, blocks);
            }

            blocks.Add(block);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Added script blocks to context with key {key}");
            }

        }

        #endregion

    }
}
