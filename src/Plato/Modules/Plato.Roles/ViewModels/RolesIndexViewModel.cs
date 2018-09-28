using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Navigation;

namespace Plato.Roles.ViewModels
{
    public class RolesIndexViewModel
    {


        public RolesIndexViewModel()
        {

        }

        public RolesIndexViewModel(
            IPagedResults<Role> results,
            RoleIndexOptions options,
            PagerOptions pager)
        {
            this.Results = results;
            this.Options = options;
            this.Pager = pager;
            this.Pager.SetTotal(results?.Total ?? 0);
        }
        
        public IPagedResults<Role> Results { get; set; }

        public PagerOptions Pager { get; set; }

        public RoleIndexOptions Options { get; set; }
        
    }
    
    public class RoleIndexOptions
    {

        public int RoleId { get; set; }

        public string Search { get; set; }

        public RolesOrder Order { get; set; }

    }

    public enum RolesOrder
    {
        Username,
        Email
    }


}
