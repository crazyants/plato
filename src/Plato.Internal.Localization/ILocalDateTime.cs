using System;
using System.Threading.Tasks;

namespace Plato.Internal.Localization
{

    public interface ILocalDateTime
    {
        Task<DateTimeOffset> GetLocalDateTimeAsync(LocalDateTimeOptions options);

    }
    
}
