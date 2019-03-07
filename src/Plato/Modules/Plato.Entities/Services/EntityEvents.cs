using Plato.Entities.Models;

namespace Plato.Entities.Services
{
    public class EntityEvents<TModel> where TModel : class
    {

        public event Handler Creating;
        public event Handler Created;
        public event Handler Updating;
        public event Handler Updated;
        public event Handler Deleting;
        public event Handler Deleted;

        public delegate void Handler(object sender, EntityEventArgs<TModel> e);

        protected virtual void OnCreating(EntityEventArgs<TModel> e)
        {
            Creating?.Invoke(this, e);
        }

        protected virtual void OnCreated(EntityEventArgs<TModel> e)
        {
            Created?.Invoke(this, e);
        }

        protected virtual void OnUpdating(EntityEventArgs<TModel> e)
        {
            Updating?.Invoke(this, e);
        }

        protected virtual void OnUpdated(EntityEventArgs<TModel> e)
        {
            Updated?.Invoke(this, e);
        }

        protected virtual void OnDeleting(EntityEventArgs<TModel> e)
        {
            Deleting?.Invoke(this, e);
        }

        protected virtual void OnDeleted(EntityEventArgs<TModel> e)
        {
            Deleted?.Invoke(this, e);
        }
    }

    public class EntityReplyEvents<TModel> where TModel : class
    {

        public event Handler Creating;
        public event Handler Created;
        public event Handler Updating;
        public event Handler Updated;
        public event Handler Deleting;
        public event Handler Deleted;

        public delegate void Handler(object sender, EntityReplyEventArgs<TModel> e);

        public virtual void OnCreating(EntityReplyEventArgs<TModel> e)
        {
            Creating?.Invoke(this, e);
        }

        public virtual void OnCreated(EntityReplyEventArgs<TModel> e)
        {
            Created?.Invoke(this, e);
        }

        public virtual void OnUpdating(EntityReplyEventArgs<TModel> e)
        {
            Updating?.Invoke(this, e);
        }

        public virtual void OnUpdated(EntityReplyEventArgs<TModel> e)
        {
            Updated?.Invoke(this, e);
        }

        public virtual void OnDeleting(EntityReplyEventArgs<TModel> e)
        {
            Deleting?.Invoke(this, e);
        }

        public virtual void OnDeleted(EntityReplyEventArgs<TModel> e)
        {
            Deleted?.Invoke(this, e);
        }
    }

}
