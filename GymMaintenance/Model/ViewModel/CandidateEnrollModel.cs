using GymMaintenance.Model.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class CandidateEnrollModel
    {
       [Key]
        public int CandidateId { get; set; }

        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? MobileNumber { get; set; }
        public DateOnly?  DOB { get; set; }
        public int?   ServiceId { get; set; }
        public int? PackageId { get; set; }
        public int? PackageMonths { get; set; }
        public decimal? PackageAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? PaymentStatus { get; set; }
        public int? FingerPrintID { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        //[ForeignKey("FingerPrintID")]
        //public FingerPrint? FingerPrint { get; set; }

        //public ICollection<AttendanceTable>? Attendances { get; set; }

    }
}
