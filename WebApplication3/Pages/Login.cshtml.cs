using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {
		[BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor contxt;
        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
		{
			this.signInManager = signInManager;
            this.userManager = userManager;
            contxt = httpContextAccessor;
        }
		public void OnGet()
        {
        }

		public async Task<IActionResult> OnPostAsync()
		{
            if (ModelState.IsValid)
			{
                var user = await userManager.FindByEmailAsync(LModel.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Email does not exist");
                    return Page();
                }

                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, false);

                if (identityResult.Succeeded)
                {
                    // Set security stamp to invalidate existing login sessions
                    await userManager.UpdateSecurityStampAsync(user);

                    // Log the user in
                    await signInManager.SignInAsync(user, LModel.RememberMe);

                    // Set session variables
                    contxt.HttpContext.Session.SetString("Email", LModel.Email);

                    return RedirectToPage("Index");
                }
                else if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account locked out. Please try again later.");
                }
                //else if (identityResult.RequiresTwoFactor)
                //{
                    // Implement two-factor authentication if needed
                //    return RedirectToPage("LoginWith2fa", new { ReturnUrl = "/Index", RememberMe = LModel.RememberMe });
                //}
                else
                {
                    var maxLoginAttempts = 3;
                    var remainingAttempts = maxLoginAttempts - await userManager.GetAccessFailedCountAsync(user);

                    if (remainingAttempts <= 0)
                    {
                        // Lock the user out
                        await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                        ModelState.AddModelError("", "Account locked out. Please try again later.");
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Invalid login attempt. {remainingAttempts} login attempts left.");
                    }
                }
            }

            return Page();
        }
    }
}
