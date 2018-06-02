namespace Plato.Abstractions.Data
{
    public class DbEventHandlers
    {
        public delegate void DbExceptionEventHandler(object sender, DbExceptionEventArgs e);
    }
}
