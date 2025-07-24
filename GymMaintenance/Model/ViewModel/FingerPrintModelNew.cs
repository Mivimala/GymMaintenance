using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class FingerPrintModelNew
    {
        public int FingerPrintID { get; set; }
        public string? Role { get; set; }

        // These are base64 strings from frontend
        public string? FingerPrint1 { get; set; }
        public string? FingerPrint2 { get; set; }
        public string? FingerPrint3 { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
