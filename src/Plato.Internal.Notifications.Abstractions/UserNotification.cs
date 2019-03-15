using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Notifications.Abstractions
{
    
    public class UserNotification : IDbModel<UserNotification>
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public ISimpleUser To { get; private set; } = new SimpleUser();

        public string NotificationName { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }

        public DateTimeOffset? ReadDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        
        public ISimpleUser From { get; private set; } = new SimpleUser();

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
            {
                UserId = Convert.ToInt32(dr["UserId"]);
                if (UserId > 0)
                {
                    To.Id = UserId;
                    if (dr.ColumnIsNotNull("UserName"))
                        To.UserName = Convert.ToString(dr["UserName"]);
                    if (dr.ColumnIsNotNull("DisplayName"))
                        To.DisplayName = Convert.ToString(dr["DisplayName"]);
                    if (dr.ColumnIsNotNull("Alias"))
                        To.Alias = Convert.ToString(dr["Alias"]);
                    if (dr.ColumnIsNotNull("PhotoUrl"))
                        To.PhotoUrl = Convert.ToString(dr["PhotoUrl"]);
                    if (dr.ColumnIsNotNull("PhotoColor"))
                        To.PhotoColor = Convert.ToString(dr["PhotoColor"]);
                }
            }
            
            if (dr.ColumnIsNotNull("NotificationName"))
                NotificationName = Convert.ToString(dr["NotificationName"]);

            if (dr.ColumnIsNotNull("Title"))
                Title = Convert.ToString(dr["Title"]);

            if (dr.ColumnIsNotNull("Message"))
                Message = Convert.ToString(dr["Message"]);

            if (dr.ColumnIsNotNull("Url"))
                Url = Convert.ToString(dr["Url"]);

            if (dr.ColumnIsNotNull("Url"))
                Url = Convert.ToString(dr["Url"]);

            if (dr.ColumnIsNotNull("ReadDate"))
                ReadDate = (DateTimeOffset)dr["ReadDate"];

            if (dr.ColumnIsNotNull("CreatedUserId"))
            {
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);
                if (CreatedUserId > 0)
                {
                    From.Id = CreatedUserId;
                    if (dr.ColumnIsNotNull("CreatedUserName"))
                        From.UserName = Convert.ToString(dr["CreatedUserName"]);
                    if (dr.ColumnIsNotNull("CreatedDisplayName"))
                        From.DisplayName = Convert.ToString(dr["CreatedDisplayName"]);
                    if (dr.ColumnIsNotNull("CreatedAlias"))
                        From.Alias = Convert.ToString(dr["CreatedAlias"]);
                    if (dr.ColumnIsNotNull("CreatedPhotoUrl"))
                        From.PhotoUrl = Convert.ToString(dr["CreatedPhotoUrl"]);
                    if (dr.ColumnIsNotNull("CreatedPhotoColor"))
                        From.PhotoColor = Convert.ToString(dr["CreatedPhotoColor"]);
                }
            }
                
            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }

    }

}
