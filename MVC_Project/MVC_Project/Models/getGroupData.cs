using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class getGroupData
    {
        public int GroupID { get; set; }

        [Required(ErrorMessage = "活動名稱為必填項")]
        public string GroupName { get; set; }

        public string GroupCategory { get; set; }

        public string GroupContent { get; set; }

        [Range(3, int.MaxValue, ErrorMessage = "人數下限最少3人")]
        public int? MinAttendee { get; set; }

        public int? MaxAttendee { get; set; }

        [Required(ErrorMessage = "請選擇活動起始日")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "請選擇活動結束日")]
        public DateTime? EndDate { get; set; }

        public int? Organizer { get; set; }

        public int? OriginalActivityID { get; set; }

        public bool HasSent { get; set; }   //10/19新增，確保不會重複寄送轉為"報名中"活動通知
        public int UserID { get; set; }

        public string? Nickname { get; set; }
        public List<PersonalPhoto> PhotoData { get; set; } = new List<PersonalPhoto>();// 將 PhotoData 改為 List<byte[]>
    }


}
