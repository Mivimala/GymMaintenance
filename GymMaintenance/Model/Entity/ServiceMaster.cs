using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class ServiceMaster
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string PlanDuration { get; set; }
        public decimal PlanAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
