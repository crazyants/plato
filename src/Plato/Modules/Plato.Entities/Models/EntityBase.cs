using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

namespace Plato.Entities.Models
{
    public class EntityBase :  IEntity
    {
        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;
        
        public int Id { get; set; }

        public int FeatureId { get; set; }

        public string Title { get; set; }

        public string TitleNormalized { get; set; }


        public string Message { get; set; }

        public string Html { get; set; }

        public string Abstract { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsSpam { get; set; }

        public bool IsPinned { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsClosed { get; set; }

        public int CreatedUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public EntityUser CreatedBy { get; private set; } = new EntityUser();

        public EntityUser ModifiedBy { get; private set; } = new EntityUser();

        public IEnumerable<IEntityData> Data { get; set; } = new List<EntityData>();

        public IDictionary<Type, ISerializable> MetaData => _metaData;

        public EntityBase()
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

            return Activator.CreateInstance<T>();

        }


        public virtual void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);
       
            if (dr.ColumnIsNotNull("Message"))
                Message = Convert.ToString(dr["Message"]);

            if (dr.ColumnIsNotNull("Html"))
                Html = Convert.ToString(dr["Html"]);

            if (dr.ColumnIsNotNull("Abstract"))
                Abstract = Convert.ToString(dr["Abstract"]);

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
                if (dr.ColumnIsNotNull("CreatedNormalizedUserName"))
                    CreatedBy.NormalizedUserName = Convert.ToString(dr["CreatedNormalizedUserName"]);
                if (dr.ColumnIsNotNull("CreatedDisplayName"))
                    CreatedBy.DisplayName = Convert.ToString(dr["CreatedDisplayName"]);
            }

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (ModifiedUserId > 0)
            {
                ModifiedBy.Id = ModifiedUserId;
                if (dr.ColumnIsNotNull("ModifiedUserName"))
                    ModifiedBy.UserName = Convert.ToString(dr["ModifiedUserName"]);
                if (dr.ColumnIsNotNull("ModifiedNormalizedUserName"))
                    ModifiedBy.NormalizedUserName = Convert.ToString(dr["ModifiedNormalizedUserName"]);
                if (dr.ColumnIsNotNull("ModifiedDisplayName"))
                    ModifiedBy.DisplayName = Convert.ToString(dr["ModifiedDisplayName"]);
            }

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

        }


    }


    public class EntityUser
    {
        private string _displayName;

        public int Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string DisplayName { get; set; }

    }

}
