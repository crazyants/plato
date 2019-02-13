using System;
using System.Runtime.Serialization;
using Plato.Internal.Abstractions;

namespace Plato.StopForumSpam.Models
{
    
    [DataContract]
    public class SpamOperationType : Serializable, ISpamOperationType
    {

        [DataMember(Name = "name")]
        public string Name { get; set; }

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
        public bool AllowAlter { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        public SpamOperationType()
        {
        }

        public SpamOperationType(string name) : this()
        {
            // We always need a name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(Name));
            }
            Name = name;
        }

        public SpamOperationType(string name, string description) : this(name)
        {
            Description = description;
        }

    }


}
