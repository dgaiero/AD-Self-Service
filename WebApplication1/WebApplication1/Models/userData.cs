using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class userData
    {
        [Required(ErrorMessage = "Username is required")]
        public string username { get; set; }
        [Required(ErrorMessage = "Old Password is required")]
        public string oldPassword { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        public string newPassword { get; set; }
        //[Required(ErrorMessage = "New Password Confirmation is required")]
        [Compare("newPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string newPasswordConfirm { get; set; }
    }
}