using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class CandidateIdRequestModel
    {
        [Required(ErrorMessage = "CandidateId is required.")]
        public int CandidateId { get; set; }
    }
}
