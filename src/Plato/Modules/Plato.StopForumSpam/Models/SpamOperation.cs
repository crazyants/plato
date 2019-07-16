using System;
using System.Runtime.Serialization;
using Plato.Internal.Abstractions;

namespace Plato.StopForumSpam.Models
{
    
    [DataContract]
    public class SpamOperation : Serializable, ISpamOperation
    {

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }


        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "flagAsSpam")]
        public bool FlagAsSpam { get; set; }

        [DataMember(Name = "notifyAdmin")]
        public bool NotifyAdmin { get; set; }

        [DataMember(Name = "notifyStaff")]
        public bool NotifyStaff { get; set; }

        [DataMember(Name = "allowAlter")]
        public bool CustomMessage { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        public SpamOperation()
        {
        }

        public SpamOperation(string name) : this()
        {
            // We always need a name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(Name));
            }
            Name = name;
        }

        public SpamOperation(string name, string title) : this(name)
        {
            Title = title;
        }

        public SpamOperation(string name, string title, string description) : this(name, title)
        {
            Description = description;
        }

    }


}
