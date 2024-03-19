using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "Passweord is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Required(ErrorMessage = "Confirm Passweord is required")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
		public string Email { get; set; }
		public string Token { get; set; }
	}
}
