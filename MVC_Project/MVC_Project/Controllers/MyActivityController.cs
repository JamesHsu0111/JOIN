using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using MVC_Project.Models;
using NuGet.ContentModel;
using SmartBreadcrumbs.Attributes;
using static NuGet.Packaging.PackagingConstants;


namespace MVC_Project.Controllers
{
    public class MyActivityController : Controller
    {
        private readonly ProjectXContext _context;
        private readonly object selectedActivityIds;
        private readonly object validActivityIds;

        public string? category { get; private set; }

        public MyActivityController(ProjectXContext context)
        {
            _context = context;
        }

        [DefaultBreadcrumb("首頁")]
        public IActionResult HomePage()
        {
            // 從Session取得當前登錄的用戶ID
            var userIdString = HttpContext.Session.GetString("UserId");

            //檢查是否需要建立並傳送"報名中"活動通知，根據使用者是否有投票而定(並且MyActivity已寫入Group)
            CheckAndUpdateActivities();

            //先在外部宣告viewModel
            HomePageViewModel viewModel = new HomePageViewModel();

            // 讀取所有有效的 ActivityID 到一個列表中
            var validActivityIds = _context.MyActivity.Select(a => a.ActivityID).ToList();

            // 設定要生成的亂數ID數量
            int numberOfRandomIds = Math.Min(3, validActivityIds.Count);

            // 使用亂數生成器來選擇有效的 ActivityID
            Random random = new Random();
            List<int> selectedActivityIds = new List<int>();

            while (selectedActivityIds.Count < numberOfRandomIds)
            {
                int randomId = validActivityIds[random.Next(validActivityIds.Count)];

                if (!selectedActivityIds.Contains(randomId))
                {
                    selectedActivityIds.Add(randomId);
                }
            }


            // 讀取所有有效的 GroupID 到一個列表中
            var validGroupIds = _context.Group.Select(g => g.GroupID).ToList();

            // 設定要生成的亂數ID數量
            int numberOfRandomIds_2 = Math.Min(3, validGroupIds.Count);

            // 使用亂數生成器來選擇有效的 GroupID
            Random random2 = new Random();
            List<int> selectedGroupIds = new List<int>();

            while (selectedGroupIds.Count < numberOfRandomIds_2)
            {
                int randomId = validGroupIds[random.Next(validGroupIds.Count)];

                if (!selectedGroupIds.Contains(randomId))
                {
                    selectedGroupIds.Add(randomId);
                }
            }

            //如果未登入
            if (userIdString == null)
            {

            }
            //如果有登入
            else
            {
                var userId = int.Parse(userIdString!);

                //進行是否建立投票通知判斷
                ProcessLikesAndCreateNotifications(userId);

                //進行是否建立新回覆通知判斷
                CheckRepliesAndCreateNotifications(userId);

                // 處理已點擊愛心的活動，查找LikeRecord資料表
                var likedActivityIds = _context.LikeRecord
                    .Where(lr => lr.UserID == userId && selectedActivityIds.Contains((int)lr.ActivityID))
                    .Select(lr => lr.ActivityID)
                    .ToList();

                // 在前端傳遞已點擊愛心的活動ID，以便在首頁上設置愛心圖示的狀態
                ViewBag.LikedActivityIds = likedActivityIds;
            }

            // 官方活動資料讀取，按 ActivityID 分組，並對相同的 ActivityID 的照片進行隨機排序
            var myActivityData = from m in _context.MyActivity
                                 join o in _context.OfficialPhoto
                                 on m.ActivityID equals o.ActivityID
                                 where selectedActivityIds.Contains(m.ActivityID)
                                 group new { m, o } by m.ActivityID into grouped
                                 select new ResponseActivity
                                 {
                                     ActivityID = grouped.FirstOrDefault().m.ActivityID,
                                     ActivityName = grouped.FirstOrDefault().m.ActivityName,
                                     Category = grouped.FirstOrDefault().m.Category,
                                     SuggestedAmount = grouped.FirstOrDefault().m.SuggestedAmount,
                                     ActivityContent = grouped.FirstOrDefault().m.ActivityContent,
                                     MinAttendee = grouped.FirstOrDefault().m.MinAttendee,
                                     VoteDate = grouped.FirstOrDefault().m.VoteDate,
                                     ExpectedDepartureMonth = grouped.FirstOrDefault().m.ExpectedDepartureMonth,
                                     // 在這裡對相同 ActivityID 的照片進行隨機排序，然後選取第一張照片
                                     PhotoPath = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().o.PhotoPath
                                 };

            //個人開團資料讀取，按 GroupID 分組，並對相同的 GroupID 的照片進行隨機排序
            var groupData = from g in _context.Group
                            join m in _context.Member on g.Organizer equals m.UserID
                            join pp in _context.PersonalPhoto on g.GroupID equals pp.GroupID into personalPhotos
                            from pp in personalPhotos.DefaultIfEmpty()
                            where selectedGroupIds.Contains(g.GroupID)
                            group new { g, pp, m } by g.GroupID into grouped
                            select new ResponseGroup
                            {
                                GroupID = grouped.FirstOrDefault().g.GroupID,
                                GroupName = grouped.FirstOrDefault().g.GroupName,
                                GroupCategory = grouped.FirstOrDefault().g.GroupCategory,
                                GroupContent = grouped.FirstOrDefault().g.GroupContent,
                                MinAttendee = grouped.FirstOrDefault().g.MinAttendee,
                                MaxAttendee = grouped.FirstOrDefault().g.MaxAttendee,
                                StartDate = grouped.FirstOrDefault().g.StartDate,
                                EndDate = grouped.FirstOrDefault().g.EndDate,
                                Nickname = grouped.FirstOrDefault().m.Nickname,
                                // 在這裡對相同 GroupID 的照片進行隨機排序，然後選取第一張照片
                                PhotoData = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().pp.PhotoData
                            };

            var activities = myActivityData.ToList();
            var groups = groupData.ToList();

            //處理PhotoData可能為0的狀況
            foreach (var group in groups)
            {
                if (group.PhotoData == null)
                {
                    group.PhotoData = new byte[0];
                }
            }

            viewModel = new HomePageViewModel
            {
                Activities = activities,
                Groups = groups
            };

            // 計算 DurationInDays 並設定給 ResponseGroup
            foreach (var group in viewModel.Groups)
            {
                TimeSpan? timeSpan = group.EndDate - group.StartDate;
                group.DurationInDays = (int)timeSpan.Value.TotalDays;
            }

            return View(viewModel);
        }

