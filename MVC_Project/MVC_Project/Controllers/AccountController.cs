using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using MimeKit;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace MVC_Project.Controllers
{
    public class AccountController : Controller
    {
        
            private readonly ProjectXContext _context;

            public AccountController(ProjectXContext context)
            {
                _context = context;
            }

		// GET 方法用於展示「登入」表單
		[HttpGet]
		public IActionResult Login()
		{
			return View("~/Views/Home/Login.cshtml");
		}

		// GET 方法用於展示「註冊」表單
		[HttpGet]
		public IActionResult Register()
		{
			return View("~/Views/Home/Register.cshtml");
		}



        [HttpPost]
        public IActionResult Login(string username, string password, string returnUrl)
        {
            var member = _context.Member.FirstOrDefault(m => m.Account == username);
            if (member != null)
            {
                if (!member.IsActive)
                {
                    TempData["ErrorMessage"] = "此帳號尚未啟用，請先啟用帳號。";
                    return View("~/Views/Home/Login.cshtml");
                }
                // 哈希輸入的密碼和存儲的salt，然後與存儲的哈希密碼進行比較
                string hashedInputPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: member.Salt,  // 從數據庫中獲取salt
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                if (hashedInputPassword == member.PasswordHash)  // 注意這裡改成了 PasswordHash
                {
                    // 密碼驗證成功，執行後續操作
                    HttpContext.Session.SetString("UserId", member.UserID.ToString());
                    return Redirect("https://localhost:7254/");
                }
            }

            TempData["ErrorMessage"] = "帳號或密碼錯誤";
            return View("~/Views/Home/Login.cshtml");
        }


        [HttpPost]
		public async Task<IActionResult> Register(string account, string password, string comfirm_password, string nickname, string email)
		{
			if (password != comfirm_password)
			{
				// 密碼和確認密碼不一致
				return View("~/Views/Home/Register.cshtml", new { error = "密碼和確認密碼不一致" });
			}

			var existingUser = _context.Member.FirstOrDefault(m => m.Account == account);
			if (existingUser != null)
			{
				// 使用者已存在
				return View("~/Views/Home/Register.cshtml", new { error = "使用者已存在" });
			}

            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // 使用密碼和salt進行哈希
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var newUser = new Member
			{
                IsActive = false,  // 新增
                Account = account,
                PasswordHash = hashedPassword,  // 更新這裡
                Salt = salt,  // 更新這裡
                Nickname = nickname,
				Email = email,
				Phone=account,
			
			};

			_context.Member.Add(newUser);
			_context.SaveChanges();


			// 發送驗證郵件

			// 在成功的情況下
			await SendVerificationEmail(email, newUser.UserID);
			TempData["SuccessMessage"] = "已發送驗證信，請檢查您的信箱。";
			return View("~/Views/Home/Register.cshtml");




			//                                                                  // 登入用戶
			//         HttpContext.Session.SetString("UserId", newUser.UserID.ToString());

			//return RedirectToAction("Index", "Home"); // 跳轉至主頁
		}

		private async Task<bool> SendVerificationEmail(string email, int userId)
        {
            var service = await GetGmailService();
            GmailMessage message = new GmailMessage
            {
                Subject = "JO!N會員您好，請啟用您的帳號",
                Body = $"<a href='https://localhost:7254/Account/ActivateAccount?userId={userId}'>點此啟用帳號</a>",
                FromAddress = "JO!N <lin0975408252@gmail.com>",
                IsHtml = true,
                ToRecipients = email
            };

            SendEmail(message, service);
            return true;
        }



        [HttpGet]
        public async Task<IActionResult> ActivateAccount(int userId)
        {
            Console.WriteLine($"Activating account for user ID: {userId}");  // Debug line
            var member = await _context.Member.FindAsync(userId);
            if (member != null)
            {
                Console.WriteLine("Member found, setting IsActive to true.");  // Debug line
                member.IsActive = true;
                _context.Update(member);
                await _context.SaveChangesAsync();
				TempData["ActivationSuccess"] = "帳戶已開通，現在可以登入了哦！";
                return View("~/Views/Home/AccountActivated.cshtml");
            }
            else
            {
                Console.WriteLine("Member not found.");  // Debug line
                return View("ActivationFailed");
            }
        }
        ////拿來看一下有沒有抓到，現在UserID是多少
        public IActionResult CheckSession()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                return Content($"Logged in user ID: {userId}");
            }
            else
            {
                return Content("No user is logged in.");
            }
        }

        ////這邊是登出要指向的
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Homepage", "MyActivity");
        }


		//忘記密碼
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("~/Views/Home/ForgotPassword.cshtml");
        }

        // POST 方法用於處理「忘記密碼」請求
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string account, string email)
        {
            // 假設 Member 是你的數據模型
            var member = _context.Member.FirstOrDefault(m => m.Account == account && m.Email == email);
            if (member != null)
            {
                // 生成重設密碼令牌和過期時間
                string resetToken = Guid.NewGuid().ToString();
                DateTime expirationTime = DateTime.UtcNow.AddHours(2);  // 令牌在2小時後過期

                member.ResetToken = resetToken;
                member.ResetTokenExpiration = expirationTime;

                _context.Update(member);
                await _context.SaveChangesAsync();

                // 發送帶有重設令牌的重設密碼郵件
                string resetLink = $"https://localhost:7254/Account/ResetPassword?token={resetToken}"; // 生成重設密碼的鏈接
                await SendResetPasswordEmail(email, resetLink);

                return RedirectToAction("Login");
            }
            else
            {
                // 處理失敗情況
                return View("ForgotPassword", new { error = "帳號或電子郵件不正確" });
            }
        }

        private async Task<bool> SendResetPasswordEmail(string email, string resetLink)
        {
            var member = _context.Member.FirstOrDefault(m => m.Email == email);
            var service = await GetGmailService();

            GmailMessage message = new GmailMessage
            {
                Subject = "JO!N會員重設密碼",
                Body = $"<h1>請點擊以下連接重設您的密碼：</h1><a href='{resetLink}'>重設密碼</a>",
                FromAddress = "JO!N <lin0975408252@gmail.com>",
                IsHtml = true,
                ToRecipients = email
            };

            SendEmail(message, service);
            Console.WriteLine("Email sent.");
            return true;
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, bool success = false)
        {
            if (success)
            {
                ViewData["ResetSuccess"] = "True";
            }
            return View("~/Views/Home/ResetPassword.cshtml", new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var member = _context.Member.FirstOrDefault(m => m.ResetToken == model.Token && m.ResetTokenExpiration >= DateTime.UtcNow);

                if (member == null)
                {
                    return View("Error", new { error = "無效或過期的令牌" });
                }

                // 重設密碼（這裡只是一個示例，你應該使用更安全的哈希方法）
                // 哈希新密碼
                string hashedNewPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: model.NewPassword,
                    salt: member.Salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                member.PasswordHash = hashedNewPassword;
                _context.SaveChanges();

                // 清除重設令牌和過期時間
                member.ResetToken = null;
                member.ResetTokenExpiration = null;

                _context.Update(member);
                await _context.SaveChangesAsync();

                await _context.SaveChangesAsync();
                return RedirectToAction("ResetPassword", "Account", new { success = true });

            }

            
            return View("~/Views/Home/index");
        }


        //齊澤寄信
        [HttpPost]
        public async Task<IActionResult> newContact(string fullName, string mail, string mobileNumber, string emailSubject, string wrotetext)
        {
            var newContact = new Contact
            {
                SenderName = fullName,
                Email = mail,
                Phone = mobileNumber,
                EmailTitle = emailSubject,
                FormContent = wrotetext,
                SendTime = DateTime.Now
            };

            _context.Contact.Add(newContact);
            await _context.SaveChangesAsync();

            // 發送確認郵件
            await SendConfirmationEmail(mail, newContact);


            return Json(new { success = true });

            
        }


        private async Task<bool> SendConfirmationEmail(string email, Contact contact)
        {
            var service = await GetGmailService();

            GmailMessage message = new GmailMessage
            {
                Subject = "聯絡表單確認",
                Body = $"<h1>親愛的 {contact.SenderName}，感謝您的留言。<br/></h1><p>您的留言內容為：<br/> {contact.FormContent}<br/>我們將迅速回復您</p>",
                FromAddress = "JO!N <lin0975408252@gmail.com>",
                IsHtml = true,
                ToRecipients = email
            };

            SendEmail(message, service);
            Console.WriteLine("Confirmation email sent.");
            return true;
        }



        ///這邊是寄送驗證信的開始
        /// <summary>
        /// 取得授權的項目
        /// </summary>
        static string[] Scopes = { GmailService.Scope.GmailSend };

		// 和登入 google 的帳號無關
		// 任意值，若未來有使用者認証，可使用使用者編號或登入帳號。
		string Username = "ABC";

		/// <summary>
		/// 存放 client_secret 和 credential 的地方
		/// </summary>

		string SecretPath = @"C:\Users\James\Desktop\ProjectX";
		//string jsonFilePath = Path.Combine("wwwroot", "Resources", "client_secret.json");
		/// <summary>
		/// 認証完成後回傳的網址, 必需和 OAuth 2.0 Client Id 中填寫的 "已授權的重新導向 URI" 相同。
		/// </summary>
		string RedirectUri = $"https://localhost:7254/Account/AuthReturn";

		/// <summary>
		/// 取得認証用的網址
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetAuthUrl()
		{
			using (var stream = new FileStream(Path.Combine(SecretPath, "client_secret.json"), FileMode.Open, FileAccess.Read))
			{
				FileDataStore dataStore = null;
				var credentialRoot = Path.Combine(SecretPath, "Credentials");
				if (!Directory.Exists(credentialRoot))
				{
					Directory.CreateDirectory(credentialRoot);
				}

				//存放 credential 的地方，每個 username 會建立一個目錄。
				string filePath = Path.Combine(credentialRoot, Username);
				dataStore = new FileDataStore(filePath);

				IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
				{
					ClientSecrets = GoogleClientSecrets.Load(stream).Secrets,
					Scopes = Scopes,
					DataStore = dataStore
				});

				var authResult = await new AuthorizationCodeWebApp(flow, RedirectUri, Username)
				.AuthorizeAsync(Username, CancellationToken.None);

				return authResult.RedirectUri;
			}
		}

		public async Task<string> AuthReturn(AuthorizationCodeResponseUrl authorizationCode)
		{
			string[] scopes = new[] { GmailService.Scope.GmailSend };

			using (var stream = new FileStream(Path.Combine(SecretPath, "client_secret.json"), FileMode.Open, FileAccess.Read))
			{
				//確認 credential 的目錄已建立.
				var credentialRoot = Path.Combine(SecretPath, "Credentials");
				if (!Directory.Exists(credentialRoot))
				{
					Directory.CreateDirectory(credentialRoot);
				}

				//暫存憑証用目錄
				string tempPath = Path.Combine(credentialRoot, authorizationCode.State);

				IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(
				new GoogleAuthorizationCodeFlow.Initializer
				{
					ClientSecrets = GoogleClientSecrets.Load(stream).Secrets,
					Scopes = scopes,
					DataStore = new FileDataStore(tempPath)
				});

				//這個動作應該是要把 code 換成 token
				await flow.ExchangeCodeForTokenAsync(Username, authorizationCode.Code, RedirectUri, CancellationToken.None).ConfigureAwait(false);

				if (!string.IsNullOrWhiteSpace(authorizationCode.State))
				{
					string newPath = Path.Combine(credentialRoot, Username);
					if (tempPath.ToLower() != newPath.ToLower())
					{
						if (Directory.Exists(newPath))
							Directory.Delete(newPath, true);

						Directory.Move(tempPath, newPath);
					}
				}

				return "OK";
			}
		}

		//public async Task<bool> SendTestMail()
		//{
		//	var service = await GetGmailService();

		//	GmailMessage message = new GmailMessage();
		//	message.Subject = "標題";
		//	message.Body = $"<h1>內容</h1>";
		//	message.FromAddress = "lin0975408252@gmail.com";
		//	message.IsHtml = true;
		//	message.ToRecipients = "wee06011@gmail.com";
		//	message.Attachments = new List<Attachment>();

		//	string filePath = @"C:\Users\User\Documents\GitHub\ProjectX\齊澤\Files\小黃人.png";    //要附加的檔案
		//	Attachment attachment1 = new Attachment(filePath);
		//	message.Attachments.Add(attachment1);

		//	SendEmail(message, service);
		//	Console.WriteLine("OK");

		//	return true;
		//}

		async Task<GmailService> GetGmailService()
		{
			UserCredential credential = null;

			var credentialRoot = Path.Combine(SecretPath, "Credentials");
			if (!Directory.Exists(credentialRoot))
			{
				Directory.CreateDirectory(credentialRoot);
			}

			string filePath = Path.Combine(credentialRoot, Username);

			using (var stream = new FileStream(Path.Combine(SecretPath, "client_secret.json"), FileMode.Open, FileAccess.Read))
			{
				credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
				GoogleClientSecrets.Load(stream).Secrets,
				Scopes,
				Username,
				CancellationToken.None,
				new FileDataStore(filePath));
			}

			var service = new GmailService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = "Send Mail",
			});

			return service;
		}


		public class GmailMessage
		{
			public string FromAddress { get; set; }
			public string ToRecipients { get; set; }

			public string Subject { get; set; }
			public string Body { get; set; }
			public bool IsHtml { get; set; }

			public List<System.Net.Mail.Attachment> Attachments { get; set; }
		}


		public static void SendEmail(GmailMessage email, GmailService service)
		{
			var mailMessage = new System.Net.Mail.MailMessage();
			mailMessage.From = new System.Net.Mail.MailAddress(email.FromAddress);
			mailMessage.To.Add(email.ToRecipients);
			mailMessage.ReplyToList.Add(email.FromAddress);
			mailMessage.Subject = email.Subject;
			mailMessage.Body = email.Body;
			mailMessage.IsBodyHtml = email.IsHtml;

			if (email.Attachments != null)
			{
				foreach (System.Net.Mail.Attachment attachment in email.Attachments)
				{
					mailMessage.Attachments.Add(attachment);
				}
			}

			var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);

			var gmailMessage = new Google.Apis.Gmail.v1.Data.Message
			{
				Raw = Encode(mimeMessage)
			};

			Google.Apis.Gmail.v1.UsersResource.MessagesResource.SendRequest request = service.Users.Messages.Send(gmailMessage, "me");

			request.Execute();
		}

		public static string Encode(MimeMessage mimeMessage)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				mimeMessage.WriteTo(ms);
				return Convert.ToBase64String(ms.GetBuffer())
					.TrimEnd('=')
					.Replace('+', '-')
					.Replace('/', '_');
			}
		}
	}
}
