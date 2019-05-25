using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Search.Models;

namespace Plato.Search.Repositories
{
    
    public class FullTextIndexRepository : IFullTextIndexRepository
    {

        private const string BySql = @"
            SELECT 
	            fi.unique_index_id AS Id,
	            fi.fulltext_catalog_id AS CatalogId,
                t.name AS TableName, 
                c.name AS CatalogName ,
                i.name AS UniqueIndexName,
                cl.name AS ColumnName,
                ic.column_id AS ColumnId
            FROM 
                sys.tables t 
            INNER JOIN 
                sys.fulltext_indexes fi 
            ON 
                t.[object_id] = fi.[object_id] 
            INNER JOIN 
                sys.fulltext_index_columns ic
            ON 
                ic.[object_id] = t.[object_id]
            INNER JOIN
                sys.columns cl
            ON 
                    ic.column_id = cl.column_id
                AND ic.[object_id] = cl.[object_id]
            INNER JOIN 
                sys.fulltext_catalogs c 
            ON 
                fi.fulltext_catalog_id = c.fulltext_catalog_id
            INNER JOIN 
                sys.indexes i
            ON 
                fi.unique_index_id = i.index_id
            AND fi.[object_id] = i.[object_id];";

        private readonly IDbContext _dbContext;

        public FullTextIndexRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FullTextIndex>> SelectIndexesAsync()
        {
            IList<FullTextIndex> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2(
                    CommandType.Text,
                    BySql,
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<FullTextIndex>();
                            while (await reader.ReadAsync())
                            {
                                var index = new FullTextIndex();
                                index.PopulateModel(reader);
                                output.Add(index);
                            }

                        }

                        return output;

                    });
              
                // Remove table prefix from table names
                if (output != null)
                {
                    foreach (var index in output)
                    {
                        index.TableName = index.TableName.Replace(context.Configuration.TablePrefix, "");
                    }
                }

            }
            
            return output;

        }

    }

}
