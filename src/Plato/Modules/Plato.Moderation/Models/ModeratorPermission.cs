using System;
using System.Collections.Generic;
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

        public ModeratorPermission(string name, string description, IEnumerable<IPermission> impliedBy) : this(name, description, string.Empty)
        {
            this.ImpliedBy = impliedBy;
        }

        public ModeratorPermission(string name, string description, string category, IEnumerable<IPermission> impliedBy) : this(name, description, category)
        {
            this.ImpliedBy = impliedBy;
        }
        
    }

}
