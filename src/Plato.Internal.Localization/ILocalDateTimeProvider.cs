using System;
using System.Threading.Tasks;

namespace Plato.Internal.Localization
{

    public interface ILocalDateTimeProvider
    {
        Task<DateTimeOffset> GetLocalDateTimeAsync(LocalDateTimeOptions options);

    }
    
}
