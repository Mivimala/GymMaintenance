using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class ServicetableModel
    {
        [Key]
        public int ServiceId { get; set; }

        public string? ServiceName { get; set; }

        public DateOnly? CreateAt { get; set; }
    }
}
