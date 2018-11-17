using System.Collections.Generic;

namespace Plato.WebApi.Models
{
    public interface IPagedApiResults<TModel> where TModel : class
    {
        int Page { get; set; }

        int Size { get; set; }

        int Total { get; set; }

        int TotalPages { get; set; }

        IEnumerable<TModel> Data { get; set; }

    }
    
}
