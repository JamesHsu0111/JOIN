using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;

namespace MVC_Project.Controllers
{
    public class ActivityController : Controller
    {
        private ProjectXContext _context;

        public ActivityController(ProjectXContext context)
        {
            _context = context;
        }

        // GET: ActivityController
        public IActionResult Index( int id)
        {
            //假設會員ID
            int? account = HttpContext.Session.GetString("UserId") != null ?
                int.Parse(HttpContext.Session.GetString("UserId")) :
                (int?)null;
            if (account == null)
            {
                ViewBag.likedActivity = "noReturn";
            }
            else {
                var likedActivity = _context.LikeRecord
                .Where(lr => lr.UserID == account && lr.ActivityID == id)
                .Select(lr => lr.ActivityID)
                .ToList();
                ViewBag.likedActivity = (likedActivity.Count() == 0 ? "false" : "true");
            }
            


            if (id == null || _context.MyActivity == null)
            {
                return NotFound();
            }
            var data =
            from m in _context.MyActivity
            join o in _context.OfficialPhoto
            on m.ActivityID equals o.ActivityID
            where m.ActivityID == id
            select new responseActivity
            {
                ActivityID = m.ActivityID,
                ActivityName = m.ActivityName,
                Category = m.Category,
                SuggestedAmount = m.SuggestedAmount,
                ActivityContent = m.ActivityContent,
                MinAttendee = m.MinAttendee,
                VoteDate = m.VoteDate,
                ExpectedDepartureMonth = m.ExpectedDepartureMonth,
                PhotoPath = o.PhotoPath
            };

            //-----麵包屑-----

            var childNode1 = new MvcBreadcrumbNode("ACT", "MyActivity", "所有活動");

            var childNode2 = new MvcBreadcrumbNode("ACT", "MyActivity", "ViewData.Category")
            {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode1
            };

            var childNode3 = new MvcBreadcrumbNode("ACT", "MyActivity", "ViewData.ActivityName")
            {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode2
            };

            ViewData["BreadcrumbNode"] = childNode3;

            //-----麵包屑結束-----
            

            var temp = data.ToList();

            return View(temp);
        }


        // POST: ActivityController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LikeActivity(int activityId, int userId)
        {
            
                var newLikeRecord = new LikeRecord
                {
                    ActivityID = activityId,
                    UserID = userId
                };

                _context.LikeRecord.Add(newLikeRecord);
                _context.SaveChanges();

                // 返回成功的回應，例如JSON對象
                return Json(new { success = true });
            
        }

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

        
       
    }
}
