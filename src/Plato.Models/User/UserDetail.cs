using System;
using System.Data;
using System.Collections.Generic;
using Plato.Models.Annotations;
using Plato.Abstractions.Extensions;

namespace Plato.Models.User
{

    [TableName("Plato_UserDetails")]
    public class UserDetail : IModel<UserDetail>
    {

        #region "Public Properties"

        [PrimaryKey]
        [ColumnName("Id", typeof(int))]
        public int Id { get; set; }

        [ColumnName("UserId", typeof(int))]
        public int UserId { get; set;  }

        [ColumnName("EditionId", typeof(int))]
        public int EditionId { get; set; }

        [ColumnName("Salts")]
        public int RoleId { get; set; }

        public int TeamId { get; set; }

        public double TimeZoneOffSet { get; set;  }

        public bool ObserveDST { get; set;  }

        public string Culture { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string WebSiteUrl { get; set; }

        public string ApiKey { get; set; }

        public int Visits { get; set; }

        public int Answers { get; set; }

        public int Entities { get; set; }

        public int Replies { get; set; }

        public int Reactions { get; set; }

        public int Mentions { get; set; }

        public int Follows { get; set; }

        public int Badges { get; set; }

        public int ReputationRank { get; set; }

        public int ReputationPoints { get; set; }

        public byte[] Banner { get; set; }

        public string ClientIpAddress { get; set; }

        public string ClientName { get; set; }

        public string EmailConfirmationCode { get; set; }

        public string PasswordResetCode { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedUserId { get; set;  }

        public User CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
        
        public int ModifiedUserId { get; set; }

        public User ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public int DeletedUserId { get; set; }

        public User DeletedBy { get; set;  }

        public bool IsBanned { get; set; }

        public DateTime? BannedDate { get; set; }

        public int BannedUserId { get; set; }
        
        public User BannedBy { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? LockedDate { get; set; }

        public int LockedUserId { get; set; }

        public User LockedBy { get; set; }

        public DateTime? UnLockDate { get; set; }

        public bool IsSpam { get; set; }
        
        public DateTime? SpamDate { get; set; }

        public int SpamUserId { get; set; }

        public User SpamBy{ get; set; }

        public DateTime? LastLoginDate { get; set;  }

        #endregion


        #region "Implementation"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                this.Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("EditionId"))
                this.EditionId = Convert.ToInt32(dr["EditionId"]);
            
            if (dr.ColumnIsNotNull("RoleId"))
                this.RoleId = Convert.ToInt32(dr["RoleId"]);

            if (dr.ColumnIsNotNull("TeamId"))
                this.TeamId = Convert.ToInt32(dr["TeamId"]);

            if (dr.ColumnIsNotNull("TimeZoneOffSet"))
                this.TimeZoneOffSet = Convert.ToDouble(dr["TimeZoneOffSet"]);

            if (dr.ColumnIsNotNull("ObserveDST"))
                this.ObserveDST = Convert.ToBoolean(dr["ObserveDST"]);

            if (dr.ColumnIsNotNull("Culture"))
                this.Culture = Convert.ToString(dr["Culture"]);

            if (dr.ColumnIsNotNull("FirstName"))
                this.FirstName = Convert.ToString(dr["FirstName"]);

            if (dr.ColumnIsNotNull("LastName"))
                this.LastName = Convert.ToString(dr["LastName"]);

            if (dr.ColumnIsNotNull("WebSiteUrl"))
                this.WebSiteUrl = Convert.ToString(dr["WebSiteUrl"]);

            if (dr.ColumnIsNotNull("ApiKey"))
                this.ApiKey = Convert.ToString(dr["ApiKey"]);

            if (dr.ColumnIsNotNull("Visits"))
                this.Visits = Convert.ToInt32(dr["Visits"]);

            if (dr.ColumnIsNotNull("Answers"))
                this.Answers = Convert.ToInt32(dr["Answers"]);

            if (dr.ColumnIsNotNull("Entities"))
                this.Entities = Convert.ToInt32(dr["Entities"]);

            if (dr.ColumnIsNotNull("Replies"))
                this.Replies = Convert.ToInt32(dr["Replies"]);

            if (dr.ColumnIsNotNull("Reactions"))
                this.Reactions = Convert.ToInt32(dr["Reactions"]);

            if (dr.ColumnIsNotNull("Mentions"))
                this.Mentions = Convert.ToInt32(dr["Mentions"]);

            if (dr.ColumnIsNotNull("Follows"))
                this.Follows = Convert.ToInt32(dr["Follows"]);

            if (dr.ColumnIsNotNull("Badges"))
                this.Badges = Convert.ToInt32(dr["Badges"]);

            if (dr.ColumnIsNotNull("ReputationRank"))
                this.ReputationRank = Convert.ToInt32(dr["ReputationRank"]);

            if (dr.ColumnIsNotNull("ReputationPoints"))
                this.ReputationPoints = Convert.ToInt32(dr["ReputationPoints"]);

            if (dr.ColumnIsNotNull("Banner"))
                this.Banner = (byte[])(dr["Banner"]);

            if (dr.ColumnIsNotNull("ClientIpAddress"))
                this.ClientIpAddress = Convert.ToString(dr["ClientIpAddress"]);

            if (dr.ColumnIsNotNull("ClientName"))
                this.ClientName = Convert.ToString(dr["ClientName"]);

            if (dr.ColumnIsNotNull("EmailConfirmationCode"))
                this.EmailConfirmationCode = Convert.ToString(dr["EmailConfirmationCode"]);

            if (dr.ColumnIsNotNull("PasswordResetCode"))
                this.PasswordResetCode = Convert.ToString(dr["PasswordResetCode"]);

            if (dr.ColumnIsNotNull("IsEmailConfirmed"))
                this.IsEmailConfirmed = Convert.ToBoolean(dr["IsEmailConfirmed"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                this.CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);

            if (dr.ColumnIsNotNull("CreatedUserId"))
                this.CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("ModifiedDate"))
                this.ModifiedDate = Convert.ToDateTime(dr["ModifiedDate"]);

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                this.ModifiedUserId = Convert.ToInt32(dr["ModifiedUserId"]);
            
            if (dr.ColumnIsNotNull("IsDeleted"))
                this.IsDeleted = Convert.ToBoolean(dr["IsDeleted"]);

            if (dr.ColumnIsNotNull("DeletedDate"))
                this.DeletedDate = Convert.ToDateTime(dr["DeletedDate"]);

            if (dr.ColumnIsNotNull("DeletedUserId"))
                this.DeletedUserId = Convert.ToInt32(dr["DeletedUserId"]);

            if (dr.ColumnIsNotNull("IsBanned"))
                this.IsBanned = Convert.ToBoolean(dr["IsBanned"]);

            if (dr.ColumnIsNotNull("BannedDate"))
                this.BannedDate = Convert.ToDateTime(dr["BannedDate"]);

            if (dr.ColumnIsNotNull("BannedUserId"))
                this.BannedUserId = Convert.ToInt32(dr["BannedUserId"]);

            if (dr.ColumnIsNotNull("IsLocked"))
                this.IsLocked = Convert.ToBoolean(dr["IsLocked"]);

            if (dr.ColumnIsNotNull("LockedDate"))
                this.LockedDate = Convert.ToDateTime(dr["LockedDate"]);

            if (dr.ColumnIsNotNull("LockedUserId"))
                this.LockedUserId = Convert.ToInt32(dr["LockedUserId"]);

            if (dr.ColumnIsNotNull("UnLockDate"))
                this.UnLockDate = Convert.ToDateTime(dr["UnLockDate"]);

            if (dr.ColumnIsNotNull("IsSpam"))
                this.IsSpam = Convert.ToBoolean(dr["IsSpam"]);

            if (dr.ColumnIsNotNull("SpamDate"))
                this.SpamDate = Convert.ToDateTime(dr["SpamDate"]);

            if (dr.ColumnIsNotNull("SpamUserId"))
                this.SpamUserId = Convert.ToInt32(dr["SpamUserId"]);

            if (dr.ColumnIsNotNull("LastLoginDate"))
                this.LastLoginDate = Convert.ToDateTime(dr["LastLoginDate"]);
            

        }

        #endregion




    }

}
