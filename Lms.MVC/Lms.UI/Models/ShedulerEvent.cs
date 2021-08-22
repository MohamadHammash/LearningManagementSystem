using System;

namespace Lms.MVC.UI.Models.ViewModels
{
    public class SchedulerEvent
    {
        public int Id { get; set; }
        public string realId { get; set; }

        public string Title { get; set; }

        public string text { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public string color { get; set; }
        public string textColor { get; set; }
    }
}