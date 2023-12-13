using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Http;

namespace MVC_Project.Controllers
{
    public class VoteRecordsController : Controller
    {
        private readonly ProjectXContext _context;

        public VoteRecordsController(ProjectXContext context)
        {
            _context = context;
        }

        //----------------James寫的-----------------

        //根據活動ID查找要顯示的投票選項
        // GET: /VoteRecords/SelectDate/{ActivityId}
        [Breadcrumb("預設活動投票", FromAction = nameof(MemberController.Member), FromController = typeof(MemberController))]
        public IActionResult SelectDate(int? id)
        {
            //接收來自點擊通知的ActivityID
            var activityId = id;

            //根據 ActivityId 查詢相應的活動投票時間
            var voteTimes = _context.VoteTime
                .Where(vt => vt.ActivityID == activityId)
                .ToList();

            ViewBag.Dates = voteTimes.Select(vt => vt.StartDate).ToList();


            //剩餘天數
            var voteDate = (from m in _context.MyActivity
                            where m.ActivityID == activityId
                            select m.VoteDate).FirstOrDefault();

            DateTime currentDate = DateTime.Now;
            var RemainingDays = voteDate;

            if (voteDate != null)
            {
                DateTime voteEndDate = voteDate.Value.AddDays(5);
                TimeSpan timeRemaining = voteEndDate - currentDate;

                if (timeRemaining.TotalDays == 0)
                {
                    // 投票已截止
                    ViewBag.RemainingDays = "投票已截止";
                }
                else
                {
                    // 顯示剩餘投票天數
                    ViewBag.RemainingDays = $"剩餘投票天數：{Convert.ToInt32(timeRemaining.TotalDays)} 天";
                }
            }

            return View();
        }

        [Breadcrumb("預設活動投票", FromAction = nameof(MemberController.Member), FromController = typeof(MemberController))]
        //小胖拿過來給頁面
        public IActionResult SelectDatefromMember(int? userId, int? activityId)
        {
            //剩餘天數
            var voteDate = (from m in _context.MyActivity
                            where m.ActivityID == activityId
                            select m.VoteDate).FirstOrDefault();

            DateTime currentDate = DateTime.Now;
            var RemainingDays = voteDate;

            if (voteDate != null)
            {
                DateTime voteEndDate = voteDate.Value.AddDays(5);
                TimeSpan timeRemaining = voteEndDate - currentDate;

                if (timeRemaining.TotalDays == 0)
                {
                    // 投票已截止
                    ViewBag.RemainingDays = "投票已截止";
                }
                else
                {
                    // 顯示剩餘投票天數
                    ViewBag.RemainingDays = $"剩餘投票天數：{Convert.ToInt32(timeRemaining.TotalDays)} 天";
                }
            }

            var activityID = activityId;

            if (userId == null || activityID == null)
            {
                return NotFound();
            }

            var voteTimes = _context.VoteTime
               .Where(vt => vt.ActivityID == activityID)
               .ToList();

            ViewBag.Dates = voteTimes.Select(vt => vt.StartDate).ToList();

            return View("SelectDate");

        }

        //取得使用者的投票紀錄
        [HttpGet]
        public IActionResult GetMemberVote(int? activityId)
        {

            //假設會員是1
            var userIdString = HttpContext.Session.GetString("UserId");
            int userId = int.Parse(userIdString);

            //查找是否已經投票(回傳True||False)
            var userHasVoted = _context.VoteRecord
           .Any(vr => vr.UserID == userId && vr.ActivityID == activityId);

            // 將lastVote初始化為null
            DateTime? lastVote = null;

            // 如果用戶已經投票，獲取用戶上一次的投票選項
            if (userHasVoted)
            {
                lastVote = _context.VoteRecord
                   .Where(vr => vr.UserID == userId && vr.ActivityID == activityId)
                   .OrderByDescending(vr => vr.RecordID)
                   .Select(vr => vr.VoteResult)
                   .FirstOrDefault();
            }

            // 將DateTime轉換為string
            string lastVoteStr = lastVote?.ToString("yyyy-MM-dd"); 

            return Json(lastVoteStr);

        }