        //新增收藏紀錄
        [HttpPost]
        public IActionResult LikeActivity(int activityId, int userId)
        {
            if (userId == 0)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                // 使用Entity Framework將新的LikeRecord插入到資料庫中
                var newLikeRecord = new LikeRecord
                {
                    ActivityID = activityId,
                    UserID = userId
                };

                _context.LikeRecord.Add(newLikeRecord);
                _context.SaveChanges();
            }

            // 返回成功的回應，例如JSON對象
            return Json(new { success = true });
        }

        //取消收藏紀錄
        [HttpDelete]
        public IActionResult UnlikeActivity(int activityId, int userId)
        {
            // 查找符合指定ActivityId和UserId的LikeRecord記錄
            var likeRecord = _context.LikeRecord.SingleOrDefault(LR => LR.ActivityID == activityId && LR.UserID == userId);

            if (likeRecord != null)
            {
                // 如果找到匹配的記錄，則將其從資料庫中刪除
                _context.LikeRecord.Remove(likeRecord);
                _context.SaveChanges();

                // 返回成功的回應，例如JSON對象
                return Json(new { success = true });
            }

            // 如果找不到匹配的記錄，可以返回一個錯誤或其他適當的回應
            return Json(new { success = false, error = "LikeRecord not found" });
        }


