using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Plato.Internal.Modules.Abstractions;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{
    
    public class SpamOperationManager<TOperation> : ISpamOperationManager<TOperation> where TOperation : class, ISpamOperation
    {

        private IEnumerable<TOperation> _operations;

        private readonly IAuthorizationService _authorizationService;
        private readonly IEnumerable<ISpamOperationProvider<TOperation>> _providers;
        private readonly ILogger<SpamOperationManager<TOperation>> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public SpamOperationManager(
            IEnumerable<ISpamOperationProvider<TOperation>> providers,
            ILogger<SpamOperationManager<TOperation>> logger,
            ITypedModuleProvider typedModuleProvider,
            IAuthorizationService authorizationService)
        {
            _providers = providers;
            _typedModuleProvider = typedModuleProvider;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        public IEnumerable<TOperation> GetSpamOperations()
        {

            if (_operations == null)
            {
                var operations = new List<TOperation>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        operations.AddRange(provider.GetSpamOperations());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the spam operation provider. Please review your spam operation provider and try again. {e.Message}");
                        throw;
                    }
                }

                _operations = operations;
            }

            return _operations;

        }

        public async Task<IDictionary<string, IEnumerable<TOperation>>> GetCategorizedSpamOperationsAsync()
        {

            var output = new Dictionary<string, IEnumerable<TOperation>>();

            foreach (var provider in _providers)
            {

                var module = await _typedModuleProvider.GetModuleForDependency(provider.GetType());
                var name = module.Descriptor.Name;
                var operations = provider.GetSpamOperations();
                foreach (var operation in operations)
                {
                    var category = operation.Category;
                    var title = String.IsNullOrWhiteSpace(category) ?
                        name :
                        category;

                    if (output.ContainsKey(title))
                    {
                        output[title] = output[title].Concat(new[] { operation });
                    }
                    else
                    {
                        output.Add(title, new[] { operation });
                    }
                }
            }

            return output;
        }

    }


}
