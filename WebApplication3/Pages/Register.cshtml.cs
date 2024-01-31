using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using System;
using Microsoft.AspNetCore.DataProtection;
using System.Linq;
using AspNetCore.ReCaptcha;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {

        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

		private readonly IReCaptchaService _recaptchaService;

		private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment;

		[BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment,
		IReCaptchaService recaptchaService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
			hostingEnvironment = environment;
			_recaptchaService = recaptchaService;
		}

        public void OnGet()
        {
        }

		public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
					FirstName = RModel.FirstName,
					LastName = RModel.LastName,
					Gender = RModel.Gender,
					NRIC = protector.Protect(RModel.NRIC),
					DateOfBirth = RModel.DateOfBirth,
                    WhoAmI = RModel.WhoAmI,
				};

                var existingUser = await userManager.FindByEmailAsync(RModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email is already in use.");
                    return Page();
                }

                if (RModel.Resume != null)
				{
                    // Validate file type
                    var allowedExtensions = new[] { ".docx", ".pdf" };
                    var fileExtension = Path.GetExtension(RModel.Resume.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("RModel.Resume", "Only .docx and .pdf files are allowed.");
                        return Page();
                    }

                    var uniqueFileName = GetUniqueFileName(RModel.Resume.FileName);
					var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
					var filePath = Path.Combine(uploads, uniqueFileName);

					if (!Directory.Exists(uploads))
					{
						Directory.CreateDirectory(uploads);
					}

					RModel.Resume.CopyTo(new FileStream(filePath, FileMode.Create));
                    user.Resume = uniqueFileName;
				}

				var recaptchaResponse = await _recaptchaService.VerifyAsync(Request.Form["g-recaptcha-response"]);

				if (!recaptchaResponse)
				{
					ModelState.AddModelError("Recaptcha", "Please complete the reCAPTCHA.");
					return Page();
				}

				var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

		private string GetUniqueFileName(string fileName)
		{
			fileName = Path.GetFileName(fileName);
			return Path.GetFileNameWithoutExtension(fileName)
					  + "_"
					  + Guid.NewGuid().ToString().Substring(0, 4)
					  + Path.GetExtension(fileName);
		}
    }
}
