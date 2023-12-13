using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.VisualBasic;
using MVC_Project.Models;
using NuGet.Protocol;
using SmartBreadcrumbs.Nodes;

namespace MVC_Project.Controllers
{
    public class groupPageController : Controller
    {
        private ProjectXContext _context;
        // GET: groupPageController
        public groupPageController(ProjectXContext context)
        {
            _context = context;
        }




        public IActionResult groupPage(int id)
        {
            int? account = HttpContext.Session.GetString("UserId") != null ?
                int.Parse(HttpContext.Session.GetString("UserId")) :
                (int?)null;
            if (account == null)
            {
                ViewBag.signOrNot = "noAccount";
            }
            else
            {
                var userInfo = from m in _context.Member
                               where m.UserID == account
                               select m;
                ViewBag.userInfo = userInfo.ToList();


                var signOrNot = _context.Registration
                    .Where(lr => lr.ParticipantID == account && lr.GroupID == id)
                    .Select(lr => lr.RegistrationID)
                    .ToList();
                ViewBag.signOrNot = (signOrNot.Count() == 0 ? "false" : "true");
            }
            var data = from g in _context.Group
                       where g.GroupID == id
                       select new Group
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
                           Chat = (_context.Chat.Count(chat => chat.ActivityID == g.GroupID) > 0) ?
                                                        _context.Chat.Where(chat => chat.ActivityID == g.GroupID)
                                                        .Include(chat => chat.User)  // Include Member data related to Chat
                                                        .ToList() : new List<Chat>(),
                           OriginalActivity = _context.MyActivity.FirstOrDefault(a => a.ActivityID == g.OriginalActivityID),
                           PersonalPhoto = _context.PersonalPhoto.Where(pp => pp.GroupID == g.GroupID).ToList(),
                           Registration = _context.Registration.Where(r => r.GroupID == g.GroupID).ToList()
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

        //主揪資訊
        [HttpGet("api/Organizer/{id}")]
        public IActionResult Organizer(int id) {
            var OrganizerData = from g in _context.Group
                                join m in _context.Member
                                on g.Organizer equals m.UserID
                                where g.GroupID == id
                                select m;
            return Ok(OrganizerData);
        }

        //留言API
        [HttpGet("api/chatData/{id}")]
        public IActionResult chatData(int id)
        {
            string dateFormat = "yyyy-MM-dd HH:mm";
            var chatData = from c in _context.Chat
                           join m in _context.Member on c.UserID equals m.UserID
                           where c.ActivityID == id
                           orderby c.ChatID /*descending*/ // 添加排序操作，降序排序
                           select new responeChat
                           {
                               ChatID = c.ChatID,
                               ActivityID = c.ActivityID,
                               UserID = c.UserID,
                               ChatContent = c.ChatContent,
                               ToWhom = c.ToWhom,
                               ChatTime = c.ChatTime,
                               Nickname = m.Nickname,
                               UserPhoto = m.UserPhoto
                           };

            return Ok(chatData);
        }

        [HttpGet("api/getUserInfo/{currentUserId}")]
        public IActionResult getUserInfo(int currentUserId)
        {
            var userInfo = from m in _context.Member
                           where m.UserID == currentUserId
                           select m;

            return Ok(userInfo);
        }


        //討論上傳API
        [HttpPost]
        [Route("api/discussUpdate")]
        public async Task<IActionResult> discussUpdate([FromBody] Chat chat)
        {
            chat.ChatTime = DateTime.Now;
            _context.Add(chat);
            await _context.SaveChangesAsync();

            return Ok(chat);
        }

        //留言上傳API
        [HttpPost]
        [Route("api/replyUpdate")]
        public async Task<IActionResult> replyUpdate([FromBody] Chat chat)
        {
            chat.ChatTime = DateTime.Now;
            _context.Add(chat);
            await _context.SaveChangesAsync();

            return Ok(chat);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentCreate(Chat chat)
        {
            int id = int.Parse(Request.Form["id"]);
            chat.ActivityID = id;
            chat.ToWhom = null;
            chat.ChatTime = DateTime.Now;

            _context.Add(chat);
            await _context.SaveChangesAsync();

            return RedirectToAction("groupPage", new { id });
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyCreate(Chat chat)
        {
            int id = int.Parse(Request.Form["id"]);
            chat.ActivityID = id;
            chat.ChatTime = DateTime.Now;

            _context.Add(chat);
            await _context.SaveChangesAsync();

            return RedirectToAction("groupPage", new { id });
        }

        [HttpPost]
        public IActionResult register(int groupId, int userId)
        {
            int id = groupId;
            int account = userId;
            var newRegistration = new Registration
            {
                ParticipantID = userId,
                GroupID = groupId
            };

            _context.Registration.Add(newRegistration);
            _context.SaveChanges();

            // 返回成功的回應，例如JSON對象
            return RedirectToAction("groupPage", new { id });
        }

        //活動參加者權限
        [HttpGet("/api/getUserIngroup/{id}")]
        public IActionResult getUserIngroup(int id)
        {
            int? account = HttpContext.Session.GetString("UserId") != null ?
                int.Parse(HttpContext.Session.GetString("UserId")) :
                (int?)null;
            if (account == 7) 
            { 
                return Ok(true); 
            } else {
                var temp = from r in _context.Registration
                           where r.GroupID == id && r.ParticipantID == account
                           select r;
                var UserIngroup = temp.ToList().Count() == 0 ? false : true;
                return Ok(UserIngroup);
            }
            
        }
        //活動參考圖片
        [HttpGet("/api/photoGet/{id}")]
        public IActionResult photoGet(int id)
        {
            var temp = from g in _context.Group
                       join o in _context.OfficialPhoto
                       on g.OriginalActivityID equals o.ActivityID
                       where g.GroupID == id
                       select new photoData
                       {
                           PhotoPath = o.PhotoPath
                       };
            if (temp.Count() == 0 ? false : true)
            {
                return Ok(temp);
            }
            else { return BadRequest(); }

        }
        [HttpGet("/api/registration/{id}")]
        public IActionResult registration(int id)
        {
            var temp = from r in _context.Registration
                       join m in _context.Member
                       on r.ParticipantID equals m.UserID
                       where r.GroupID == id
                       select m;
            return Ok(temp);
                
        }

        //刪除留言
        // DELETE: groupPage/Delete/1
        [HttpPost]
        [Route("/groupPage/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var entityToDelete = _context.Chat.Find(id);
            var repliesToDelete = _context.Chat.Where(chat => chat.ToWhom == id).ToList();

            // 逐个删除记录
            foreach (var replyToDelete in repliesToDelete)
            {
                _context.Chat.Remove(replyToDelete);
            }
            _context.Chat.Remove(entityToDelete);
            _context.SaveChanges();
            return NoContent();
            
        }

        //編輯留言
        [HttpPost]
        [Route("/groupPage/EditChat")]
        public IActionResult EditChat([FromBody] Chat updatedChat)
        {
            var existingChat = _context.Chat.Find(updatedChat.ChatID);

            if (existingChat == null)
            {
                // 如果找不到聊天，返回 NotFound 或其他适当的响应
                return NotFound();
            }

            // 更新原始聊天的内容
            existingChat.ChatTime = DateTime.Now;
            existingChat.ChatContent = updatedChat.ChatContent;

            // 保存更改
            _context.SaveChanges();

            // 返回更新后的聊天
            return NoContent();
        }

        //取消報名 (James 10/21新增)
        public IActionResult DeleteRegistration(int groupId, int userId)
        {
            try
            {
                var registration = _context.Registration
                    .FirstOrDefault(r => r.GroupID == groupId && r.ParticipantID == userId);

                if (registration != null)
                {
                    _context.Registration.Remove(registration);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "找不到要刪除的記錄" });
                }
            }
            catch (Exception ex)
            {
                // 在這裡處理例外情況，例如資料庫錯誤
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
