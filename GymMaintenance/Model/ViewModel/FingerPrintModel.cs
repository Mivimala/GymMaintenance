using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class FingerPrintModel
    {
        [Key]
        public int FingerPrintID { get; set; }

        public string? Role { get; set; }
        public byte[]? FingerPrint1 { get; set; }
        public byte[]? FingerPrint2 { get; set; }
        public byte[]? FingerPrint3 { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
