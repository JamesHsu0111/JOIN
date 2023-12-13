#nullable disable
using System;
using System.Collections.Generic;

namespace MVC_Project.Models
{
    public class ResponseGroup
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public string GroupCategory { get; set; }

        public string GroupContent { get; set; }

        public int? MinAttendee { get; set; }

        public int? MaxAttendee { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int DurationInDays { get; set; }

        public int? Organizer { get; set; }
        public string Nickname { get; set; }

        public int? OriginalActivityID { get; set; }

        public int PersonalPhotoID { get; set; }

        public byte[] PhotoData { get; set; }

        public virtual Group Group { get; set; }
    }

}
