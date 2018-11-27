using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Follow.Models
{

    public interface IFollowType
    {
        string Name { get; }

        string Description { get; }

    }
    
    public class FollowType : IFollowType
    {

        public string Name { get; }

        public string Description { get; }

        public FollowType(string name)
        {
            this.Name = name;
        }
        public FollowType(string name, string description) : this(name)
        {
            this.Description = description;
        }

    }

}