        //儲存||更新投票選項
        [HttpPost]
        public IActionResult SaveVote(int activityId, string voteResult)
        {
            
            var userIdString = HttpContext.Session.GetString("UserId");
            int userId = int.Parse(userIdString);

            // 檢查是否已經投票
            var userHasVoted = _context.VoteRecord
                .Any(vr => vr.UserID == userId && vr.ActivityID == activityId);

            if (userHasVoted)
            {
                // 如果使用者已經投票，更新現有的投票選項
                var existingVote = _context.VoteRecord
                    .Where(vr => vr.UserID == userId && vr.ActivityID == activityId)
                    .FirstOrDefault();

                existingVote.VoteResult = DateTime.Parse(voteResult);
            }
            else
            {
                // 如果使用者尚未投票，建立新的投票紀錄
                var newVoteRecord = new VoteRecord
                {
                    UserID = userId,
                    ActivityID = activityId,
                    VoteResult = DateTime.Parse(voteResult)
                };

                _context.VoteRecord.Add(newVoteRecord);
            }

            _context.SaveChanges();

            //再次查找投票紀錄
            var updatedVote = _context.VoteRecord
                 .Where(vr => vr.UserID == userId && vr.ActivityID == activityId)
                   .OrderByDescending(vr => vr.RecordID)
                   .Select(vr => vr.VoteResult)
                   .FirstOrDefault();

            // 將DateTime轉換為string
            string UpdatedVoteStr = updatedVote?.ToString("yyyy-MM-dd");


            return Json(UpdatedVoteStr);
        }




        //----------------James寫的-----------------













        // GET: VoteRecords
        public async Task<IActionResult> Index()
        {
            var projectXContext = _context.VoteRecord.Include(v => v.Activity).Include(v => v.User);
            return View(await projectXContext.ToListAsync());
        }

        // GET: VoteRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VoteRecord == null)
            {
                return NotFound();
            }

            var voteRecord = await _context.VoteRecord
                .Include(v => v.Activity)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (voteRecord == null)
            {
                return NotFound();
            }

            return View(voteRecord);
        }

















        //先暫時註解掉

        // GET: VoteRecords/Create
        //public IActionResult Create()
        //{
        //    var datesQuery = from voteTime in _context.VoteTime
        //                     where voteTime.ActivityID == 1
        //                     select voteTime.StartDate;

        //    // 轉為 List
        //    var dates = datesQuery.ToList();

        //    // 用於 Debug 的代碼
        //    Console.WriteLine("Dates Count: " + dates.Count);
        //    foreach (var date in dates)
        //    {
        //        Console.WriteLine("Date: " + date);
        //    }

        //    // 將日期傳遞到 View 中
        //    ViewBag.Dates = dates;

        //    return View();
        //}



        // POST: VoteRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //先暫時註解掉
        //public async Task<IActionResult> Create([Bind("RecordID,UserID,ActivityID,VoteResult")] VoteRecord voteRecord)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // 設置 ActivityID 為 1
        //        voteRecord.ActivityID = 1;

        //        _context.Add(voteRecord);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewData["ActivityID"] = new SelectList(_context.MyActivity, "ActivityID", "ActivityID", voteRecord.ActivityID);
        //    ViewData["UserID"] = new SelectList(_context.Member, "UserID", "UserID", voteRecord.UserID);
        //    return View(voteRecord);
        //}


        // GET: VoteRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VoteRecord == null)
            {
                return NotFound();
            }

            var voteRecord = await _context.VoteRecord.FindAsync(id);
            if (voteRecord == null)
            {
                return NotFound();
            }
            ViewData["ActivityID"] = new SelectList(_context.MyActivity, "ActivityID", "ActivityID", voteRecord.ActivityID);
            ViewData["UserID"] = new SelectList(_context.Member, "UserID", "UserID", voteRecord.UserID);
            return View(voteRecord);
        }

        // POST: VoteRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecordID,UserID,ActivityID,VoteResult")] VoteRecord voteRecord)
        {
            if (id != voteRecord.RecordID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voteRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoteRecordExists(voteRecord.RecordID))
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
            ViewData["ActivityID"] = new SelectList(_context.MyActivity, "ActivityID", "ActivityID", voteRecord.ActivityID);
            ViewData["UserID"] = new SelectList(_context.Member, "UserID", "UserID", voteRecord.UserID);
            return View(voteRecord);
        }

        // GET: VoteRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VoteRecord == null)
            {
                return NotFound();
            }

            var voteRecord = await _context.VoteRecord
                .Include(v => v.Activity)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (voteRecord == null)
            {
                return NotFound();
            }

            return View(voteRecord);
        }

        // POST: VoteRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.VoteRecord == null)
            {
                return Problem("Entity set 'ProjectXContext.VoteRecord'  is null.");
            }
            var voteRecord = await _context.VoteRecord.FindAsync(id);
            if (voteRecord != null)
            {
                _context.VoteRecord.Remove(voteRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoteRecordExists(int id)
        {
            return (_context.VoteRecord?.Any(e => e.RecordID == id)).GetValueOrDefault();
        }
    }
}
