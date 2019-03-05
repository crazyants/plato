using System;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Models
{

    public class ReportSubmission<TModel> where TModel : class
    {

        public IUser Who { get; set; }

        public TModel What { get; set; }

        public ReportReasons.Reason Why { get; set; }
        
        public DateTimeOffset When { get; set; }

        public ReportSubmission()
        {
            When = DateTimeOffset.UtcNow;
        }

    }
    
}
