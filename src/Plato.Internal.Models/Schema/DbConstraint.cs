using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Schema
{

    /// <summary>
    /// Models a constraint type in the database.
    /// </summary>
    public class DbConstraint : IDbModel<DbConstraint>
    {

        // PRIMARY KEY - A combination of a NOT NULL and UNIQUE. Uniquely identifies each row in a table
        private const string ByPrimaryKeyConstraintType = "PRIMARY KEY";

        // FOREIGN KEY - Uniquely identifies a row/record in another table
        private const string ByForeignKeyConstraintType = "FOREIGN KEY";

        // INDEX - Used to create and retrieve data from the database very quickly
        private const string ByIndexConstraintType = "INDEX";

        // NOT NULL - Ensures that a column cannot have a NULL value
        private const string ByNotNullConstraintType = "NOT NULL";

        // UNIQUE - Ensures that all values in a column are different
        private const string ByUniqueConstraintType = "UNIQUE";

        // CHECK - Ensures that all values in a column satisfies a specific condition
        private const string ByCheckConstraintType = "CHECK";

        // DEFAULT - Sets a default value for a column when no value is specified
        private const string DefaultConstraintType = "DEFAULT";
        
        // ---------

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public string ConstraintName { get; set; }

        public ConstraintTypes ConstraintType { get; set; } = ConstraintTypes.None;

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("TableName"))
                TableName = Convert.ToString(dr["TableName"]);

            if (dr.ColumnIsNotNull("ColumnName"))
                ColumnName = Convert.ToString(dr["ColumnName"]);

            if (dr.ColumnIsNotNull("ConstraintName"))
                ConstraintName = Convert.ToString(dr["ConstraintName"]);

            if (dr.ColumnIsNotNull("ConstraintType"))
            {
                var constraintType = Convert.ToString(dr["ConstraintType"]);
                switch (constraintType.ToUpper())
                {
                    case ByPrimaryKeyConstraintType:
                        ConstraintType = ConstraintTypes.PrimaryKey;
                        break;
                    case ByForeignKeyConstraintType:
                        ConstraintType = ConstraintTypes.ForeignKey;
                        break;
                    case ByIndexConstraintType:
                        ConstraintType = ConstraintTypes.Index;
                        break;
                    case ByNotNullConstraintType:
                        ConstraintType = ConstraintTypes.NotNull;
                        break;
                    case ByUniqueConstraintType:
                        ConstraintType = ConstraintTypes.Unique;
                        break;
                    case ByCheckConstraintType:
                        ConstraintType = ConstraintTypes.Check;
                        break;
                    case DefaultConstraintType:
                        ConstraintType = ConstraintTypes.Default;
                        break;
                }
                
            }
          

        }
    }


    public enum ConstraintTypes
    {
        None,
        PrimaryKey,
        ForeignKey,
        Index,
        NotNull,
        Unique,
        Check,
        Default
    }

}
