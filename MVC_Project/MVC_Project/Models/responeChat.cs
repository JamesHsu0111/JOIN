using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models
{
    public class responeChat
    {
        public int ChatID { get; set; }

        public int? ActivityID { get; set; }

        public int? UserID { get; set; }

        public string? ChatContent { get; set; }

        public int? ToWhom { get; set; }

        
        public DateTime? ChatTime { get; set; }

        public string? Nickname { get; set; }

        public byte[]? UserPhoto { get; set; }

    }
}
