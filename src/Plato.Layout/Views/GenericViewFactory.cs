using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Layout.Views
{

    public interface IViewResultFactory
    {
        Task<ViewResult> CreateAsync(string view, object model);

        dynamic New { get; }
    }


    public class ViewResultFactory  : IViewResultFactory
    {
        public Task<ViewResult> CreateAsync(string view, object model)
        {
            throw new NotImplementedException();
        }

        public dynamic New { get; }
    }
}
