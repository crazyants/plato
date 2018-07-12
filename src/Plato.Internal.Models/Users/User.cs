using System;
using System.Collections.Concurrent;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Roles;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Users
{
    
    public class User : IdentityUser<int>, IModel<User>
    {
        
        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;

        private string _displayName;

        #region "Public Properties"

        public string DisplayName
        {
            get => string.IsNullOrWhiteSpace(_displayName) ? this.UserName : _displayName;
            set => _displayName = value;
        }

        public int PrimaryRoleId { get; set; }

        public string SamAccountName { get; set;  }
          
        public string ApiKey { get; set; }

        public IEnumerable<string> RoleNames { get; set; } = new List<string>();

        public IEnumerable<Role> UserRoles { get; } = new List<Role>();

        public IDictionary<Type, ISerializable> MetaData => _metaData;
        
        public IEnumerable<UserData> Data { get; set; } = new List<UserData>();

        #endregion

        #region "constructor"

        public User()
        {
            _metaData = new ConcurrentDictionary<Type, ISerializable>();
        }

        #endregion

        #region "Public Methods"
        
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

        #endregion
        
        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("PrimaryRoleId"))
                this.PrimaryRoleId = Convert.ToInt32(dr["PrimaryRoleId"]);

            if (dr.ColumnIsNotNull("UserName"))
                this.UserName = Convert.ToString(dr["UserName"]);

            if (dr.ColumnIsNotNull("NormalizedUserName"))
                this.NormalizedUserName = Convert.ToString(dr["NormalizedUserName"]);
            
            if (dr.ColumnIsNotNull("Email"))
                this.Email = Convert.ToString(dr["Email"]);

            if (dr.ColumnIsNotNull("NormalizedEmail"))
                this.NormalizedEmail = Convert.ToString(dr["NormalizedEmail"]);
            
            if (dr.ColumnIsNotNull("EmailConfirmed"))
                this.EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]);

            if (dr.ColumnIsNotNull("DisplayName"))            
                this.DisplayName = Convert.ToString(dr["DisplayName"]);

            if (dr.ColumnIsNotNull("SamAccountName"))
                this.SamAccountName = Convert.ToString(dr["SamAccountName"]);
       
            if (dr.ColumnIsNotNull("PasswordHash"))
                this.PasswordHash = Convert.ToString(dr["PasswordHash"]);

            if (dr.ColumnIsNotNull("SecurityStamp"))
                this.SecurityStamp = Convert.ToString(dr["SecurityStamp"]);

            if (dr.ColumnIsNotNull("PhoneNumber"))
                this.PhoneNumber = Convert.ToString(dr["PhoneNumber"]);

            if (dr.ColumnIsNotNull("PhoneNumberConfirmed"))
                this.PhoneNumberConfirmed = Convert.ToBoolean(dr["PhoneNumberConfirmed"]);

            if (dr.ColumnIsNotNull("TwoFactorEnabled"))
                this.TwoFactorEnabled = Convert.ToBoolean(dr["TwoFactorEnabled"]);

            if (dr.ColumnIsNotNull("LockoutEnd"))
                this.LockoutEnd = Convert.ToDateTime(dr["LockoutEnd"]);

            if (dr.ColumnIsNotNull("LockoutEnabled"))
                this.LockoutEnabled = Convert.ToBoolean(dr["LockoutEnabled"]);

            if (dr.ColumnIsNotNull("AccessFailedCount"))
                this.AccessFailedCount = Convert.ToInt32(dr["AccessFailedCount"]);
            
            if (dr.ColumnIsNotNull("ApiKey"))
                this.ApiKey = Convert.ToString(dr["ApiKey"]);

        }

        #endregion

    }
       
}
