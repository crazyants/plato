using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Layout.Drivers
{

    public interface IViewDriverProvider
    {
        Task<IViewDriverResult> Configure();
    }

}
