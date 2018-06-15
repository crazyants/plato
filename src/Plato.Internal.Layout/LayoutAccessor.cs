
using System.ComponentModel.Design.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Plato.Internal.Layout
{

    public interface ILayoutAccessor
    {
        Task<LayoutViewModel> GetLayoutAsync();

    }

    public class LayoutAccessor : ILayoutAccessor
    {
        

        private readonly IActionContextAccessor _actionContextAccessor;

        public LayoutAccessor(IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<LayoutViewModel> GetLayoutAsync()
        {

            var controller = _actionContextAccessor.ActionContext.ActionDescriptor.DisplayName;


            return null;


        }



    }

}
