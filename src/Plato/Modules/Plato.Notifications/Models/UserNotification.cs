using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;
using Plato.Internal.Models.Users;

namespace Plato.Notifications.Models
{


    public class UserNotification : IModel<UserNotification>
    {

        public int Id { get; set; }

        public int UserId { get; set; }
        
        public string NotificationName { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }

        public DateTimeOffset? ReadDate { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public ISimpleUser User { get; private set; } = new SimpleUser();

        public ISimpleUser CreatedBy { get; private set; } = new SimpleUser();

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
            {
                UserId = Convert.ToInt32(dr["UserId"]);
                if (UserId > 0)
                {
                    User.Id = UserId;
                    if (dr.ColumnIsNotNull("UserName"))
                        User.UserName = Convert.ToString(dr["UserName"]);
                    if (dr.ColumnIsNotNull("DisplayName"))
                        User.DisplayName = Convert.ToString(dr["DisplayName"]);
                    if (dr.ColumnIsNotNull("FirstName"))
                        User.FirstName = Convert.ToString(dr["FirstName"]);
                    if (dr.ColumnIsNotNull("LastName"))
                        User.LastName = Convert.ToString(dr["LastName"]);
                    if (dr.ColumnIsNotNull("Alias"))
                        User.Alias = Convert.ToString(dr["Alias"]);
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
                ReadDate = DateTimeOffset.Parse(Convert.ToString((dr["ReadDate"])));

            if (dr.ColumnIsNotNull("CreatedUserId"))
            {
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);
                if (CreatedUserId > 0)
                {
                    CreatedBy.Id = CreatedUserId;
                    if (dr.ColumnIsNotNull("CreatedUserName"))
                        CreatedBy.UserName = Convert.ToString(dr["CreatedUserName"]);
                    if (dr.ColumnIsNotNull("CreatedDisplayName"))
                        CreatedBy.DisplayName = Convert.ToString(dr["CreatedDisplayName"]);
                    if (dr.ColumnIsNotNull("CreatedFirstName"))
                        CreatedBy.FirstName = Convert.ToString(dr["CreatedFirstName"]);
                    if (dr.ColumnIsNotNull("CreatedLastName"))
                        CreatedBy.LastName = Convert.ToString(dr["CreatedLastName"]);
                    if (dr.ColumnIsNotNull("CreatedAlias"))
                        CreatedBy.Alias = Convert.ToString(dr["CreatedAlias"]);
                }
            }
                
            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

        }

    }

}
