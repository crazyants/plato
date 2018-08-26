using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Security.Abstractions
{
    public interface IPermission
    {
        string Name { get; set; }

        string Description { get; set; }

        string Category { get; set; }

        IEnumerable<IPermission> ImpliedBy { get; set; }

    }


    public class Permission : IPermission
    {

        public const string ClaimType = "Permission";

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public IEnumerable<IPermission> ImpliedBy { get; set; }

        public Permission(string name)
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

    }

}
