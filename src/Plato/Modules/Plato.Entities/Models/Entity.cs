using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{
    public class Entity :
        IComparable<IEntity>,
        IEntity
    {
        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;
        
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int FeatureId { get; set; }

        public string ModuleId { get; set; }

        public int CategoryId { get; set; }
        
        public string Title { get; set; }

        public string Alias { get; set; }
        
        public string Message { get; set; }

        public string Html { get; set; }

        public string Abstract { get; set; }

        public string Urls { get; set; }
        
        public bool IsHidden { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocked { get; set; }

        public bool IsClosed { get; set; }


        public int TotalViews { get; set; }

        public int TotalReplies { get; set; }

        public int TotalAnswers { get; set; }

        public int TotalParticipants { get; set; }

        public int TotalReactions { get; set; }

        public int TotalFollows { get; set; }

        public int TotalReports { get; set; }

        public int TotalStars { get; set; }

        public int TotalRatings { get; set; }

        public int SummedRating { get; set; }

        public int MeanRating { get; set; }

        public int TotalLinks { get; set; }

        public int TotalImages { get; set; }

        public double DailyViews { get; set; }

        public double DailyReplies { get; set; }

        public double DailyAnswers { get; set; }

        public double DailyReactions { get; set; }

        public double DailyFollows { get; set; }

        public double DailyReports { get; set; }

        public double DailyStars { get; set; }
        public double DailyRatings { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int EditedUserId { get; set; }

        public DateTimeOffset? EditedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public int LastReplyId { get; set; }

        public int LastReplyUserId { get; set; }

        public DateTimeOffset? LastReplyDate { get; set; }

        public SimpleUser CreatedBy { get; private set; } = new SimpleUser();

        public SimpleUser ModifiedBy { get; private set; } = new SimpleUser();

        public SimpleUser LastReplyBy { get; private set; } = new SimpleUser();
        
        public int Rank { get; set; }

        public int MaxRank { get; set; }

        public int Relevance { get; set; }

        // IMetaData

        public IEnumerable<IEntityData> Data { get; set; } = new List<EntityData>();

        public IDictionary<Type, ISerializable> MetaData => _metaData;

        // INestable

        public IEnumerable<IEntity> Children { get; set; } = new List<IEntity>();

        public IEntity Parent { get; set; }

        public int Depth { get; set; }

        public int SortOrder { get; set; }
        
        public Entity()
        {
            _metaData = new ConcurrentDictionary<Type, ISerializable>();
        }

        public void AddOrUpdate<T>(T obj) where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                _metaData.TryUpdate(typeof(T), (ISerializable)obj, _metaData[typeof(T)]);
            }
            else
            {
                _metaData.TryAdd(typeof(T), (ISerializable)obj);
            }
        }

        public void AddOrUpdate(Type type, ISerializable obj)
        {
            if (_metaData.ContainsKey(type))
            {
                _metaData.TryUpdate(type, (ISerializable)obj, _metaData[type]);
            }
            else
            {
                _metaData.TryAdd(type, obj);
            }
        }

        public T GetOrCreate<T>() where T : class
        {
            if (_metaData.ContainsKey(typeof(T)))
            {
                return (T)_metaData[typeof(T)];
            }

            return ActivateInstanceOf<T>.Instance(); 

        }

        public async Task<EntityUris> GetEntityUrlsAsync()
        {
            if (!string.IsNullOrEmpty(Urls))
            {
                return await Urls.DeserializeAsync<EntityUris>();
            }

            return new EntityUris();
        }

        public virtual void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ParentId"))
                ParentId = Convert.ToInt32(dr["ParentId"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                FeatureId = Convert.ToInt32(dr["FeatureId"]);

            if (dr.ColumnIsNotNull("ModuleId"))
                ModuleId = Convert.ToString(dr["ModuleId"]);

            if (dr.ColumnIsNotNull("CategoryId"))
                CategoryId = Convert.ToInt32(dr["CategoryId"]);
            
            if (dr.ColumnIsNotNull("Title"))
                Title = Convert.ToString(dr["Title"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("Message"))
                Message = Convert.ToString(dr["Message"]);

            if (dr.ColumnIsNotNull("Html"))
                Html = Convert.ToString(dr["Html"]);

            if (dr.ColumnIsNotNull("Abstract"))
                Abstract = Convert.ToString(dr["Abstract"]);

            if (dr.ColumnIsNotNull("Urls")) 
                Urls = Convert.ToString(dr["Urls"]);
            
            if (dr.ColumnIsNotNull("IsHidden"))
                IsHidden = Convert.ToBoolean(dr["IsHidden"]);

            if (dr.ColumnIsNotNull("IsPrivate"))
                IsPrivate = Convert.ToBoolean(dr["IsPrivate"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("IsPinned"))
                IsPinned = Convert.ToBoolean(dr["IsPinned"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("IsLocked"))
                IsLocked = Convert.ToBoolean(dr["IsLocked"]);

            if (dr.ColumnIsNotNull("IsClosed"))
                IsClosed = Convert.ToBoolean(dr["IsClosed"]);

            if (dr.ColumnIsNotNull("TotalViews"))
                TotalViews = Convert.ToInt32(dr["TotalViews"]);

            if (dr.ColumnIsNotNull("TotalReplies"))
                TotalReplies = Convert.ToInt32(dr["TotalReplies"]);
            
            if (dr.ColumnIsNotNull("TotalAnswers"))
                TotalAnswers = Convert.ToInt32(dr["TotalAnswers"]);

            if (dr.ColumnIsNotNull("TotalParticipants"))
                TotalParticipants = Convert.ToInt32(dr["TotalParticipants"]);

            if (dr.ColumnIsNotNull("TotalReactions"))
                TotalReactions = Convert.ToInt32(dr["TotalReactions"]);
            
            if (dr.ColumnIsNotNull("TotalFollows"))
                TotalFollows = Convert.ToInt32(dr["TotalFollows"]);

            if (dr.ColumnIsNotNull("TotalReports"))
                TotalReports = Convert.ToInt32(dr["TotalReports"]);

            if (dr.ColumnIsNotNull("TotalStars"))
                TotalStars = Convert.ToInt32(dr["TotalStars"]);

            if (dr.ColumnIsNotNull("TotalRatings"))
                TotalRatings = Convert.ToInt32(dr["TotalRatings"]);

            if (dr.ColumnIsNotNull("SummedRating"))
                SummedRating = Convert.ToInt32(dr["SummedRating"]);

            if (dr.ColumnIsNotNull("MeanRating"))
                MeanRating = Convert.ToInt32(dr["MeanRating"]);
            
            if (dr.ColumnIsNotNull("TotalLinks"))
                TotalLinks = Convert.ToInt32(dr["TotalLinks"]);
            
            if (dr.ColumnIsNotNull("TotalImages"))
                TotalImages = Convert.ToInt32(dr["TotalImages"]);
            
            if (dr.ColumnIsNotNull("DailyViews"))
                DailyViews = Convert.ToDouble(dr["DailyViews"]);
            
            if (dr.ColumnIsNotNull("DailyReplies"))
                DailyReplies = Convert.ToDouble(dr["DailyReplies"]);

            if (dr.ColumnIsNotNull("DailyAnswers"))
                DailyAnswers = Convert.ToDouble(dr["DailyAnswers"]);
            
            if (dr.ColumnIsNotNull("DailyReactions"))
                DailyReactions = Convert.ToDouble(dr["DailyReactions"]);

            if (dr.ColumnIsNotNull("DailyFollows"))
                DailyFollows = Convert.ToDouble(dr["DailyFollows"]);

            if (dr.ColumnIsNotNull("DailyReports"))
                DailyReports = Convert.ToDouble(dr["DailyReports"]);
            
            if (dr.ColumnIsNotNull("DailyStars"))
                DailyStars = Convert.ToDouble(dr["DailyStars"]);

            if (dr.ColumnIsNotNull("DailyRatings"))
                DailyRatings = Convert.ToDouble(dr["DailyRatings"]);

            if (dr.ColumnIsNotNull("SortOrder"))
                SortOrder = Convert.ToInt32(dr["SortOrder"]);
            
            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (CreatedUserId > 0)
            {
                CreatedBy.Id = CreatedUserId;
                if (dr.ColumnIsNotNull("CreatedUserName"))
                    CreatedBy.UserName = Convert.ToString(dr["CreatedUserName"]);
                if (dr.ColumnIsNotNull("CreatedDisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["CreatedDisplayName"]);
                if (dr.ColumnIsNotNull("CreatedAlias"))
                    CreatedBy.Alias = Convert.ToString(dr["CreatedAlias"]);
                if (dr.ColumnIsNotNull("CreatedPhotoUrl"))
                    CreatedBy.PhotoUrl = Convert.ToString(dr["CreatedPhotoUrl"]);
                if (dr.ColumnIsNotNull("CreatedPhotoColor"))
                    CreatedBy.PhotoColor = Convert.ToString(dr["CreatedPhotoColor"]);
                if (dr.ColumnIsNotNull("CreatedSignatureHtml"))
                    CreatedBy.SignatureHtml = Convert.ToString(dr["CreatedSignatureHtml"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];
            
            if (dr.ColumnIsNotNull("EditedUserId"))
                EditedUserId = Convert.ToInt32(dr["EditedUserId"]);
            
            if (dr.ColumnIsNotNull("EditedDate"))
                EditedDate = (DateTimeOffset)dr["EditedDate"];

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (ModifiedUserId > 0)
            {
                ModifiedBy.Id = ModifiedUserId;
                if (dr.ColumnIsNotNull("ModifiedUserName"))
                    ModifiedBy.UserName = Convert.ToString(dr["ModifiedUserName"]);
                if (dr.ColumnIsNotNull("ModifiedDisplayName"))
                    ModifiedBy.DisplayName = Convert.ToString(dr["ModifiedDisplayName"]);
                if (dr.ColumnIsNotNull("ModifiedAlias"))
                    ModifiedBy.Alias = Convert.ToString(dr["ModifiedAlias"]);
                if (dr.ColumnIsNotNull("ModifiedPhotoUrl"))
                    ModifiedBy.PhotoUrl = Convert.ToString(dr["ModifiedPhotoUrl"]);
                if (dr.ColumnIsNotNull("ModifiedPhotoColor"))
                    ModifiedBy.PhotoColor = Convert.ToString(dr["ModifiedPhotoColor"]);
                if (dr.ColumnIsNotNull("ModifiedSignatureHtml"))
                    ModifiedBy.SignatureHtml = Convert.ToString(dr["ModifiedSignatureHtml"]);
            }

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = (DateTimeOffset)dr["ModifiedDate"];
         
            if (dr.ColumnIsNotNull("LastReplyId"))
                LastReplyId = Convert.ToInt32(dr["LastReplyId"]);

            if (dr.ColumnIsNotNull("LastReplyUserId"))
                LastReplyUserId = Convert.ToInt32(dr["LastReplyUserId"]);

            if (LastReplyUserId > 0)
            {
                LastReplyBy.Id = LastReplyUserId;
                if (dr.ColumnIsNotNull("LastReplyUserName"))
                    LastReplyBy.UserName = Convert.ToString(dr["LastReplyUserName"]);
                if (dr.ColumnIsNotNull("LastReplyDisplayName"))
                    LastReplyBy.DisplayName = Convert.ToString(dr["LastReplyDisplayName"]);
                if (dr.ColumnIsNotNull("LastReplyPhotoUrl"))
                    LastReplyBy.PhotoUrl = Convert.ToString(dr["LastReplyPhotoUrl"]);
                if (dr.ColumnIsNotNull("LastReplyPhotoColor"))
                    LastReplyBy.PhotoColor = Convert.ToString(dr["LastReplyPhotoColor"]);
                if (dr.ColumnIsNotNull("LastReplySignatureHtml"))
                    LastReplyBy.SignatureHtml = Convert.ToString(dr["LastReplySignatureHtml"]);
            }
            
            if (dr.ColumnIsNotNull("LastReplyDate"))
                LastReplyDate = (DateTimeOffset)dr["LastReplyDate"];

            if (dr.ColumnIsNotNull("Rank"))
                Rank = Convert.ToInt32(dr["Rank"]);

            if (dr.ColumnIsNotNull("MaxRank"))
                MaxRank = Convert.ToInt32(dr["MaxRank"]);

            Relevance = Rank.ToPercentageOf(MaxRank);
        }

        public int CompareTo(IEntity other)
        {
            if (other == null)
                return 1;
            var sortOrderCompare = other.SortOrder;
            if (this.SortOrder == sortOrderCompare)
                return 0;
            if (this.SortOrder < sortOrderCompare)
                return -1;
            if (this.SortOrder > sortOrderCompare)
                return 1;
            return 0;
        }
    }
    
}
