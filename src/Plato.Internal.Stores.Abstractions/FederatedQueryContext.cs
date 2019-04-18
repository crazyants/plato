using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions
{
    public interface IFederatedQueryContext<TModel> where TModel : class
    {
        
        WhereString Keywords { get; set; }

        string Where { get; set; }

        IQuery<TModel> Query { get; set; }

    }

    public class FederatedQueryContext<TModel> : IFederatedQueryContext<TModel> where TModel : class
    {

  

        public IQuery<TModel> Query { get; set; }

        public WhereString Keywords { get; set; }

        public string Where { get; set; }


    }

}
