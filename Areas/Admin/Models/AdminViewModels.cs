using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using protean.Models;
using protean.Infrastructure;

namespace protean.Areas.Admin.Models
{
    public class NewUserViewModel
    {
        [StringLength(254)]
        [Display(Name = "Avatar Path")]
        public string BadgePath { get; set; }

        [Required]
        [StringLength(254)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Enabled?")]
        public bool IsEnabled { get; set; }

        public string Title { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Cell { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public ApplicationUser User { get; set; }

        public string Code { get; set; }

        public string UserId { get; set; }
    }

    public class EditRolesViewModel
    {
        public ApplicationUser User { get; set; }

        public List<SelectRoleEditor> Roles { get; set; }

        public EditRolesViewModel()
        {
            Roles = new List<SelectRoleEditor>();
        }

        public string UserId { get; set; }

    }

}

