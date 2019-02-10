namespace Plato.StopForumSpam.Models
{
    public class SpamFrequencies
    {

        public int UserName { get; set; }

        public int Email { get; set; }

        public int IpAddress { get; set; }

        public bool Success { get; set; }

    }

}
