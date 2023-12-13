using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;
using NuGet.Protocol;

namespace MVC_Project.Controllers
{

   
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ProjectXContext _context;
        // GET: groupPageController
        public ValuesController(ProjectXContext context)
        {
            _context = context;
        }
        public string Values() {
            var temp = from m in _context.Member
                       select new Member
                       {
                           UserID = m.UserID,
                           Nickname = m.Nickname,
                           Account = m.Account
                       };
            var test = temp.ToJson();
            return test;
        }
    }
}
