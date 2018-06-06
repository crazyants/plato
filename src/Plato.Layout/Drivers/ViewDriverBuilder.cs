using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Layout.Drivers
{

    public interface IViewDriverBuilder
    {

        object Model { get; }

        void UpdateModel(object model);

        Task<ViewDriverBuilder> OnDisplay(Action<ViewDriverBuilder> context);
    }


    public class ViewDriverBuilder : IViewDriverBuilder
    {

        private object _model;

        public object Model => _model;

        public void UpdateModel(object model)
        {
            _model = model;
        }

        public Task<ViewDriverBuilder> OnDisplay(Action<ViewDriverBuilder> context)
        {
            throw new NotImplementedException();
        }
    }
}
