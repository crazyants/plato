using System;
using Plato.Abstractions.Extensions;
using Plato.Models.Annotations;

namespace Plato.Models.Tennets
{
    [TableName("Plato_Tennets")]
    public class Tennet
    {

        [ColumnName("Id", typeof(int))]
        public int ID { get; set;  }

        [ColumnName("Name", typeof(string), 255)]
        public string Name { get; set; }

        [ColumnName("SafeUrlName", typeof(string), 255)]
        public string SafeUrlName { get; set; }

        [ColumnName("Description", typeof(string), 255)]
        public string Description { get; set; }

        [ColumnName("ConnectionString", typeof(string), 255)]
        public string ConnectionString { get; set; }

        [ColumnName("IsActive", typeof(bool))]
        public bool IsActive { get; set; }

        [ColumnName("IsDeleted", typeof(bool))]
        public bool IsDeleted { get; set; }

        [ColumnName("CreatedDate", typeof(DateTime?))]
        public bool CreatedDate { get; set; }

        [ColumnName("CreatedUserId", typeof(int))]
        public int CreatedUserId { get; set; }

        [ColumnName("ModifiedDate", typeof(DateTime?))]
        public bool ModifiedDate { get; set; }

        [ColumnName("ModifiedUserId", typeof(int))]
        public int ModifiedUserId { get; set; }

        [ColumnName("DeletedDate", typeof(DateTime?))]
        public bool DeletedDate { get; set; }

        [ColumnName("DeletedUserId", typeof(int))]
        public int DeletedUserId { get; set; }
        
    }

}
