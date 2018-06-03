using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Navigation
{
    public class PagerOptions
    {
        /// <summary>
        /// Gets or sets the current page number or null if none specified.
        /// </summary>
        public int? PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the current page size or null if none specified.
        /// </summary>
        public int? PageSize { get; set; }
    }
}