        //處理使用者已收藏活動是否需要寄送投票通知
        public IActionResult ProcessLikesAndCreateNotifications(int userId)
        {
            //使用者收藏紀錄
            var likedActivityIds = _context.LikeRecord
                .Where(lr => lr.UserID == userId)
                .Select(lr => lr.ActivityID)
                .ToList();

            //查找收藏紀錄中符合條件的活動
            var activitiesToSendNotifications = _context.MyActivity
                .Where(a => likedActivityIds.Contains(a.ActivityID) && DateTime.Compare(a.VoteDate.Value.Date, DateTime.Today)==0)
                .ToList();

            // 對符合條件的活動建立通知
            foreach (var activity in activitiesToSendNotifications)
            {
                // 查找所有使用者收藏此活動的記錄
                var usersWhoLikedActivity = _context.LikeRecord
                    .Where(lr => lr.ActivityID == activity.ActivityID)
                    .Select(lr => lr.UserID)
                    .ToList();

                // 檢查 MinAttendee 是否大於所有使用者的收藏數量
                if (activity.MinAttendee.HasValue && usersWhoLikedActivity.Count >= activity.MinAttendee)
                {
                    var notificationContent = $"您參與投票的活動\"{activity.ActivityName}\"已經可以進行投票。";

                    // 檢查通知內容是否已存在於資料庫
                    bool notificationExists = _context.Notification.Any(n =>
                        n.UserID == userId &&
                        n.NotificationContent == notificationContent);

                    if (!notificationExists)
                    {
                        var notification = new Notification
                        {
                            UserID = userId,
                            NotificationContent = notificationContent,
                            IsRead = false,
                            NotificationDate = DateTime.Now,
                            NotificationType = "Vote",
                            NotificationToWhichActivityID = activity.ActivityID,
                        };
                        _context.Notification.Add(notification);
                    }
                }
                else
                {
                    var notificationContent = $"您參與投票的活動\"{activity.ActivityName}\"未達出團人數，已取消您的參與。";
                    var notification = new Notification
                    {
                        UserID = userId,
                        NotificationContent = notificationContent,
                        IsRead = false,
                        NotificationDate = DateTime.Now,
                        NotificationType = "Cancelled",
                        NotificationToWhichActivityID = activity.ActivityID,
                    };
                    _context.Notification.Add(notification);

                    //移除已建立通知之已收藏活動
                    var likeRecordEntry = _context.LikeRecord.SingleOrDefault(lr => lr.UserID == userId && lr.ActivityID == activity.ActivityID);
                    if (likeRecordEntry != null)
                    {
                        _context.LikeRecord.Remove(likeRecordEntry);
                    }
                }

            }

            _context.SaveChanges();

            return RedirectToAction("HomePage");
        }

        //根據目前使用者拿取通知Action
        //userId寫在"Layout.js"的AJAX裡面，讀取從Session讀到value的頁面Input
        [HttpGet]
        public IActionResult GetNotifications(int userId)
        {
            // 從資料庫中獲取未讀通知數目
            int notificationCount = _context.Notification
                .Where(n => n.UserID == userId && !n.IsRead)
                .Count();

            // 從資料庫中獲取通知內容
            var notifications = _context.Notification
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.NotificationDate)
                .ToList();

            var notificationData = new
            {
                notificationCount = notificationCount,
                notifications = notifications
            };

