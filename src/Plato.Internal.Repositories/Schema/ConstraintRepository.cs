using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Schema;

namespace Plato.Internal.Repositories.Schema
{
    
    public class ConstraintRepository : IConstraintRepository
    {

        private const string BySql = @"
            SELECT 
                KU.table_name AS TableName,
                column_name AS ColumnName,
                TC.CONSTRAINT_NAME AS ConstraintName,
                TC.CONSTRAINT_TYPE AS ConstraintType
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC
            INNER JOIN
                INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU
                      ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
            ORDER BY KU.TABLE_NAME, KU.ORDINAL_POSITION;";

        private readonly IDbContext _dbContext;

        public ConstraintRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DbConstraint>> SelectConstraintsAsync()
        {

            ICollection<DbConstraint> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.Text, BySql);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<DbConstraint>();
                    while (await reader.ReadAsync())
                    {
                        var constraint = new DbConstraint();
                        constraint.PopulateModel(reader);
                        output.Add(constraint);
                    }

                }
            }

            return output;

        }

    }
    
}
