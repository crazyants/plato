using System;
using Plato.Internal.Security.Abstractions;

namespace Plato.Moderation.Models
{
    public class ModeratorPermission : Permission
    {
        
        public new const string ClaimTypeName = "ModeratorPermission";

        public ModeratorPermission()
        {
            ClaimType = ClaimTypeName;
        }
        
        public ModeratorPermission(string name) : this()
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
