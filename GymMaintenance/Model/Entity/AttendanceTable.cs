using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class AttendanceTable
    {

        [Key]
        public int AttendanceId { get; set; }

        public int CandidateId { get; set; }
        public string CandidateName { get; set; }

        public int FingerPrintID { get; set; }

        public DateTime AttendanceDate { get; set; }
        public TimeSpan InTime { get; set; }
        public TimeSpan OutTime { get; set; }

        [ForeignKey("FingerPrintID")]
        public FingerPrint FingerPrint { get; set; }
    }
}
