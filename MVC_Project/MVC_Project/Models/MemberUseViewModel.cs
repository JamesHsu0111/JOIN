namespace MVC_Project.Models
{
    public class MemberUseViewModel
    {
        public int? ActivityID { get; set; }
        public int UserId { get; set; }
        public string? ActivityName { get; set; }
        public DateTime? VoteDate { get; set; }

        public string? GroupName { get; set; }
        public DateTime? EndDate { get; set; }
        public bool CanEdit { get; set; }

       public int LikeRecordID { get; set; } //刪除

        public int RegistrationID { get; set; } // 刪除

        public int? GroupID { get; set; } 

    };
}

