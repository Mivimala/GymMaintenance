using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class CandidateEnroll
    {
        [Key]
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
             
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DOJ { get; set; }
        public int ServiceId { get; set; }//FK
        public int PackageId { get; set; }//FK
        public decimal PackageAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PaymentStatus { get; set; }

        public int FingerPrintID { get; set; }//FK
        [NotMapped]
              
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        
    }
}
