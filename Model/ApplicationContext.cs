using LuxoraStore.Model.DB;
using Microsoft.EntityFrameworkCore;

namespace LuxoraStore.Model
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            
        }

        public virtual DbSet<User> Users { get; set; }


    }
}
