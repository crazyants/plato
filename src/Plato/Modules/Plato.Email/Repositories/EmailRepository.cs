using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Emails.Abstractions;

namespace Plato.Email.Repositories
{
    
    public class EmailRepository : IEmailRepository<EmailMessage>
    {
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<EmailRepository> _logger;
   
        public EmailRepository(
            IDbContext dbContext,
            ILogger<EmailRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EmailMessage> InsertUpdateAsync(EmailMessage email)
        {

            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
                
            var id = await InsertUpdateInternal(
                email.Id,
                email.To,
                email.Cc,
                email.Bcc,
                email.From,
                email.Subject,
                email.Body,
                (short)email.Priority,
                email.SendAttempts,
                email.CreatedUserId,
                email.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<EmailMessage> SelectByIdAsync(int id)
        {
            EmailMessage email = null;
            using (var context = _dbContext)
            {
                email = await context.ExecuteReaderAsync2<EmailMessage>(
                    CommandType.StoredProcedure,
                    "SelectEmailById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            email = new EmailMessage();
                            email.PopulateModel(reader);
                        }

                        return email;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return email;

        }

        public async Task<IPagedResults<EmailMessage>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<EmailMessage> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EmailMessage>>(
                    CommandType.StoredProcedure,
                    "SelectEmailsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EmailMessage>();
                            while (await reader.ReadAsync())
                            {
                                var email = new EmailMessage();
                                email.PopulateModel(reader);
                                output.Data.Add(email);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                        }

                        return output;
                    },
                    inputParams);
               
            }

            return output;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting email with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteEmailById", new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            string to,
            string cc,
            string bcc,
            string from,
            string subject,
            string body,
            short priority,
            short sendAttempts,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var emailId = 0;
            using (var context = _dbContext)
            {
                emailId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEmail",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("To", DbType.String, 255, to),
                        new DbParam("Cc", DbType.String, 255, cc),
                        new DbParam("Bcc", DbType.String, 255, bcc),
                        new DbParam("From", DbType.String, 255, from),
                        new DbParam("Subject", DbType.String, 255, subject),
                        new DbParam("Body", DbType.String, body),
                        new DbParam("Priority", DbType.Int16, priority),
                        new DbParam("SendAttempts", DbType.Int16, sendAttempts),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.Int32, createdDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return emailId;

        }

        #endregion

    }
}
