using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMaintenance.Model.Entity
{
    public class FingerprintNew
    {
        [Key]
        public int FingerPrintID { get; set; }
        public string Role { get; set; }

     
        public byte[] FingerPrint1 { get; set; }
        public byte[] FingerPrint2 { get; set; }
        public byte[] FingerPrint3 { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
