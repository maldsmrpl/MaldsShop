using MaldsShopWebApp.Data;
using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Repository;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace MaldsShopWebApp.Controllers
{
	public class AccountController : Controller
	{
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
		private readonly IUnitOfWork _unitOfWork;

		public AccountController(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            ApplicationDbContext context, 
            IEmailSender emailSender, 
            IUnitOfWork unitOfWork
            )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
		}
        [HttpGet]
        public IActionResult Login()
		{
            var response = new LoginViewModel();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            if (user != null)
            {
                //User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    //Password correct, is email confirmed
                    if (user.EmailConfirmed != false)
                    {
                        //Password correct, email confirmed, sign in
                        var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, true, false);
                        if (result.Succeeded)
                        {
                            await _unitOfWork.Users.UpdateLastActivityAsync(user.Email);
                            await _unitOfWork.CompleteAsync();
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        SendConfirmationEmailAsync(user);
                        return RedirectToAction("SuccessRegistration", "Account");
                    }
                    
                }
                //Password is incorrect
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginViewModel);
            }
            //User not found
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginViewModel);
        }
        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress,
                AddedTime = DateTime.UtcNow,
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            SendConfirmationEmailAsync(newUser);

            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            return RedirectToAction("SuccessRegistration", "Account");
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }
        [HttpGet]
        public IActionResult SuccessRegistration()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
        [HttpGet]
		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
		{
			if (!ModelState.IsValid)
				return View(forgotPasswordVM);
			var user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);
			if (user == null)
				return RedirectToAction(nameof(ForgotPasswordConfirmation));
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
			var message = new EmailMessages();
			await _emailSender.SendEmailAsync(user.Email, "MaldsShop - Password Reset Request", message.ResetPassword(callback));
			return RedirectToAction(nameof(ForgotPasswordConfirmation));
		}
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}
		[HttpGet]
		public IActionResult ResetPassword(string token, string email)
		{
			var model = new ResetPasswordViewModel { Token = token, Email = email };
			return View(model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordVM)
		{
			if (!ModelState.IsValid)
				return View(resetPasswordVM);

			var user = await _userManager.FindByEmailAsync(resetPasswordVM.Email);
			if (user == null)
				RedirectToAction(nameof(ResetPasswordConfirmation));

			var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);
			if (!resetPassResult.Succeeded)
			{
				foreach (var error in resetPassResult.Errors)
				{
					ModelState.TryAddModelError(error.Code, error.Description);
				}

				return View();
			}

			return RedirectToAction(nameof(ResetPasswordConfirmation));
		}
		[HttpGet]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}
		[HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async void SendConfirmationEmailAsync(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            var message = new EmailMessages();
            await _emailSender.SendEmailAsync(user.Email, "MaldsShop account - Email confirmation", message.ConfirmEmail(confirmationLink));
        }
    }
}
