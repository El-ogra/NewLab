using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NewLab.Data
{
    public class NewLabDbContextFactory : IDesignTimeDbContextFactory<NewLabDbContext>
    {
        public NewLabDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NewLabDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=NewLabDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new NewLabDbContext(optionsBuilder.Options);
        }
    }
}
