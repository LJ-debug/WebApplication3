using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
	public class Register
	{
		[Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "First Name is required.")]
		[DataType(DataType.Text)]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last Name is required.")]
		[DataType(DataType.Text)]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Gender is required.")]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

		[Required(ErrorMessage = "NRIC is required.")]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }

		[Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(12, ErrorMessage = "Password must be at least 12 characters long.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}$", ErrorMessage = "Password must contain a combination of lower-case, upper-case, numbers, and special characters.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile Resume { get; set; }

        [DataType(DataType.Text)]
        public string WhoAmI { get; set; }
	}
}
