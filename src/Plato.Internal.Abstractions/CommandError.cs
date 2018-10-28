namespace Plato.Internal.Abstractions
{

    public class CommandError
    {

        public CommandError(string description) : this("", description)
        {
        }

        public CommandError(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }

        public string Code { get; set; }

        public string Description { get; set; }

    }

}
