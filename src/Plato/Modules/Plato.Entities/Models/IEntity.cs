using System;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public class SimpleEntity : ISimpleEntity
    {
        
        public int Id { get; set; }

        public int FeatureId { get; set; }

        public string ModuleId { get; set; }

        public string Title { get; set; }

        public string Alias { get; set; }

    }

    public interface ISimpleEntity
    {
        int Id { get; set; }

        int FeatureId { get; set; }

        string ModuleId { get; set; }

        string Title { get; set; }

        string Alias { get; set; }
        
    }

    public interface IEntity  :
        ISimpleEntity,
        IEntityMetaData<IEntityData>,
        INestable<IEntity>
    {
        
        int ParentId { get; set; }
        
        int CategoryId { get; set; }
        
        string Message { get; set; }

        string Html { get; set; }

        string Abstract { get; set; }

        string Urls { get; set; }
        
        bool IsPrivate { get; set; }

        bool IsSpam { get; set; }

        bool IsPinned { get; set; }

        bool IsDeleted { get; set; }

        bool IsLocked { get; set; }

        bool IsClosed { get; set; }
        
        int TotalViews { get; set; }

        int TotalReplies { get; set; }

        int TotalAnswers { get; set; }
        
        int TotalParticipants { get; set; }

        int TotalReactions { get; set; }

        int TotalFollows { get; set; }

        int TotalReports { get; set; }

        int TotalStars { get; set; }

        int TotalRatings { get; set; }

        int SummedRating { get; set; }
        
        int MeanRating { get; set; }

        int TotalLinks { get; set; }

        int TotalImages { get; set; }

        double DailyViews { get; set; }

        double DailyReplies { get; set; }

        double DailyAnswers { get; set; }

        double DailyReactions { get; set; }

        double DailyFollows { get; set; }

        double DailyReports { get; set; }

        double DailyStars { get; set; }

        double DailyRatings { get; set; }
        
        int CreatedUserId { get; set; }

        DateTimeOffset? CreatedDate { get; set; }

        int EditedUserId { get; set; }

        DateTimeOffset? EditedDate { get; set; }
        
        int ModifiedUserId { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        int LastReplyId { get; set; }

        int LastReplyUserId { get; set; }
        
        DateTimeOffset? LastReplyDate { get; set; }
        
        SimpleUser CreatedBy { get; }

        SimpleUser ModifiedBy { get; }

        SimpleUser LastReplyBy { get; }
        
        Task<EntityUris> GetEntityUrlsAsync();

        void PopulateModel(IDataReader dr);
        
        int Rank { get; set; }

        int MaxRank { get; set; }

        int Relevance { get; set; }

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

