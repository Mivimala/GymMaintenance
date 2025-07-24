
using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.Entity
{
    public class Packagetable
    {

        [Key]
        public int PackageId { get; set; }

        public string PackageName { get; set; }

        public decimal PackageAmount { get; set; }

        public DateOnly CreatedAt { get; set; }

    }
}
