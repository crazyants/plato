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
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.FilterOpts = filterOptions;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results.Total);
        }


        public IPagedResults<Role> Results { get; set; }

        public PagerOptions PagerOpts { get; set; }

        public FilterOptions FilterOpts { get; set; }
        
    }
    
    public class FilterOptions
    {

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public RolesOrder Order { get; set; }

    }

    public enum RolesOrder
    {
        Username,
        Email
    }


}
