using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class PackagetableModel
    {
        [Key]
        public int PackageId { get; set; }

        public string? PackageName { get; set; }

        public decimal? PackageAmount { get; set; }

        public DateOnly? CreatedAt { get; set; }
    }
}
