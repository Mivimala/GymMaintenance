using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class TrainerEnrollmentModel
    {
        [Key]
        public int TrainerId { get; set; }

        public string? Password { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }

        public string? Address { get; set; }

        public string? MobileNumber { get; set; }

        public DateOnly? JoiningDate { get; set; }

        public int? FingerPrintID { get; set; }//FK

        public bool? IsActive { get; set; }
        public DateOnly? CreatedDate { get; set; }

    }
}
