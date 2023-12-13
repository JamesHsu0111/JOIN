using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models;
using SmartBreadcrumbs.Attributes;
using System.Diagnostics;

namespace MVC_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult FormPage()
        {
            return View();
        }

        public IActionResult Act()
        {
            return View();
        }


        
        

        public IActionResult ActivityPage()
        {
            return View();
        }

		[Breadcrumb("登入", FromAction = nameof(MyActivityController.HomePage), FromController = typeof(MyActivityController))]

		public IActionResult Login()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		public IActionResult Register()
		{
			return View();
		}
	}
}