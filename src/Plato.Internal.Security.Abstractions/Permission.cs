using System;
using System.Collections.Generic;

namespace Plato.Internal.Security.Abstractions
{

    public class Permission : IPermission
    {

        public const string ClaimTypeName = "Permission";

        public string ClaimType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public IEnumerable<IPermission> ImpliedBy { get; set; }

        public Permission()
        {
            ClaimType = ClaimTypeName;
        }

        public Permission(string name) : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Permission(string name, string description) : this(name)
        {
            this.Description = description;
        }

        public Permission(string name, string description, string category) : this(name, description)
        {
            this.Category = category;
        }

        public Permission(string name, string description, string category, IEnumerable<IPermission> impliedBy) : this(name, description, category)
        {
            this.ImpliedBy = impliedBy;
        }
        
    }

}
