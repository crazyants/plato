using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Security.Abstractions
{
    
    public class Permission
    {

        public const string ClaimType = "Permission";

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public IEnumerable<Permission> ImpliedBy { get; set; }

        public Permission(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Permission(string name, string description)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Description = description;
        }



    }

}
