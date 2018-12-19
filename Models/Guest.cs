using System.ComponentModel.DataAnnotations;
using WeddingPlanner.Models;

namespace WeddingPlanner
{
    public class Guest
    {
        [Key]
        public int GuestId {get;set;}
        public int UserId {get;set;}
        public int WedId {get;set;}
        public User User {get;set;}
        public int isAvailable {get;set;}
    }
}