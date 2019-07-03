using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public class EntityReply : IEntityReply
    {

        public int Id { get; set; }
        
        public int ParentId { get; set; }

        public int EntityId { get; set; }
        
        public int CategoryId { get; set; }
        
        public string Message { get; set; }

        public string Html { get; set; }

        public string Abstract { get; set; }

        public string Urls { get; set; }
        
        public bool IsHidden { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsClosed { get; set; }

        public bool IsAnswer { get; set; }

        public int TotalReactions { get; set; }

        public int TotalReports { get; set; }

        public int TotalRatings { get; set; }

        public int SummedRating { get; set; }

        public int MeanRating { get; set; }

        public int TotalLinks { get; set; }

        public int TotalImages { get; set; }

        public string IpV4Address { get; set; }

        public string IpV6Address { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int EditedUserId { get; set; }

        public DateTimeOffset? EditedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public ISimpleUser CreatedBy { get; set; } = new SimpleUser();

        public ISimpleUser ModifiedBy { get; set; } = new SimpleUser();
        
        // IMetaData

        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;

        public IEnumerable<IEntityReplyData> Data { get; set; } = new List<EntityReplyData>();
        
        public EntityReply()
        {
            _metaData = new ConcurrentDictionary<Type, ISerializable>();
        }

        public async Task<EntityUris> GetEntityUrlsAsync()
        {
            if (!string.IsNullOrEmpty(Urls))
            {
                return await Urls.DeserializeAsync<EntityUris>();
            }

            return new EntityUris();
        }

        // IDbModel

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);
            
            if (dr.ColumnIsNotNull("Message"))
                Message = Convert.ToString(dr["Message"]);

            if (dr.ColumnIsNotNull("Html"))
                Html = Convert.ToString(dr["Html"]);

            if (dr.ColumnIsNotNull("EntityId"))
                EntityId = Convert.ToInt32(dr["EntityId"]);

            if (dr.ColumnIsNotNull("Abstract"))
                Abstract = Convert.ToString(dr["Abstract"]);

            if (dr.ColumnIsNotNull("Urls"))
                Urls = Convert.ToString(dr["Urls"]);

            if (dr.ColumnIsNotNull("IsHidden"))
                IsHidden = Convert.ToBoolean(dr["IsHidden"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("IsPinned"))
                IsPinned = Convert.ToBoolean(dr["IsPinned"]);

            if (dr.ColumnIsNotNull("IsDeleted"))
                IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("IsClosed"))
                IsClosed = Convert.ToBoolean(dr["IsClosed"]);

            if (dr.ColumnIsNotNull("IsAnswer"))
                IsAnswer = Convert.ToBoolean(dr["IsAnswer"]);

            if (dr.ColumnIsNotNull("TotalReactions"))
                TotalReactions = Convert.ToInt32(dr["TotalReactions"]);

            if (dr.ColumnIsNotNull("TotalReports"))
                TotalReports = Convert.ToInt32(dr["TotalReports"]);
            
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

            if (dr.ColumnIsNotNull("IpV4Address"))
                IpV4Address = Convert.ToString(dr["IpV4Address"]);

            if (dr.ColumnIsNotNull("IpV6Address"))
                IpV6Address = Convert.ToString(dr["IpV6Address"]);

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

        }

        // IMetaData

        public IDictionary<Type, ISerializable> MetaData => _metaData;

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
        
    }

}
