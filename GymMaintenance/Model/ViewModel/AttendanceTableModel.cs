using GymMaintenance.Model.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class AttendanceTableModel
    {
        [Key]
        public int AttendanceId { get; set; }

        public int? CandidateId { get; set; }
        public string CandidateName { get; set; }

        public int? FingerPrintID { get; set; }

        public DateTime AttendanceDate { get; set; }
        public TimeSpan InTime { get; set; }
        public bool IsActive { get; set; }
        public string? fpbase64 { get; set; }
    }
}

