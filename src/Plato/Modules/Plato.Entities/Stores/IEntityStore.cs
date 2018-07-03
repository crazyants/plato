using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityStore<TModel> : IStore<TModel> where TModel : class
    {

        event EntityStore.EntityStoreEventHandler Creating;
        event EntityStore.EntityStoreEventHandler Created;
        event EntityStore.EntityStoreEventHandler Updating;
        event EntityStore.EntityStoreEventHandler Updated;
        event EntityStore.EntityStoreEventHandler Deleting;
        event EntityStore.EntityStoreEventHandler Deleted;
        event EntityStore.ConfigureEntityEventHandler Configure;

    }


}
