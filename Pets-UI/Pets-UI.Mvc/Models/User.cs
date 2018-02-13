using System;
using System.ComponentModel.DataAnnotations;

namespace Pets_UI.Mvc.Models
{
    public class User
    {
        public Guid Id { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail address only")]
        public string Email { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Account created at")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }
    }
}