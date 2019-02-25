using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

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

        public SortBy Sort { get; set; }

        public OrderBy Order { get; set; }

    }


    public enum SortBy
    {
        Username,
        Email
    }


}
