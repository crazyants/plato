using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Plato.Layout.Views;

namespace Plato.Layout.Adaptors
{
    public interface IViewAdaptor
    {

        Task<IViewAdaptorResult> Configure();

    }

}
