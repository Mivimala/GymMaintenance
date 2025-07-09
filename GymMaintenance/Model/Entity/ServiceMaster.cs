using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class ServiceMaster
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        
        public DateTime CreatedAt{ get; set; }
       
    }
}
