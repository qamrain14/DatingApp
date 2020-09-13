using datingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users{get;set;}
    }
}