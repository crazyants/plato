namespace Plato.Entities.Services
{
    public class EntityEvents
    {

        public event Handler Creating;
        public event Handler Created;
        public event Handler Updating;
        public event Handler Updated;
        public event Handler Deleting;
        public event Handler Deleted;

        public delegate void Handler(object sender, EntityEventArgs e);

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
