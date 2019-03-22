using System;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public interface IEntityReply
    {
        int Id { get; set; }

        int ParentId { get; set; }

        int EntityId { get; set; }

        int CategoryId { get; set; }

        string Message { get; set; }

        string Html { get; set; }

        string Abstract { get; set; }

        string Urls { get; set; }

        bool IsPrivate { get; set; }

        bool IsSpam { get; set; }

        bool IsPinned { get; set; }

        bool IsDeleted { get; set; }

        bool IsClosed { get; set; }

        bool IsAnswer { get; set; }
        
        int TotalReactions { get; set; }

        int TotalReports { get; set; }

        int TotalRatings { get; set; }

        int MeanRating { get; set; }
        
        int TotalLinks { get; set; }

        int TotalImages { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int EditedUserId { get; set; }

        DateTimeOffset? EditedDate { get; set; }
        
        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        ISimpleUser CreatedBy { get; set; }

        ISimpleUser ModifiedBy { get; set; }

        Task<EntityUris> GetEntityUrlsAsync();

        void PopulateModel(IDataReader dr);

    }

}
