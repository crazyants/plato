using System;
using System.Threading.Tasks;

namespace Plato.Internal.Localization.Abstractions
{

    public interface ILocalDateTimeProvider
    {
        Task<DateTimeOffset> GetLocalDateTimeAsync(LocalDateTimeOptions options);

    }
    
}
