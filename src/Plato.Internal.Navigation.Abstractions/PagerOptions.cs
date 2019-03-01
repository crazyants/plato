using System.Runtime.Serialization;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Navigation.Abstractions
{

    [DataContract]
    public class PagerOptions
    {
        private int _page = 1;

        /// <summary>
        /// Gets or sets the current page. If an offset is provided the page is calculated from this offset.
        /// </summary>
        [DataMember(Name = "page")]
        public int Page
        {
            get => Offset > 0 ? Offset.ToSafeCeilingDivision(PageSize) : _page;
            set => _page = value;
        }

        [DataMember(Name = "size")]
        public int PageSize { get; set; } = 20;

        [DataMember(Name = "offset")]
        public int Offset { get; set; }
 
        public bool Enabled { get; set; } = false;

        // Private setters

        public int Total { get; private set; }

        public int TotalPages { get; private set; }
        
        /// <summary>
        /// Gets the row offset for the current page.
        /// </summary>
        public int RowOffset => PageSize * Page - PageSize + 1;
        
        /// <summary>
        /// Gets or sets the call back Url which can be used for client side paging purposes.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Returns a route value dictionary from the supplied route data with all pager specific route values removed.
        /// </summary>
        /// <param name="routeData"></param>
        /// <returns></returns>
        public RouteValueDictionary Route(RouteData routeData)
        {
            routeData.Values.Remove("pager.page");
            routeData.Values.Remove("pager.size");
            routeData.Values.Remove("pager.offset");
            return routeData.Values;
        }

        public void SetTotal(int total)
        {
            Total = total;
            TotalPages = total > 0 ? total.ToSafeCeilingDivision(PageSize) : 1; 
        }

    }

}
