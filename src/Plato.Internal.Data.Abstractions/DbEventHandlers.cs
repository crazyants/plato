namespace Plato.Internal.Data.Abstractions
{
    public class DbEventHandlers
    {
        public delegate void DbExceptionEventHandler(object sender, DbExceptionEventArgs e);
    }
}
