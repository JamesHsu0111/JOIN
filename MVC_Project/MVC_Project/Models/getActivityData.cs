namespace MVC_Project.Models
{
    public class getActivityData
    {
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public string Category { get; set; }
        public decimal? SuggestedAmount { get; set; }
        public string ActivityContent { get; set; }
        public int? MinAttendee { get; set; }
        public DateTime? VoteDate { get; set; }
        public DateTime? ExpectedDepartureMonth { get; set; }
        public List<OfficialPhoto> OfficialPhoto { get; set; } = new List<OfficialPhoto>();
    }
}
