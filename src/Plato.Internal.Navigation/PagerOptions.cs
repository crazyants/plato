using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Navigation
{
    public class PagerOptions
    {

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        private int _total;

        public int Total => _total;

        public void SetTotal(int total)
        {
            _total = total;
        }
    }
}
