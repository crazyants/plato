namespace Plato.Internal.Messaging.Abstractions
{
    public class MessageOptions
    {
        /// <summary>
        ///  Gets or sets the identifying key for the subscriner delegate. 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the sort order of execution for the subscriner delegate. 
        /// </summary>
        public short Order { get; set; }

    }
    
}
