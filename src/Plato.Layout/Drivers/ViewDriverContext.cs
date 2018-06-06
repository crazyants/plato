using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Layout.Drivers
{

    public interface IViewDriverContext
    {

        object Model { get; }

        void UpdateModel(object model);

        Task<ViewDriverContext> OnDisplay(Action<ViewDriverContext> context);
    }


    public class ViewDriverContext : IViewDriverContext
    {

        private object _model;

        public object Model => _model;

        public void UpdateModel(object model)
        {
            _model = model;
        }

        public Task<ViewDriverContext> OnDisplay(Action<ViewDriverContext> context)
        {
            throw new NotImplementedException();
        }
    }
}
