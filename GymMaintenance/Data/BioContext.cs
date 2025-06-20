using Microsoft.EntityFrameworkCore;
using GymMaintenance.Model.Entity;



namespace GymMaintenance.Data
{
    public class BioContext:DbContext
    {

        public BioContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Login> Login { get; set; }
    }
}
