using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Repositories;

namespace Plato.Moderation.Repositories
{
    
    public interface IModeratorRepository<T> : IRepository<T> where T : class
    {

    }

    public class ModeratorRepository
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<ModeratorRepository> _logger;

        public ModeratorRepository(
            IDbContext dbContext,
            ILogger<ModeratorRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


    }
}
