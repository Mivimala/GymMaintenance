using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class TrainerEnrollment
    {
        [Key]
        public int TrainerId { get; set; }

        public string Password { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public int    Age { get; set; }
        public string CurrentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string AadharNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string EmploymentType { get; set; }
        public string Experience { get; set; }
        public string Qualification { get; set; }
        public DateOnly JoiningDate { get; set; } 
        public byte[] Picture { get; set; }

        public int FingerPrintID { get; set; }

        public bool IsActive { get; set; }
        public DateOnly CreatedDate { get; set; }

        
    }
}
