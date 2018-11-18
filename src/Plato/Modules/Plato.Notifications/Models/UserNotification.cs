using System;
using System.Data;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models;

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


        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("UserId"))
                UserId = Convert.ToInt32(dr["UserId"]);
            
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
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = DateTimeOffset.Parse(Convert.ToString((dr["CreatedDate"])));

        }

    }

}
