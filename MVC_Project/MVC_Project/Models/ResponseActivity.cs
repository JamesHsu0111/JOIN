#nullable disable
using System;
using System.Collections.Generic;

namespace MVC_Project.Models
{
    public class ResponseActivity
    {
        public int ActivityID { get; set; }

        public string ActivityName { get; set; }

        public string Category { get; set; }

        public decimal? SuggestedAmount { get; set; }

        public string ActivityContent { get; set; }

        public int? MinAttendee { get; set; }

        public DateTime? VoteDate { get; set; }


        public DateTime? ExpectedDepartureMonth { get; set; }


        public int OfficialPhotoID { get; set; }


        public string PhotoPath { get; set; }

        public virtual MyActivity Activity { get; set; }
    }
}
