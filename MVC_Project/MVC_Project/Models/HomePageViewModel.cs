namespace MVC_Project.Models
{
    public class HomePageViewModel
    {
        public List<ResponseActivity> Activities { get; set; }
        public List<ResponseGroup> Groups { get; set; }
        public int TotalPages { get; set; } // 總頁數
        public int CurrentPage { get; set; } // 當前頁數
    }

  
}