using System;

namespace Plato.Reporting.ViewModels
{
    public class ReportIndexViewModel
    {

        public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow.AddDays(-7);

        public DateTimeOffset EndDate { get; set; } = DateTimeOffset.UtcNow;
        
    }

}