            return Json(notificationData);
        }

        //處理已讀||未讀狀態
        [HttpPost]
        public IActionResult ChangeNotificationReadState(int notificationId, int userId)
        {
            var notification = _context.Notification.FirstOrDefault(n => n.NotificationID == notificationId);

            //如果未讀>>改為已讀
            if (notification != null && notification.IsRead == false)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }

            else
            {
                notification.IsRead = false;
                _context.SaveChanges();
            }

            // 從資料庫中獲取未讀通知數目
            int notificationCount = _context.Notification
                .Where(n => n.UserID == userId && !n.IsRead)
                .Count();

            // 從資料庫中獲取通知內容
            var notifications = _context.Notification
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.NotificationDate)
                .ToList();

            var notificationData = new
            {
                notificationCount = notificationCount,
                notifications = notifications
            };

            return Json(notificationData);
        }

        //處理"全部已讀"狀態
        [HttpPost]
        public IActionResult MarkAllNotificationsAsRead(int userId)
        {
            // 根據用戶ID將所有通知設置為已讀
            var notifications = _context.Notification
                .Where(n => n.UserID == userId && !n.IsRead)
                .ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            _context.SaveChanges();

            // 重新獲取已讀通知數目
            int notificationCount = _context.Notification
                .Where(n => n.UserID == userId &&!n.IsRead)
                .Count();

            // 重新獲取通知內容
            var updatedNotifications = _context.Notification
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.NotificationDate)
                .ToList();

            var notificationData = new
            {
                notificationCount = notificationCount,
                notifications = updatedNotifications
            };

            return Json(notificationData);
        }

        //查找回覆並建立通知
        public IActionResult CheckRepliesAndCreateNotifications(int userId)
        {
            // 找到所有具有回覆的 chat 資料
            var repliedChats = _context.Chat
                .Where(c => c.ToWhom != null)
                .Select(c => new { c.ChatID, c.ToWhom, c.UserID, c.ActivityID, c.ChatTime })
                .ToList();

            foreach (var chat in repliedChats)
            {
                //防呆機制，確保不會傳送自己寫的回覆
                if (chat.UserID != userId)
                {
                    // 首先查找原始留言的 ChatID
                    var originalChat = _context.Chat
                        .Where(c => c.ChatID == chat.ToWhom)
                        .FirstOrDefault();

                    // 檢查該通知是否已存在於資料庫，且具有相同通知內容、回覆者名稱和通知日期晚於找到的 Chat 資料列的 ChatTime，如果有就代表已經處理過了
                    var existingNotification = _context.Notification.FirstOrDefault(n =>
                        n.UserID == userId &&
                        n.NotificationContent == $"{GetUserNameById(chat.UserID)} 已在 \"{GetActivityNameById(chat.ActivityID)}\" 回覆了你的留言" &&
                        n.NotificationDate > chat.ChatTime
                    );

                    // 如果沒有找到，而且UserID與提供的userId相符，則寫入新通知
                    if (existingNotification == null && originalChat?.UserID == userId)
                    {
                        var notification = new Notification
                        {
                            UserID = userId,
                            NotificationContent = $"{GetUserNameById(chat.UserID)} 已在 \"{GetActivityNameById(chat.ActivityID)}\" 回覆了你的留言",
                            IsRead = false,
                            NotificationDate = DateTime.Now,
                            NotificationType = "Reply",
                            NotificationToWhichActivityID = (int)chat.ActivityID
                        };
                        _context.Notification.Add(notification);
                    }
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Homepage");
        }

        // 透過 ActivityID (從Chat資料表來) 獲取 ActivityName (非官方活動)的方法
        private string GetActivityNameById(int? activityId)
        {
            var activity = _context.Group.FirstOrDefault(g => g.GroupID == activityId);
            return activity != null ? activity.GroupName : "Unknown Activity";
        }

        // 透過 UserID (從Chat資料表來) 獲取使用者名稱的方法
        private string GetUserNameById(int? userId)
        {
            var user = _context.Member.FirstOrDefault(u => u.UserID == userId);
            return user != null ? user.Nickname : "Unknown User";
        }

        //查看投票中活動是否轉為"報名中"
        public IActionResult CheckAndUpdateActivities()
        {
            // 找到包含 originalActivityID 的活動
            var activitiesWithOriginalID = _context.Group
                .Where(g => g.OriginalActivityID != null && g.HasSent == false)
                .ToList();

            foreach (var activity in activitiesWithOriginalID)
            {
                // 找到對應的 MyActivity
                var myActivity = _context.MyActivity.FirstOrDefault(a => a.ActivityID == activity.OriginalActivityID);

                if (myActivity != null)
                {
                    // 找到對這個 MyActivity 投票的會員
                    var voters = _context.VoteRecord
                        .Where(vr => vr.ActivityID == myActivity.ActivityID)
                        .Select(vr => vr.UserID)
                        .ToList();

                    // 建立通知並發送給投票的會員
                    foreach (var userId in voters)
                    {
                        var notificationContent = $"您投票的活動\"{myActivity.ActivityName}\"已轉為\"報名中\"，可以開始報名!";
                        var notification = new Notification
                        {
                            UserID = (int)userId,
                            NotificationContent = notificationContent,
                            IsRead = false,
                            NotificationDate = DateTime.Now,
                            NotificationType = "ActivityToGroup",
                            NotificationToWhichActivityID = activity.GroupID,
                        };
                        _context.Notification.Add(notification);
                    }

                    // 設置 HasSent 為 True，避免重複處理
                    activity.HasSent = true;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("HomePage");
        }



        //搜尋框功能
        public IActionResult SearchResult(string searchString)
        {
            var myActivityResults = _context.MyActivity
            .Where(m => m.ActivityName.Contains(searchString))
            .Select(m => new
            {
                ID = m.ActivityID,
                NAME = m.ActivityName,
            })
            .ToList();

            var groupResults = _context.Group
                .Where(g => g.GroupName.Contains(searchString))
                .Select(g => new
                {
                    ID = g.GroupID,
                    NAME = g.GroupName,
                })
            .ToList();

            //把兩個變數合在一起
            var combinedResults = myActivityResults.Concat(groupResults);

            //宣告一個List接查到的資料
            var finalResults = new List<object>();

            foreach (var result in combinedResults)
            {
                var myActivityData = Enumerable.Empty<object>(); // 初始化 myActivityData 變數
                var groupData = Enumerable.Empty<object>(); // 初始化 groupData 變數

                var myActivityResult = _context.MyActivity.FirstOrDefault(m => m.ActivityID == result.ID && m.ActivityName == result.NAME);
                if (myActivityResult != null)
                {
                    myActivityData = (from m in _context.MyActivity
                                     join o in _context.OfficialPhoto on m.ActivityID equals o.ActivityID
                                     where m.ActivityID == myActivityResult.ActivityID
                                     group new { m, o } by m.ActivityID into grouped
                                     select new
                                     {
                                         ActivityID = grouped.FirstOrDefault().m.ActivityID,
                                         ActivityName = grouped.FirstOrDefault().m.ActivityName,
                                         Category = grouped.FirstOrDefault().m.Category,
                                         SuggestedAmount = grouped.FirstOrDefault().m.SuggestedAmount,
                                         ActivityContent = grouped.FirstOrDefault().m.ActivityContent,
                                         MinAttendee = grouped.FirstOrDefault().m.MinAttendee,
                                         VoteDate = grouped.FirstOrDefault().m.VoteDate,
                                         ExpectedDepartureMonth = grouped.FirstOrDefault().m.ExpectedDepartureMonth,
                                         PhotoPath = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().o.PhotoPath
                                     }).ToList();
                }


                var GroupResult = _context.Group.FirstOrDefault(g => g.GroupName == result.NAME && g.GroupID == result.ID);
                if (GroupResult != null)
                {
                    groupData = (from g in _context.Group
                                join m in _context.Member on g.Organizer equals m.UserID
                                join pp in _context.PersonalPhoto on g.GroupID equals pp.GroupID into personalPhotos
                                from pp in personalPhotos.DefaultIfEmpty()
                                where g.GroupID == GroupResult.GroupID
                                group new { g, pp, m } by g.GroupID into grouped
                                select new
                                {
                                    GroupID = grouped.FirstOrDefault().g.GroupID,
                                    GroupName = grouped.FirstOrDefault().g.GroupName,
                                    GroupCategory = grouped.FirstOrDefault().g.GroupCategory,
                                    GroupContent = grouped.FirstOrDefault().g.GroupContent,
                                    MinAttendee = grouped.FirstOrDefault().g.MinAttendee,
                                    MaxAttendee = grouped.FirstOrDefault().g.MaxAttendee,
                                    StartDate = grouped.FirstOrDefault().g.StartDate,
                                    EndDate = grouped.FirstOrDefault().g.EndDate,
                                    Nickname = grouped.FirstOrDefault().m.Nickname,
                                    //Photos = grouped.Select(item => item.pp.PhotoData).ToList() // 收集照片到集合
                                    PhotoData = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().pp.PhotoData
                                }).ToList();
                }
                if (myActivityData.Any())
                {
                    finalResults.Add(myActivityData);
                }
                else
                {
                    finalResults.Add(groupData);
                }
            }

            return Json(finalResults);
        }






        
        //-----------------------------^^^^我的程式碼結束^^^^----------------------------------
        [Breadcrumb("所有活動", FromAction = nameof(MyActivityController.HomePage), FromController = typeof(MyActivityController))]
        public IActionResult ACT(int? page, string getData, string category)
        {

            int pageSize = 9;             // 計算要跳過的項目數量
            int pageNumber = (page ?? 1); // 如果 page 為空，默認為第 1 頁
            pageNumber = pageNumber <= 1 ? 1 : pageNumber;
            var userIdString = HttpContext.Session.GetString("UserId");
            var validActivityIds = _context.MyActivity.Select(a => a.ActivityID).ToList();

            IQueryable<ResponseActivity> myActivityData;
            IQueryable<ResponseGroup> groupData;

            if (getData == "group")
            {
                groupData = (from g in _context.Group
                             join m in _context.Member on g.Organizer equals m.UserID
                             join ma in _context.MyActivity on g.OriginalActivityID equals ma.ActivityID
                             join pp in _context.PersonalPhoto on g.GroupID equals pp.GroupID into personalPhotos
                             from pp in personalPhotos.DefaultIfEmpty()
                             where ma.Category == category
                             group new { g, pp, m } by g.GroupID into grouped
                             select new ResponseGroup
                             {
                                 GroupID = grouped.FirstOrDefault().g.GroupID,
                                 GroupName = grouped.FirstOrDefault().g.GroupName,
                                 GroupCategory = grouped.FirstOrDefault().g.GroupCategory,
                                 GroupContent = grouped.FirstOrDefault().g.GroupContent,
                                 MinAttendee = grouped.FirstOrDefault().g.MinAttendee,
                                 MaxAttendee = grouped.FirstOrDefault().g.MaxAttendee,
                                 StartDate = grouped.FirstOrDefault().g.StartDate,
                                 EndDate = grouped.FirstOrDefault().g.EndDate,
                                 Nickname = grouped.FirstOrDefault().m.Nickname,
                                 // 在這裡對相同 GroupID 的照片進行隨機排序，然後選取第一張照片
                                 PhotoData = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().pp.PhotoData
                             });

                myActivityData = Enumerable.Empty<ResponseActivity>().AsQueryable();
            }
            else
            {
                myActivityData = from m in _context.MyActivity
                                 join o in _context.OfficialPhoto on m.ActivityID equals o.ActivityID
                                 where m.Category == category
                                 group new { m, o } by m.ActivityID into grouped
                                 select new ResponseActivity
                                 {
                                     ActivityID = grouped.FirstOrDefault().m.ActivityID,
                                     ActivityName = grouped.FirstOrDefault().m.ActivityName,
                                     Category = grouped.FirstOrDefault().m.Category,
                                     SuggestedAmount = grouped.FirstOrDefault().m.SuggestedAmount,
                                     ActivityContent = grouped.FirstOrDefault().m.ActivityContent,
                                     MinAttendee = grouped.FirstOrDefault().m.MinAttendee,
                                     VoteDate = grouped.FirstOrDefault().m.VoteDate,
                                     ExpectedDepartureMonth = grouped.FirstOrDefault().m.ExpectedDepartureMonth,
                                     // 在這裡對相同 ActivityID 的照片進行隨機排序，然後選取第一張照片
                                     PhotoPath = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().o.PhotoPath
                                 };
                groupData = Enumerable.Empty<ResponseGroup>().AsQueryable(); 
            }
                // 根据类别进行筛选
                if (!string.IsNullOrEmpty(category))
            {
                myActivityData = from m in _context.MyActivity
                                 join o in _context.OfficialPhoto on m.ActivityID equals o.ActivityID
                                 where m.Category == category
                                 group new { m, o } by m.ActivityID into grouped
                                 select new ResponseActivity
                                 {
                                     ActivityID = grouped.FirstOrDefault().m.ActivityID,
                                     ActivityName = grouped.FirstOrDefault().m.ActivityName,
                                     Category = grouped.FirstOrDefault().m.Category,
                                     SuggestedAmount = grouped.FirstOrDefault().m.SuggestedAmount,
                                     ActivityContent = grouped.FirstOrDefault().m.ActivityContent,
                                     MinAttendee = grouped.FirstOrDefault().m.MinAttendee,
                                     VoteDate = grouped.FirstOrDefault().m.VoteDate,
                                     ExpectedDepartureMonth = grouped.FirstOrDefault().m.ExpectedDepartureMonth,
                                     // 在這裡對相同 ActivityID 的照片進行隨機排序，然後選取第一張照片
                                     PhotoPath = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().o.PhotoPath
                                 };

                //個人開團資料讀取
                groupData = (from g in _context.Group
                             join m in _context.Member on g.Organizer equals m.UserID
                             join ma in _context.MyActivity on g.OriginalActivityID equals ma.ActivityID
                             join pp in _context.PersonalPhoto on g.GroupID equals pp.GroupID into personalPhotos
                             from pp in personalPhotos.DefaultIfEmpty()
                             where ma.Category == category
                             group new { g, pp, m } by g.GroupID into grouped
                             select new ResponseGroup
                             {
                                 GroupID = grouped.FirstOrDefault().g.GroupID,
                                 GroupName = grouped.FirstOrDefault().g.GroupName,
                                 GroupCategory = grouped.FirstOrDefault().g.GroupCategory,
                                 GroupContent = grouped.FirstOrDefault().g.GroupContent,
                                 MinAttendee = grouped.FirstOrDefault().g.MinAttendee,
                                 MaxAttendee = grouped.FirstOrDefault().g.MaxAttendee,
                                 StartDate = grouped.FirstOrDefault().g.StartDate,
                                 EndDate = grouped.FirstOrDefault().g.EndDate,
                                 Nickname = grouped.FirstOrDefault().m.Nickname,
                                 // 在這裡對相同 GroupID 的照片進行隨機排序，然後選取第一張照片
                                 PhotoData = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().pp.PhotoData
                             });
            }
            else
            {
                myActivityData = from m in _context.MyActivity
                                 join o in _context.OfficialPhoto on m.ActivityID equals o.ActivityID
                                 group new { m, o } by m.ActivityID into grouped
                                 select new ResponseActivity
                                 {
                                     ActivityID = grouped.FirstOrDefault().m.ActivityID,
                                     ActivityName = grouped.FirstOrDefault().m.ActivityName,
                                     Category = grouped.FirstOrDefault().m.Category,
                                     SuggestedAmount = grouped.FirstOrDefault().m.SuggestedAmount,
                                     ActivityContent = grouped.FirstOrDefault().m.ActivityContent,
                                     MinAttendee = grouped.FirstOrDefault().m.MinAttendee,
                                     VoteDate = grouped.FirstOrDefault().m.VoteDate,
                                     ExpectedDepartureMonth = grouped.FirstOrDefault().m.ExpectedDepartureMonth,
                                     // 在這裡對相同 ActivityID 的照片進行隨機排序，然後選取第一張照片
                                     PhotoPath = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().o.PhotoPath
                                 };

                groupData = from g in _context.Group
                            join m in _context.Member on g.Organizer equals m.UserID
                            join ma in _context.MyActivity on g.OriginalActivityID equals ma.ActivityID
                            join pp in _context.PersonalPhoto on g.GroupID equals pp.GroupID into personalPhotos
                            from pp in personalPhotos.DefaultIfEmpty()
                            group new { g, pp, m } by g.GroupID into grouped
                            select new ResponseGroup
                            {
                                GroupID = grouped.FirstOrDefault().g.GroupID,
                                GroupName = grouped.FirstOrDefault().g.GroupName,
                                GroupCategory = grouped.FirstOrDefault().g.GroupCategory,
                                GroupContent = grouped.FirstOrDefault().g.GroupContent,
                                MinAttendee = grouped.FirstOrDefault().g.MinAttendee,
                                MaxAttendee = grouped.FirstOrDefault().g.MaxAttendee,
                                StartDate = grouped.FirstOrDefault().g.StartDate,
                                EndDate = grouped.FirstOrDefault().g.EndDate,
                                Nickname = grouped.FirstOrDefault().m.Nickname,
                                // 在這裡對相同 GroupID 的照片進行隨機排序，然後選取第一張照片
                                PhotoData = grouped.OrderBy(x => Guid.NewGuid()).FirstOrDefault().pp.PhotoData
                            };
            }

            //如果未登入
            if (userIdString == null)
            {

            }
            //如果有登入
            else
            {
                var userId = int.Parse(userIdString!);

                //進行是否建立投票通知判斷
                ProcessLikesAndCreateNotifications(userId);

                //進行是否建立新回覆通知判斷
                CheckRepliesAndCreateNotifications(userId);

                // 處理已點擊愛心的活動，查找LikeRecord資料表
                var likedActivityIds = _context.LikeRecord
                    .Where(lr => lr.UserID == userId && validActivityIds.Contains((int)lr.ActivityID))
                    .Select(lr => lr.ActivityID)
                    .ToList();

                // 在前端傳遞已點擊愛心的活動ID，以便在首頁上設置愛心圖示的狀態
                ViewBag.LikedActivityIds = likedActivityIds;
            }


            int totalItems = myActivityData.Count() + groupData.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var myActivityDataList = myActivityData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var groupDataList = groupData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new HomePageViewModel
            {
                Activities = myActivityDataList,
                Groups = groupDataList,
            };

            // 計算 DurationInDays 並設定給 ResponseGroup
            foreach (var group in viewModel.Groups)
            {
                TimeSpan? timeSpan = group.EndDate - group.StartDate;
                group.DurationInDays = (int)timeSpan.Value.TotalDays;
            }
            ViewBag.PageNumber = page;
            ViewBag.TotalPages = totalPages;
            return View(viewModel);
        }

        [HttpGet("api/loadData")]
        public IActionResult loadData()
        {
            var Data = from g in _context.Group
                       join m in _context.Member on g.Organizer equals m.UserID
                       join p in _context.PersonalPhoto on g.GroupID equals p.GroupID into photos
                       select new getGroupData
                       {
                           GroupID = g.GroupID,
                           GroupName = g.GroupName,
                           GroupCategory = g.GroupCategory,
                           GroupContent = g.GroupContent,
                           MinAttendee = g.MinAttendee,
                           MaxAttendee = g.MaxAttendee,
                           StartDate = g.StartDate,
                           EndDate = g.EndDate,
                           Organizer = g.Organizer,
                           OriginalActivityID = g.OriginalActivityID,
                           HasSent = g.HasSent,
                           UserID = m.UserID,
                           Nickname = m.Nickname,
                           PhotoData = photos.ToList()
                       };
            return Ok(Data);
        }



        [HttpGet("api/loadActivityData")]
        public IActionResult loadActivityData()
        {
            var Data = from m in _context.MyActivity
                       join o in _context.OfficialPhoto
                       on m.ActivityID equals o.ActivityID into photos
                       select new getActivityData
                       {
                           ActivityID = m.ActivityID,
                           ActivityName = m.ActivityName,
                           Category = m.Category,
                           SuggestedAmount = m.SuggestedAmount,
                           ActivityContent = m.ActivityContent,
                           MinAttendee = m.MinAttendee,
                           VoteDate = m.VoteDate,
                           ExpectedDepartureMonth = m.ExpectedDepartureMonth,
                           OfficialPhoto = photos.ToList() // 存成 List
                       };
            return Ok(Data);
        }

      

        // ?---------------------------------------------------?

        // GET: MyActivity
        public async Task<IActionResult> Index()
        {
            return _context.MyActivity != null ?
                        View(await _context.MyActivity.ToListAsync()) :
                        Problem("Entity set 'ProjectXContext.MyActivity'  is null.");
        }

        // GET: MyActivity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MyActivity == null)
            {
                return NotFound();
            }

            var myActivity = await _context.MyActivity
                .FirstOrDefaultAsync(m => m.ActivityID == id);
            if (myActivity == null)
            {
                return NotFound();
            }

            return View(myActivity);
        }

        // GET: MyActivity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MyActivity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActivityID,ActivityName,Category,SuggestedAmount,ActivityContent,MinAttendee,VoteDate,ExpectedDepartureMonth")] MyActivity myActivity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(myActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(myActivity);
        }

        // GET: MyActivity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MyActivity == null)
            {
                return NotFound();
            }

            var myActivity = await _context.MyActivity.FindAsync(id);
            if (myActivity == null)
            {
                return NotFound();
            }
            return View(myActivity);
        }

        // POST: MyActivity/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityID,ActivityName,Category,SuggestedAmount,ActivityContent,MinAttendee,VoteDate,ExpectedDepartureMonth")] MyActivity myActivity)
        {
            if (id != myActivity.ActivityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(myActivity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyActivityExists(myActivity.ActivityID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(myActivity);
        }

        // GET: MyActivity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MyActivity == null)
            {
                return NotFound();
            }

            var myActivity = await _context.MyActivity
                .FirstOrDefaultAsync(m => m.ActivityID == id);
            if (myActivity == null)
            {
                return NotFound();
            }

            return View(myActivity);
        }

        // POST: MyActivity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MyActivity == null)
            {
                return Problem("Entity set 'ProjectXContext.MyActivity'  is null.");
            }
            var myActivity = await _context.MyActivity.FindAsync(id);
            if (myActivity != null)
            {
                _context.MyActivity.Remove(myActivity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyActivityExists(int id)
        {
            return (_context.MyActivity?.Any(e => e.ActivityID == id)).GetValueOrDefault();
        }
    }
}
