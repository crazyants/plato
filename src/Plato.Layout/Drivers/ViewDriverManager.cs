using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Layout.Drivers
{

    public interface IViewDriverManager
    {
        Task<IEnumerable<IViewDriverResult>> ConfigureAsync();
    }

    public class ViewDriverManager : IViewDriverManager
    {
        private readonly IEnumerable<IViewDriverProvider> _viewDriverProviders;

        public ViewDriverManager(
            IEnumerable<IViewDriverProvider> viewDriverProviders)
        {
            _viewDriverProviders = viewDriverProviders;
        }

        public async Task<IEnumerable<IViewDriverResult>> ConfigureAsync()
        {

            var viewDriverResults = new List<IViewDriverResult>();

            foreach (var viewDriverProvider in _viewDriverProviders)
            {
                try
                {
                    var viewDriverResult = await viewDriverProvider.Configure();
                    viewDriverResults.Add(viewDriverResult);
                }
                catch (Exception e)
                {
                    //_logger.LogError(e, $"An exception occurred while building the menu: {name}");
                }
            }

            return viewDriverResults;

        }

    }
}
