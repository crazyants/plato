using System;
using System.Data;
using Plato.Email.Models;
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
                email = await context.ExecuteReaderAsync<EmailMessage>(
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
                    },
                    id);

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
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEmailById", id);
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
                emailId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEmail",
                    id,
                    to.ToEmptyIfNull().TrimToSize(255),
                    cc.ToEmptyIfNull().TrimToSize(255),
                    bcc.ToEmptyIfNull().TrimToSize(255),
                    from.ToEmptyIfNull().TrimToSize(255),
                    subject.ToEmptyIfNull().TrimToSize(255),
                    body.ToEmptyIfNull(),
                    priority,
                    sendAttempts,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return emailId;

        }

        #endregion

    }
}
