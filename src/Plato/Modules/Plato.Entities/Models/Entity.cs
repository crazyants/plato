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
    public class Entity :  IEntity
    {
        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;
        
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int FeatureId { get; set; }

        public int CategoryId { get; set; }
        
        public string Title { get; set; }

        public string Alias { get; set; }
        
        public string Message { get; set; }

        public string Html { get; set; }

        public string Abstract { get; set; }

        public string Urls { get; set; }
        
        public bool IsPrivate { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsClosed { get; set; }

        public int TotalViews { get; set; }

        public int TotalReplies { get; set; }

        public int TotalParticipants { get; set; }

        public int TotalReactions { get; set; }

        public int TotalFollows { get; set; }

        public int TotalReports { get; set; }

        public double DailyViews { get; set; }

        public double DailyReplies { get; set; }

        public double DailyReactions { get; set; }

        public double DailyFollows { get; set; }

        public double DailyReports { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public int LastReplyId { get; set; }

        public int LastReplyUserId { get; set; }

        public DateTimeOffset? LastReplyDate { get; set; }

        public SimpleUser CreatedBy { get; private set; } = new SimpleUser();

        public SimpleUser ModifiedBy { get; private set; } = new SimpleUser();

        public SimpleUser LastReplyBy { get; private set; } = new SimpleUser();

        public IEnumerable<IEntityData> Data { get; set; } = new List<EntityData>();

        public IDictionary<Type, ISerializable> MetaData => _metaData;

        public int Rank { get; set; }

        public int MaxRank { get; set; }

        public int Relevance { get; set; }

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
            
            if (dr.ColumnIsNotNull("TotalViews"))
                TotalViews = Convert.ToInt32(dr["TotalViews"]);

            if (dr.ColumnIsNotNull("TotalReplies"))
                TotalReplies = Convert.ToInt32(dr["TotalReplies"]);
            
            if (dr.ColumnIsNotNull("TotalParticipants"))
                TotalParticipants = Convert.ToInt32(dr["TotalParticipants"]);

            if (dr.ColumnIsNotNull("TotalReactions"))
                TotalReactions = Convert.ToInt32(dr["TotalReactions"]);
            
            if (dr.ColumnIsNotNull("TotalFollows"))
                TotalFollows = Convert.ToInt32(dr["TotalFollows"]);
            
            if (dr.ColumnIsNotNull("IsPrivate"))
                IsPrivate = Convert.ToBoolean(dr["IsPrivate"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("IsPinned"))
                IsPinned = Convert.ToBoolean(dr["IsPinned"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("IsClosed"))
                IsClosed = Convert.ToBoolean(dr["IsClosed"]);

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
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

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
            }
            
            if (dr.ColumnIsNotNull("LastReplyDate"))
                LastReplyDate = (DateTimeOffset)dr["LastReplyDate"];

            if (dr.ColumnIsNotNull("Rank"))
                Rank = Convert.ToInt32(dr["Rank"]);

            if (dr.ColumnIsNotNull("MaxRank"))
                MaxRank = Convert.ToInt32(dr["MaxRank"]);

            Relevance = Rank.ToPercentageOf(MaxRank);
        }


    }
    
}
