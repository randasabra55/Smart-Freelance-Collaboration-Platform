/*using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Smart_Freelance_Infrastructure.Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseSqlServer("Server=.;Database=FreelanceProject;Trusted_Connection=True;TrustServerCertificate=True;");

            return new Context(optionsBuilder.Options);
        }
    }
}
*/