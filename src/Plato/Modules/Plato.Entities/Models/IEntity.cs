using System;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Models;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public interface IEntityMetaData<TNodel> : IMetaData<TNodel> where TNodel : class
    {

    }

    public interface IEntity  : IEntityMetaData<IEntityData>
    {

        int Id { get; set; }

        int ParentId { get; set; }

        int FeatureId { get; set; }

        int CategoryId { get; set; }

        string Title { get; set; }

        string Alias { get; set; }
        
        string Message { get; set; }

        string Html { get; set; }

        string Abstract { get; set; }

        string Urls { get; set; }
        
        bool IsPrivate { get; set; }

        bool IsSpam { get; set; }

        bool IsPinned { get; set; }

        bool IsDeleted { get; set; }

        bool IsClosed { get; set; }
        
        int TotalViews { get; set; }

        int TotalReplies { get; set; }

        int TotalParticipants { get; set; }

        int TotalReactions { get; set; }

        int TotalFollows { get; set; }

        int TotalReports { get; set; }

        double DailyViews { get; set; }

        double DailyReplies { get; set; }

        double DailyReactions { get; set; }

        double DailyFollows { get; set; }

        double DailyReports { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        int LastReplyId { get; set; }

        int LastReplyUserId { get; set; }
        
        DateTimeOffset? LastReplyDate { get; set; }
        
        SimpleUser CreatedBy { get; }

        SimpleUser ModifiedBy { get; }

        Task<EntityUris> GetEntityUrlsAsync();

        void PopulateModel(IDataReader dr);

    }
    
    public interface IEntityData
    {

        int Id { get; set; }

        int EntityId { get; set; }

        string Key { get; set; }

        string Value { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int CreatedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        int ModifiedUserId { get; set; }

    }

}
