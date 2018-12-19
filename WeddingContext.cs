using Microsoft.EntityFrameworkCore;
namespace WeddingPlanner.Models
{
    public class WeddingContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public WeddingContext(DbContextOptions<WeddingContext> options) : base(options) { }
        // ADDING USER DB TO CONTEXT
        public DbSet<User> UserTable {get;set;}
        public DbSet<WeddingModel> Wedtable {get;set;}
        public DbSet<Guest> Guest { get; set; }
    }
}