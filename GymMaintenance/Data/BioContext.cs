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
        public DbSet<FingerPrint> FingerPrint { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<TrainerEnrollment> TrainerEnrollment { get; set;}
        public DbSet<CandidateEnroll> CandidateEnroll { get; set; }
        public DbSet<AttendanceTable> AttendanceTable { get; set; }
        public DbSet<ServiceMaster> ServiceMaster { get; set; }
        public DbSet<EquipmentEnrollment> EquipmentEnrollment { get; set; }


        public DbSet<HealthProgressTracking> HealthProgressTracking { get; set; }   
    }
}
