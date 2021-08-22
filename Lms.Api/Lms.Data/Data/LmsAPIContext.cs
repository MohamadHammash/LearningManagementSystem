using Lms.API.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace Lms.API.Data.Data
{
    public class LmsAPIContext : DbContext
    {
        public LmsAPIContext(DbContextOptions<LmsAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Publication> Publications { get; set; }

        public DbSet<Level> Levels { get; set; }

        public DbSet<Subject> Subjects { get; set; }
    }
}