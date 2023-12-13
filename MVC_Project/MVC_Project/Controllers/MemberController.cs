using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using SmartBreadcrumbs.Attributes;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MVC_Project.Controllers

{

	public class MemberController : Controller
	{
		private readonly ProjectXContext _context;

		public MemberController(ProjectXContext context)
		{
			_context = context;
		}

		[Breadcrumb("已報名活動", FromAction = nameof(Member))]
		public IActionResult Registration(int page = 1)
		{
			int pageSize = 6; // 每頁顯示的項目數
			//var userId = 1; // 從當前登錄的使用者取得UserId

            // 從Session取得當前登錄的用戶ID
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out var userId))
            {
                TempData["ErrorMessage"] = "未登錄或Session已過期";
                return RedirectToAction("Login", "Home"); // Redirect to login page or any error page
            }



            // 根據當前頁數和每頁顯示的項目數計算跳過的項目數
            int skip = (page - 1) * pageSize;

            //計算總頁數
            int totalRecords = _context.Registration.Where(r => r.ParticipantID == userId).Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var registeredGroups = _context.Registration
				.Where(r => r.ParticipantID == userId)
				.Include(r => r.Group)
				.Select(r => new MemberUseViewModel
				{
					RegistrationID = r.RegistrationID,
					GroupName = r.Group.GroupName,
					EndDate = r.Group.EndDate,
                    GroupID=r.Group.GroupID
				})
				.Skip(skip)
				.Take(pageSize)
				.ToList();

			// 計算是否有下一頁的邏輯
			bool hasNextPage = _context.Registration
				.Where(r => r.ParticipantID == userId)
				.Skip(skip + pageSize)
				.Any();

			ViewBag.RegisteredGroups = registeredGroups;
			ViewBag.PageNumber = page;
			ViewBag.HasNextPage = hasNextPage;
            ViewBag.TotalPages = totalPages;
            return View();
		}


		////public IActionResult Registration(Group group)
		////{


		////    var userId = 1; // 從當前登錄的使用者取得UserId

		////    var registeredGroups = _context.Registration
		////        .Where(r => r.ParticipantID == userId)
		////        .Include(r => r.Group)
		////        .Select(r => new MemberUseViewModel
		////        {
		////            RegistrationID = r.RegistrationID,
		////            GroupName = r.Group.GroupName,
		////            EndDate = r.Group.EndDate
		////        })
		////        .ToList();
		////    ViewBag.registeredGroups = registeredGroups;
		////    return View();

		////}
       



		[HttpPost]
		public IActionResult DeleteRegistration(int RegistrationID)
		{
			var record = _context.Registration.Find(RegistrationID);
			if (record != null)
			{

				_context.Registration.Remove(record);
				_context.SaveChanges();
			}
			return RedirectToAction("Registration");
		}


		[Breadcrumb("已收藏活動", FromAction = nameof(Member))]
		public IActionResult LikeRecord(int page = 1)
		{
          

            int pageSize = 6; // 每頁顯示的項目數
			//var userId = 1; // 從當前登錄的使用者取得UserId

            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out var userId))
            {
                TempData["ErrorMessage"] = "未登錄或Session已過期";
                return RedirectToAction("Login", "Home"); // Redirect to login page or any error page
            }
            var currentDate = DateTime.Now; // 取得當前日期

			// 根據當前頁數和每頁顯示的項目數計算跳過的項目數
			int skip = (page - 1) * pageSize;

			//計算總頁數
            int totalRecords = _context.LikeRecord.Where(lr => lr.UserID == userId).Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            // 取得這個UserId的LikeRecords以及相對應的MyActivity資料，根據分頁參數進行分頁
            var likedActivities = _context.LikeRecord
                .Where(lr => lr.UserID == userId)
                .Include(lr => lr.Activity)
                .Select(lr => new MemberUseViewModel
                {
                    
                    LikeRecordID = lr.LikeRecordID,  //為了刪除
                    ActivityID = lr.ActivityID,
                    ActivityName = lr.Activity.ActivityName,
                    VoteDate = lr.Activity.VoteDate,
                    // 判斷是否可以編輯
                    CanEdit = currentDate >= lr.Activity.VoteDate && currentDate <= lr.Activity.VoteDate.Value.AddDays(5)
                })
				.Skip(skip)
				.Take(pageSize)
				.ToList();

			// 計算是否有下一頁的邏輯
			bool hasNextPage = _context.LikeRecord
				.Where(lr => lr.UserID == userId)
				.Skip(skip + pageSize)
				.Any();

			ViewBag.LikedActivities = likedActivities;
			ViewBag.PageNumber = page;
            ViewBag.UserId = userId;
            
            ViewBag.TotalPages = totalPages;

            return View();
		}

       // 測試郁倫那邊
        public IActionResult EditActivity(int userId, int activityId)
        {
            Console.WriteLine($"UserId: {userId}, ActivityId: {activityId}"); 
            return View();
        }


        //管理員
        public IActionResult Admin()
        {
            return View();
        }

        [HttpGet("/api/getChatData")]
        public IActionResult getChatData()
        {
            var temp = from c in _context.Chat
                       select new adminChat {
                           ChatID = c.ChatID,
                           ActivityID = c.ActivityID,
                           UserID = c.UserID,
                           ChatContent = c.ChatContent
                       };
            return Ok(temp);
        }

        [HttpGet("/api/getGroupData")]
        public IActionResult getGroupData()
        {
            var temp = from g in _context.Group
                       select g;
            return Ok(temp);
        }

        [HttpGet("/api/getActivityData")]
        public IActionResult getActivityData()
        {
            var temp = from m in _context.MyActivity
                       select m;
            return Ok(temp);
        }

        //public IActionResult LikeRecord()
        //{
        //    var userId = 1; // 從當前登錄的使用者取得UserId

        //    var currentDate = DateTime.Now; // 取得當前日期

        //    // 取得這個UserId所有的LikeRecords以及相對應的MyActivity資料
        //    var likedActivities = _context.LikeRecord
        //   .Where(lr => lr.UserID == userId)
        //   .Include(lr => lr.Activity)
        //   .Select(lr => new MemberUseViewModel
        //   {
        //       LikeRecordID = lr.LikeRecordID,  //為了刪除
        //       ActivityName = lr.Activity.ActivityName,
        //       VoteDate = lr.Activity.VoteDate,
        //       // 判斷是否可以編輯
        //       CanEdit = currentDate >= lr.Activity.VoteDate && currentDate <= lr.Activity.VoteDate.Value.AddDays(5)
        //   })
        //   .ToList();

        //    ViewBag.LikedActivities = likedActivities;

        //    return View();
        //}




        [HttpPost]
		public IActionResult DeleteLikeRecord(int likeRecordID)
		{
			var record = _context.LikeRecord.Find(likeRecordID);
			if (record != null)
			{
				_context.LikeRecord.Remove(record);
				_context.SaveChanges();
			}
			return RedirectToAction("LikeRecord");
		}

		[Breadcrumb("個人揪團", FromAction = nameof(Member))]
		public IActionResult MyGroups(int page = 1)
		{
			int pageSize = 6;
			//var userId = 1; // 從當前登錄的使用者取得UserId

            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out var userId))
            {
                TempData["ErrorMessage"] = "未登錄或Session已過期";
                return RedirectToAction("Login", "Home"); // Redirect to login page or any error page
            }
            int skip = (page - 1) * pageSize;

			int totalRecords = _context.Group.Where(g => g.Organizer == userId).Count();
			int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

			var organizedGroups = _context.Group
				.Where(g => g.Organizer == userId)
				.Select(g => new MemberUseViewModel
				{
                    GroupID = g.GroupID,
                    GroupName = g.GroupName,
					EndDate = g.EndDate
				})
				.Skip(skip)
				.Take(pageSize)
				.ToList();

			ViewBag.OrganizedGroups = organizedGroups;
			ViewBag.PageNumber = page;
			ViewBag.TotalPages = totalPages;
			return View();
		}


        [HttpPost]
        public IActionResult DeleteGroup(int GroupID)
        {
            var response = new { success = false, message = string.Empty }; // 初始化時包含所有屬性


            var group = _context.Group.Find(GroupID);
           /* var currentUserId = 1;*/ // 從Session或其他地方獲取當前登錄的用戶ID

            if (group != null)
            {
                // 刪除與此活動相關的Chat紀錄
                var chats = _context.Chat.Where(c => c.ActivityID == GroupID).ToList();
                _context.Chat.RemoveRange(chats);

                // 刪除與此活動相關的Registration紀錄
                var registrations = _context.Registration.Where(r => r.GroupID == GroupID).ToList();
                _context.Registration.RemoveRange(registrations);


				//刪除相關的照片
                var PersonalPhoto = _context.PersonalPhoto.Where(r => r.GroupID == GroupID).ToList();
                _context.PersonalPhoto.RemoveRange(PersonalPhoto);

                // 刪除活動
                _context.Group.Remove(group);
                _context.SaveChanges();

                response = new { success = true, message = "成功刪除活動" };
            }
            else
            {
                response = new { success = false, message = "找不到該活動" };
            }

            return Json(response);
        }



        [Breadcrumb("會員中心", FromAction = nameof(MyActivityController.HomePage), FromController = typeof(MyActivityController))]
        public IActionResult Member()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out var userId))
            {
                TempData["ErrorMessage"] = "未登錄或Session已過期";
                return RedirectToAction("Login", "Home"); // Redirect to login page or any error page
            }

            var member = _context.Member.FirstOrDefault(m => m.UserID == userId); // 或者是其他相對應的查詢方式

            if (member == null)
            {
                // 你可以選擇返回一個錯誤視圖或其他方式來處理這種情況
                return View("Error");
            }
            return View(member);  // 確保你傳遞了一個非null的對象
        }


        // (password hashing) 之後可能要做密碼hashing，資料庫新增一個欄位來存儲鹽值（Salt）--->我在想把phone用掉改成salt欄位
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out var memberId))
            {
                TempData["ErrorMessage"] = "未登錄或Session已過期";
                return RedirectToAction("Login", "Home");
            }

            var member = _context.Member.Find(memberId);
            if (member == null)
            {
                return View("Error", "未找到會員資料");
            }

            // 驗證舊密碼
            string hashedOldPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: oldPassword,
                salt: member.Salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (hashedOldPassword != member.PasswordHash)
            {
                TempData["PasswordChangeError"] = "舊密碼不正確";
                return RedirectToAction("Member");
            }

            if (newPassword != confirmPassword)
            {
                TempData["PasswordChangeError"] = "新密碼與確認密碼不符合";
                return RedirectToAction("Member");
            }

            // 哈希新密碼
            string hashedNewPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: newPassword,
                salt: member.Salt,  // 通常新密碼應該使用新的Salt，但為了簡單起見在這裡我們用了舊的Salt
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            member.PasswordHash = hashedNewPassword;
            _context.SaveChanges();

            TempData["PasswordChangeSuccess"] = "密碼已成功更改";
            return RedirectToAction("Member");
        }

        [HttpPost]
        public IActionResult UpdateProfile(string Nickname, string Intro)
        {
            //int userId = 1;  // 此處應根據當前登錄的用戶設置

            var userIdString = HttpContext.Session.GetString("UserId");

            if (!int.TryParse(userIdString, out var userId))
            {
                TempData["ErrorMessage"] = "未登錄或Session已過期";
                return RedirectToAction("Login", "Home"); // Redirect to login page or any error page
            }

            var member = _context.Member.FirstOrDefault(m => m.UserID == userId);
            if (member != null)
            {
                member.Nickname = Nickname;
                member.Intro = Intro;
                _context.SaveChanges();
                TempData["ProfileUpdateSuccess"] = "變更成功";
                return RedirectToAction("Member");  // 返回Member頁面
            }
            return View();  // 或者返回錯誤頁面，如果你有的話
        }

        [HttpPost]
		public IActionResult UpdateUserPhoto(int userId, string imageBase64)
		{
			var member = _context.Member.FirstOrDefault(m => m.UserID == userId);
			if (member != null && !string.IsNullOrEmpty(imageBase64))
			{
				byte[] photoBytes = Convert.FromBase64String(imageBase64.Split(',')[1]);
				member.UserPhoto = photoBytes;
				_context.SaveChanges();
				return Json(new { success = true });
			}
			return Json(new { success = false, message = "Failed to update photo" });
		}

		[HttpGet]
		public IActionResult GetUserPhoto(int userId)
		{
			var member = _context.Member.FirstOrDefault(m => m.UserID == userId);
			if (member != null && member.UserPhoto != null)
			{
				string imageBase64Data = Convert.ToBase64String(member.UserPhoto);
				string imageDataURL = $"data:image/jpg;base64,{imageBase64Data}";
				return Json(new { imageUrl = imageDataURL });
			}
			return Json(new { imageUrl = "" }); // 或者返回預設照片的URL
		}


        //抓給group的照片那些
        [HttpGet]
        public IActionResult GetOrganizerInfo(int userId)
        {

            var member = _context.Member.FirstOrDefault(m => m.UserID == userId);
            if (member != null)
            {
                string imageBase64Data = member.UserPhoto != null ? Convert.ToBase64String(member.UserPhoto) : "";
                string imageDataURL = $"data:image/jpg;base64,{imageBase64Data}";
                return Json(new
                {
                    Nickname = member.Nickname,
                    Intro = member.Intro,
                    imageUrl = imageDataURL
                });
            }
            return Json(new { success = false, message = "User not found" });
        }


    }
	/*var member = GetCurrentUser();*/ // 這裡需要您自己實現獲取當前用戶的方法
}

