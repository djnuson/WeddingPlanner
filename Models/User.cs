using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

// FIRST NAME
        [Required (ErrorMessage="First Name is Required")]
        public string FirstName {get;set;}

// LAST NAME
        [Required (ErrorMessage="Last Name is Required")]
        public string LastName {get;set;}

// EMAIL
        [Required (ErrorMessage="Email is Required")]
        [EmailAddress]
        public string Email {get;set;}

// PASSWORD
        [DataType(DataType.Password)]
        [Required (ErrorMessage="Password is Required")]
        [MinLength (3, ErrorMessage="Password must be at least 3 characters long!")]
        public string Password {get;set;}

// CONFIRM PASSWORD
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPW {get;set;}

// CREATED/UPDATED
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

// CONNECTING A LIST OF ALL WEDDINGS AND GUESTS

        public List<WeddingModel> wedding {get;set;}
        public List<Guest> guest {get;set;}
        }
}