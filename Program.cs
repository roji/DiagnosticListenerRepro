using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DiagnosticListenerRepro
{
    class Program
    {
        private const string ConnectionString = "Server=localhost;Database=test;User=SA;Password=Abcd5678;Connect Timeout=60;ConnectRetryCount=0";

        static void Main(string[] args)
        {
            var collection = new ServiceCollection();
            collection.AddDbContext<BlogContext>(o => o.UseSqlServer(ConnectionString));
            var serviceProvider = collection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            Iteration(serviceProvider);
        }

        static void Iteration(ServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
                var blogs = context.Blogs.ToList();
            }
        }
    }

    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}
        public DbSet<Blog> Blogs { get; set; }
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
