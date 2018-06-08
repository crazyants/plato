using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.Views;

namespace Plato.Layout.Drivers
{


    public class ProviderDisplayContext
    {

    }
    public class ProviderEditContext
    {

    }


    public interface IViewProviderResult
    {

        IList<IGenericView> Views { get; set; }

        Task ApplyAsync(ProviderDisplayContext context);
        Task ApplyAsync(ProviderEditContext context);
    }

    public class ViewProviderResult : IViewProviderResult
    {
        public Task ApplyAsync(ProviderDisplayContext context)
        {
            throw new NotImplementedException();
        }

        private IList<IGenericView> _views;

        public IList<IGenericView> Views
        {
            get => _views ?? (_views = new List<IGenericView>());
            set => _views = value;
        }

        public Task ApplyAsync(ProviderEditContext context)
        {
            throw new NotImplementedException();
        }
    }
}
