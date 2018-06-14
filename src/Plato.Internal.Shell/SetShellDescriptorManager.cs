using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Shell
{
    public class SetShellDescriptorManager : IShellDescriptorStore

    {
        private readonly IEnumerable<ShellFeature> _shellFeatures;
        private IShellDescriptor _shellDescriptor;



        public SetShellDescriptorManager(IEnumerable<ShellFeature> shellFeatures)
        {
            _shellFeatures = shellFeatures;
        }


        public Task<IShellDescriptor> GetAsync()
        {
            if (_shellDescriptor == null)
            {
                _shellDescriptor = new ShellDescriptor
                {
                    Modules = _shellFeatures.ToList()
                };
            }

            return Task.FromResult(_shellDescriptor);
        }

        public Task<IShellDescriptor> SaveAsync(IShellDescriptor model)
        {
            return Task.FromResult(default(IShellDescriptor));
        }

        public Task<bool> DeleteAsync()
        {
            return Task.FromResult(default(bool));
        }
    }
}
