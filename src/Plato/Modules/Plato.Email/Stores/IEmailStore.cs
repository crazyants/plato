using Plato.Internal.Stores.Abstractions;

namespace Plato.Email.Stores
{

    public interface IEmailStore<TModel> : IStore2<TModel> where TModel : class
    {

    }

}
