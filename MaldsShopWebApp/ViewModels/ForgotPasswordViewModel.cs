using System.ComponentModel.DataAnnotations;

namespace MaldsShopWebApp.ViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
