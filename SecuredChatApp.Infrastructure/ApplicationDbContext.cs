using Microsoft.EntityFrameworkCore;

namespace SecuredChatApp.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
