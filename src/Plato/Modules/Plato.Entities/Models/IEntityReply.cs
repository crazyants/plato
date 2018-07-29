using System;
using System.Data;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public interface IEntityReply
    {
        int Id { get; set; }

        int EntityId { get; set; }

        int CategoryId { get; set; }

        string Message { get; set; }

        string Html { get; set; }

        string Abstract { get; set; }

        bool IsPrivate { get; set; }

        bool IsSpam { get; set; }

        bool IsPinned { get; set; }

        bool IsDeleted { get; set; }

        bool IsClosed { get; set; }

        int TotalReactions { get; set; }

        int TotalReports { get; set; }

        int MeanReactions { get; set; }

        int MeanReports { get; set; }

        int CreatedUserId { get; set; }

        DateTime? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTime? ModifiedDate { get; set; }

        SimpleUser CreatedBy { get; set; }

        SimpleUser ModifiedBy { get; set; }

        void PopulateModel(IDataReader dr);

    }

}
