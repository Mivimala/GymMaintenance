using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class Servicetable
    {
        [Key]
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }

        public DateOnly CreateAt { get; set; }

    }
}
