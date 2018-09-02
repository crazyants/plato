namespace Plato.Internal.Abstractions
{

    public class ActivityError
    {

        public ActivityError(string description) : this("", description)
        {
        }

        public ActivityError(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }

        public string Code { get; set; }

        public string Description { get; set; }

    }

}
