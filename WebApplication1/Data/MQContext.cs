using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class MQContext : DbContext
    {
        public MQContext(DbContextOptions<MQContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Machine> Machines { get; set; } = null!; 
        public DbSet<Maintenance> Maintenances { get; set; } = null!;
        public DbSet<Team> Teams { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

    }
}

