using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.SetUp;

namespace Plato.SetUp.Services
{
    public interface ISetUpService
    {
        Task<string> SetUpAsync(SetUpContext context);
    }
}
