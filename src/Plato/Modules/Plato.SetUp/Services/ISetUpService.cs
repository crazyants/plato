using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.SetUp.Services
{
    public interface ISetUpService
    {
        Task<string> SetupAsync(SetUpContext context);
    }
}
