using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.Views;

namespace Plato.Layout.Drivers
{


    public class CompiledViewProvider : IViewProviderResult
    {
        private readonly IEnumerable<IGenericView> _views;

        public CompiledViewProvider(params IGenericView[] results)
        {
            _views = results;
        }

        public CompiledViewProvider(IEnumerable<IGenericView> results)
        {
            _views = results;
        }
        
        public IList<IGenericView> Views { get; set; }

        public async Task ApplyAsync(ProviderDisplayContext context)
        {
            foreach (var result in _views)
            {
                result.ApplyAsync(context);
            }
        }

        public async Task ApplyAsync(ProviderEditContext context)
        {
            foreach (var result in _views)
            {
                await result.ApplyAsync(context);
            }
        }

        public IEnumerable<IGenericView> GetResults()
        {
            return _views;
        }



    }
}
