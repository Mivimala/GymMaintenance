using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GymMaintenance.Model.Entity
{
    public class FingerPrint
    {
        [Key]
        public int FingerPrintID { get; set; }

        public string Role { get; set; }
        public byte[] FingerPrint1 { get; set; }
        public byte[] FingerPrint2 { get; set; }
        public byte[] FingerPrint3 { get; set; }
        public DateTime CreatedDate { get; set; }
        //public ICollection<CandidateEnrollment>? Candidates { get; set; }
        //public ICollection<AttendanceTable>? Attendances { get; set; }

        //public ICollection<TrainerEnrollment>? Trainers { get; set; }
        //public ICollection<CandidateEnroll>? Candidates { get; set; }
        //public ICollection<AttendanceTable>? Attendances { get; set; }
    }
    }
