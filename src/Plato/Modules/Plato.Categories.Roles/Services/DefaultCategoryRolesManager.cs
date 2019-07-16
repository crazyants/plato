using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Categories.Roles.Services
{
    
    /// <summary>
    /// When enabling role based security the DefaultCategoryRolesManager is responsible
    /// for adding relationships for all existing roles to all existing categories
    /// </summary>
    public class DefaultCategoryRolesManager : IDefaultCategoryRolesManager
    {

        private readonly IDbHelper _dbHelper;

        public DefaultCategoryRolesManager(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task InstallAsync()
        {

            // Iterates all categories adding relationships for all roles
            await _dbHelper.ExecuteScalarAsync<int>(@"
                DECLARE @categoryId int;
                DECLARE @roleId int;                
                DECLARE MSGCURSOR1 CURSOR FOR SELECT Id FROM {prefix}_Categories
                OPEN MSGCURSOR1 FETCH NEXT FROM MSGCURSOR1 INTO @categoryId;	                
                WHILE @@FETCH_STATUS = 0
                BEGIN
	                
	                DECLARE MSGCURSOR2 CURSOR FOR SELECT Id FROM {prefix}_Roles
	                OPEN MSGCURSOR2 FETCH NEXT FROM MSGCURSOR2 INTO @roleId;	                
	                WHILE @@FETCH_STATUS = 0
	                BEGIN

		                -- ensure category role relationship does not already exist
		                IF (NOT EXISTS(SELECT Id FROM {prefix}_CategoryRoles 
			                WHERE CategoryId = @categoryId AND RoleId = @roleId))
		                BEGIN
			                INSERT INTO {prefix}_CategoryRoles (
				                CategoryId,
				                RoleId,
                                CreatedUserId,
                                CreatedDate,
                                ModifiedUserId,
                                ModifiedDate
			                ) VALUES (
				                @categoryId,
				                @roleId,
                                0,
                                getutcdate(),
                                0,
                                NULL
			                );
		                END;

		                FETCH NEXT FROM MSGCURSOR2 INTO @roleId;
	                
	                END;

	                CLOSE MSGCURSOR2;
	                DEALLOCATE MSGCURSOR2;

	                FETCH NEXT FROM MSGCURSOR1 INTO @categoryId;
	                
                END;

                CLOSE MSGCURSOR1;
                DEALLOCATE MSGCURSOR1;");
            
        }

    }

}
