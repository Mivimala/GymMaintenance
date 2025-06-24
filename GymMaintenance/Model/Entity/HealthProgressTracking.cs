using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class HealthProgressTracking
    {
        [Key] 
        public int CandidateId { get; set; }

        public string Name { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal Height { get; set; }
        public decimal CurrentWeight { get; set; }
        public decimal InitialBMI { get; set; }
        public decimal CurrentBMI { get; set; }
        public DateTime CurrentDate { get; set; }


    }
}
