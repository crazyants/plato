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
    
    public class User : IdentityUser<int>,
        IUser,
        IUserMetaData<UserData>,
        IModel<User>
    {


        private readonly ConcurrentDictionary<Type, ISerializable> _metaData;

        private string _displayName;

        #region "Public Properties"

        public int PrimaryRoleId { get; set; }

        public string DisplayName
        {
            get => string.IsNullOrWhiteSpace(_displayName) ? this.UserName : _displayName;
            set => _displayName = value;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Alias { get; set; }

        public string PhotoUrl { get; set; }

        public string PhotoColor { get; set; }

        public UserAvatar Avatar => new UserAvatar(this);

        public string SamAccountName { get; set; }

        public string ResetToken { get; set; }

        public string ConfirmationToken { get; set; }

        public string ApiKey { get; set; }

        public string Password { get; set; }
        
        public DateTimeOffset? PasswordExpiryDate { get; set; }

        public DateTimeOffset? PasswordUpdatedDate { get; set; }

        public string TimeZone { get; set; }

        public bool ObserveDst { get; set; }

        public string Culture { get; set; }

        public string Theme { get; set; }

        public string IpV4Address { get; set; }

        public string IpV6Address { get; set; }

        public int Visits { get; set; }

        public DateTimeOffset? VisitsUpdatedDate { get; set; }

        public int MinutesActive { get; set; }

        public DateTimeOffset? MinutesActiveUpdatedDate { get; set; }

        public int Reputation { get; set; }

        public DateTimeOffset? ReputationUpdatedDate { get; set; }
        
        public int Rank { get; set; }

        public DateTimeOffset? RankUpdatedDate { get; set; }

        public string Signature { get; set; }

        public bool IsSpam { get; set; }

        public bool IsVerified { get; set; }

        public bool BanEnabled { get; set; }
        
        public DateTimeOffset? BanExpiryDate { get; set; }
        
        public UserType UserType { get; set; } = UserType.None;

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int ModifiedUserId { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }
        
        public DateTimeOffset? LastLoginDate { get; set; }

        public IEnumerable<string> RoleNames { get; set; } = new List<string>();
        
        public IEnumerable<Role> UserRoles { get; set; } = new List<Role>();

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
                _metaData.TryUpdate(typeof(T), (ISerializable) obj, _metaData[typeof(T)]);
            }
            else
            {
                _metaData.TryAdd(typeof(T), (ISerializable) obj);
            }
        }

        public void AddOrUpdate(Type type, ISerializable obj)
        {
            if (_metaData.ContainsKey(type))
            {
                _metaData.TryUpdate(type, (ISerializable) obj, _metaData[type]);
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
                return (T) _metaData[typeof(T)];
            }

            return ActivateInstanceOf<T>.Instance();

        }

        #endregion

        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("PrimaryRoleId"))
                PrimaryRoleId = Convert.ToInt32(dr["PrimaryRoleId"]);

            if (dr.ColumnIsNotNull("UserName"))
                UserName = Convert.ToString(dr["UserName"]);

            if (dr.ColumnIsNotNull("NormalizedUserName"))
                NormalizedUserName = Convert.ToString(dr["NormalizedUserName"]);

            if (dr.ColumnIsNotNull("Email"))
                Email = Convert.ToString(dr["Email"]);

            if (dr.ColumnIsNotNull("NormalizedEmail"))
                NormalizedEmail = Convert.ToString(dr["NormalizedEmail"]);

            if (dr.ColumnIsNotNull("EmailConfirmed"))
                EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]);

            if (dr.ColumnIsNotNull("DisplayName"))
                DisplayName = Convert.ToString(dr["DisplayName"]);

            if (dr.ColumnIsNotNull("FirstName"))
                FirstName = Convert.ToString(dr["FirstName"]);

            if (dr.ColumnIsNotNull("LastName"))
                LastName = Convert.ToString(dr["LastName"]);

            if (dr.ColumnIsNotNull("Alias"))
                Alias = Convert.ToString(dr["Alias"]);

            if (dr.ColumnIsNotNull("PhotoUrl"))
                PhotoUrl = Convert.ToString(dr["PhotoUrl"]);
            
            if (dr.ColumnIsNotNull("PhotoColor"))
                PhotoColor = Convert.ToString(dr["PhotoColor"]);

            if (dr.ColumnIsNotNull("SamAccountName"))
                SamAccountName = Convert.ToString(dr["SamAccountName"]);

            if (dr.ColumnIsNotNull("PasswordHash"))
                PasswordHash = Convert.ToString(dr["PasswordHash"]);

            if (dr.ColumnIsNotNull("PasswordExpiryDate"))
                PasswordExpiryDate = (DateTimeOffset)dr["PasswordExpiryDate"];

            if (dr.ColumnIsNotNull("PasswordUpdatedDate"))
                PasswordUpdatedDate = (DateTimeOffset)dr["PasswordUpdatedDate"];

            if (dr.ColumnIsNotNull("SecurityStamp"))
                SecurityStamp = Convert.ToString(dr["SecurityStamp"]);
            
            if (dr.ColumnIsNotNull("ResetToken"))
                ResetToken = Convert.ToString(dr["ResetToken"]);

            if (dr.ColumnIsNotNull("ConfirmationToken"))
                ConfirmationToken = Convert.ToString(dr["ConfirmationToken"]);

            if (dr.ColumnIsNotNull("PhoneNumber"))
                PhoneNumber = Convert.ToString(dr["PhoneNumber"]);

            if (dr.ColumnIsNotNull("PhoneNumberConfirmed"))
                PhoneNumberConfirmed = Convert.ToBoolean(dr["PhoneNumberConfirmed"]);

            if (dr.ColumnIsNotNull("TwoFactorEnabled"))
                TwoFactorEnabled = Convert.ToBoolean(dr["TwoFactorEnabled"]);

            if (dr.ColumnIsNotNull("LockoutEnd"))
                LockoutEnd = Convert.ToDateTime(dr["LockoutEnd"]);

            if (dr.ColumnIsNotNull("LockoutEnabled"))
                LockoutEnabled = Convert.ToBoolean(dr["LockoutEnabled"]);

            if (dr.ColumnIsNotNull("AccessFailedCount"))
                AccessFailedCount = Convert.ToInt32(dr["AccessFailedCount"]);

            if (dr.ColumnIsNotNull("ApiKey"))
                ApiKey = Convert.ToString(dr["ApiKey"]);
            
            if (dr.ColumnIsNotNull("TimeZone"))
                TimeZone = Convert.ToString(dr["TimeZone"]);

            if (dr.ColumnIsNotNull("ObserveDst"))
                ObserveDst = Convert.ToBoolean(dr["ObserveDst"]);

            if (dr.ColumnIsNotNull("Culture"))
                Culture = Convert.ToString(dr["Culture"]);

            if (dr.ColumnIsNotNull("Theme"))
                Theme = Convert.ToString(dr["Theme"]);

            if (dr.ColumnIsNotNull("IpV4Address"))
                IpV4Address = Convert.ToString(dr["IpV4Address"]);

            if (dr.ColumnIsNotNull("IpV6Address"))
                IpV6Address = Convert.ToString(dr["IpV6Address"]);

            if (dr.ColumnIsNotNull("Visits"))
                Visits = Convert.ToInt32(dr["Visits"]);

            if (dr.ColumnIsNotNull("VisitsUpdatedDate"))
                VisitsUpdatedDate = (DateTimeOffset)dr["VisitsUpdatedDate"];

            if (dr.ColumnIsNotNull("MinutesActive"))
                MinutesActive = Convert.ToInt32(dr["MinutesActive"]);

            if (dr.ColumnIsNotNull("MinutesActiveUpdatedDate"))
                MinutesActiveUpdatedDate = (DateTimeOffset)dr["MinutesActiveUpdatedDate"];

            if (dr.ColumnIsNotNull("Reputation"))
                Reputation = Convert.ToInt32(dr["Reputation"]);
            
            if (dr.ColumnIsNotNull("ReputationUpdatedDate"))
                ReputationUpdatedDate = (DateTimeOffset)dr["ReputationUpdatedDate"];

            if (dr.ColumnIsNotNull("Rank"))
                Rank = Convert.ToInt32(dr["Rank"]);

            if (dr.ColumnIsNotNull("RankUpdatedDate"))
                RankUpdatedDate = (DateTimeOffset)dr["RankUpdatedDate"];

            if (dr.ColumnIsNotNull("Signature"))
                Signature = Convert.ToString(dr["Signature"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("IsVerified"))
                IsVerified = Convert.ToBoolean(dr["IsVerified"]);

            if (dr.ColumnIsNotNull("BanEnabled"))
                BanEnabled = Convert.ToBoolean(dr["BanEnabled"]);

            if (dr.ColumnIsNotNull("BanExpiryDate"))
                BanExpiryDate = (DateTimeOffset)dr["BanExpiryDate"];
            
            if (dr.ColumnIsNotNull("UserType"))
                UserType = (UserType)Convert.ToInt16(dr["UserType"]);
            
            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = (DateTimeOffset)dr["ModifiedDate"];

            if (dr.ColumnIsNotNull("LastLoginDate"))
                LastLoginDate = (DateTimeOffset)dr["LastLoginDate"];

        }
        
        public override string ToString()
        {
            return this.UserName;
        }

        #endregion

    }

}
