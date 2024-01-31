using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using WebApplication3.Model;

namespace WebApplication3.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IDataProtector _dataProtector;
        private readonly AuthDbContext _dbContext;
		private readonly IHttpContextAccessor contxt;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment;

        public IndexModel(AuthDbContext dbContext, ILogger<IndexModel> logger, SignInManager<ApplicationUser> signInManager,

			Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            _dataProtector = dataProtectionProvider.CreateProtector("MySecretKey");
            hostingEnvironment = environment;
            contxt = httpContextAccessor;
        }

        public string DecryptedNRIC { get; set; }
        public string Iname { get; set; }
        public string Igender { get; set; }
        public string Iemail { get; set; }
        public DateTime Idob { get; set; }
        public string IResume { get; set; }
        public string IWhoAmI { get; set; }


        public void OnGet()
        {
			// Check if the session has timed out
			var lastAccessTime = contxt.HttpContext.Session.GetString("LastAccessTime");
			if (!string.IsNullOrEmpty(lastAccessTime) && DateTime.TryParse(lastAccessTime, out var lastAccessDateTime))
			{
				var idleTimeout = TimeSpan.FromSeconds(5);
				var elapsedTime = DateTime.Now - lastAccessDateTime;

				if (elapsedTime > idleTimeout)
				{
                    // Redirect to the login page or any other desired page
                    contxt.HttpContext.SignOutAsync();
                    contxt.HttpContext.Response.Redirect("/Login");
                    return;
                }
			}

			// Update the last access time in the session
			contxt.HttpContext.Session.SetString("LastAccessTime", DateTime.Now.ToString());

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            Iname = user.LastName + " " + user.FirstName;
            Igender = user.Gender;
            Iemail = user.Email;
            Idob = (DateTime)user.DateOfBirth;
            IResume = user.Resume;
            IWhoAmI = user.WhoAmI;

            if (user != null)
            {
                // Decrypt the NRIC
                DecryptedNRIC = _dataProtector.Unprotect(user.NRIC);
            }
            else
            {
                // Handle the case where the user is not found
                DecryptedNRIC = "User Not Found";
            }

        }
    }
}
