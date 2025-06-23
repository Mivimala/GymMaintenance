using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class CandidateEnroll
    {
        [Key]
        public int CandidateId { get; set; }

        public string? Name { get; set; }
        public string? Gender { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? Waist { get; set; }
        public decimal? BMI { get; set; }
        public string? BloodGroup { get; set; }
        public int? Age { get; set; }
        public string? CurrentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? AadharNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public string? Profession { get; set; }
        public byte[]? Picture { get; set; }

        public int? FingerPrintID { get; set; }

        public bool? IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("FingerPrintID")]
        public FingerPrint? FingerPrint { get; set; }
    }
}
