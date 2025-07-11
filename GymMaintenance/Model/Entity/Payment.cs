using GymMaintenance.Model.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMaintenance.Model.Entity
{
    public class Payment
    {
        [Key]
        public int PaymentReceiptNo { get; set; }
        public int CandiadteId { get; set; }



        public string Name { get; set; }
        public int ServiceId { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Paymentmode { get; set; }

        public string collectedby { get; set; }
        public bool IsActive { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly UpdatedDate { get; set; }


    }
}
