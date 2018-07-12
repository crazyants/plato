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

    }

    public class EntityReplyEvents
    {

        public event Handler Creating;
        public event Handler Created;
        public event Handler Updating;
        public event Handler Updated;
        public event Handler Deleting;
        public event Handler Deleted;

        public delegate void Handler(object sender, EntityReplyEventArgs e);

    }

}
