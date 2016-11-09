using System;
using System.Collections.Generic;
using System.Data;
using Plato.Abstractions.Extensions;

namespace Plato.Abstractions.Collections
{
    public class PagedResults<T> : IPagedResults<T> where T : class
    {

        private IList<T> _data;

        public IList<T> Data
        {
            get
            {
                if (_data == null)
                    _data = new List<T>();
                return _data;
            }

            set { _data = value; }
        }

        public int Total { get; set; }


        public void PopulateTotal(IDataReader reader)
        {
            if (reader.ColumnIsNotNull(0))
                Total = Convert.ToInt32(reader[0]);
        }
    }
}