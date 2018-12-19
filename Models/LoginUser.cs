using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner
{
    public class LoginUser
    {
        [Required (ErrorMessage="Email is Required.")]
        // No other fields!
        public string Email {get; set;}
        [Required (ErrorMessage="Password is Required.")]
        public string Password { get; set; }
    }
}