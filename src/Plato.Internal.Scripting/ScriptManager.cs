using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.Internal.Scripting
{

    public class ScriptManager : IScriptManager
    {
        const string _key = "Script_";

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
            var key = _key + section;
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

        // Register a ScriptCapture on the HttpContext for a specific section
        public void SetScriptBlock(ScriptBlock block, ScriptSection section)
        {
            
            var key = _key + section;

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

            blocks.Add(block.Content, block.Attributes, block.Order, block.CanMerge);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Added script blocks to context with key {key}");
            }

        }

        #endregion

    }
}
