using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Security.Abstractions;

namespace Plato.Moderation.Models
{
    public class ModeratorPermission : IPermission
    {

        public const string ClaimType = "ModeratorPermission";

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public IEnumerable<IPermission> ImpliedBy { get; set; }

        public ModeratorPermission(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public ModeratorPermission(string name, string description) : this(name)
        {
            this.Description = description;
        }

        public ModeratorPermission(string name, string description, string category) : this(name, description)
        {
            this.Category = category;
        }

    }

}
