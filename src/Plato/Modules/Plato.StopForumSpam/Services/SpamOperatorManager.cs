using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Stores;

namespace Plato.StopForumSpam.Services
{
    
    public class SpamOperatorManager<TModel> : ISpamOperatorManager<TModel> where TModel : class
    {

        private readonly IStopForumSpamSettingsStore<StopForumSpamSettings> _stopForumSpamSettingsStore;
        private readonly IEnumerable<ISpamOperatorProvider<TModel>> _spamOperators;
        private readonly ILogger<SpamOperatorManager<TModel>> _logger;

        public SpamOperatorManager(
            IEnumerable<ISpamOperatorProvider<TModel>> spamOperators,
            IStopForumSpamSettingsStore<StopForumSpamSettings> stopForumSpamSettingsStore,
            ILogger<SpamOperatorManager<TModel>> logger)
        {
            _spamOperators = spamOperators;
            _stopForumSpamSettingsStore = stopForumSpamSettingsStore;
            _logger = logger;
        }

        public async Task<IEnumerable<ISpamOperatorResult<TModel>>> OperateAsync(ISpamOperation operation, TModel model)
        {
            
            // If the spam operation has been updated within the database ensure
            // we use the updated version from the database as opposed to the default
            var settings = await _stopForumSpamSettingsStore.GetAsync();
            var existingOperation = settings?.SpamOperations?.FirstOrDefault(o => o.Name.Equals(operation.Name));
            if (existingOperation != null)
            {
                operation = existingOperation;
            }

            // Create context for involved providers
            var context = new SpamOperatorContext<TModel>()
            {
                Model = model,
                Operation = operation
            };

            // Invoke provided operators
            var results = new List<ISpamOperatorResult<TModel>>();
            foreach (var spamOperator in _spamOperators)
            {
                try
                {
                    var result = await spamOperator.OperateAsync(context);
                    if (result != null)
                    {
                        results.Add(result);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An error occurred whilst invoking the ExecuteAsync method within a spam operation provider for type '{operation.Name}'.");
                }
            }

            // Log results
            foreach (var result in results)
            {
                if (result.Succeeded)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Spam Operation '{operation.Name}' Success!");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (_logger.IsEnabled(LogLevel.Critical))
                        {
                            _logger.LogCritical($"Spam Operation of type '{operation.Name}' failed with the following error: {error.Description}");
                        }
                    }
                }

            }

            return results;

        }

    }


}
