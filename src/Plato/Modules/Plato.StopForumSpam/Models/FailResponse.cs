namespace Plato.StopForumSpam.Models
{
    public class FailResponse : Response
    {

        public FailResponse(string reply, string format) : base(reply, format)
        {
        }

        public string Error { get; internal set; }

    }

}
