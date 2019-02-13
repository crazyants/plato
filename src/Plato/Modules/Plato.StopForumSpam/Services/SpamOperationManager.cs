using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Stores;

namespace Plato.StopForumSpam.Services
{
    
    public class SpamOperationManager<TModel> : ISpamOperationManager<TModel> where TModel : class
    {

        private readonly IStopForumSpamSettingsStore<StopForumSpamSettings> _stopForumSpamSettingsStore;
        private readonly IEnumerable<ISpamOperationProvider<TModel>> _spamOperationsProviders;
        private readonly ILogger<SpamOperationManager<TModel>> _logger;

        public SpamOperationManager(
            IEnumerable<ISpamOperationProvider<TModel>> spamOperationProviders,
            IStopForumSpamSettingsStore<StopForumSpamSettings> stopForumSpamSettingsStore,
            ILogger<SpamOperationManager<TModel>> logger)
        {
            _spamOperationsProviders = spamOperationProviders;
            _stopForumSpamSettingsStore = stopForumSpamSettingsStore;
            _logger = logger;
        }

        public async Task<IEnumerable<ICommandResult<TModel>>> ExecuteAsync(ISpamOperationType operation, TModel model)
        {
            
            // If the spam operation has been updated within the database ensure
            // we use the updated version from the database as opposed to the default
            var settings = await _stopForumSpamSettingsStore.GetAsync();
            var existingOperation = settings?.SpamOperations?.FirstOrDefault(o => o.Name.Equals(operation.Name));
            if (existingOperation != null)
            {
                operation = existingOperation;
            }

            // Create context for providers
            var context = new SpamOperationContext<TModel>()
            {
                Model = model,
                Operation = operation
            };

            // Invoke providers
            var results = new List<ICommandResult<TModel>>();
            foreach (var operationProvider in _spamOperationsProviders)
            {
                try
                {
                    var result = await operationProvider.ExecuteAsync(context);
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
