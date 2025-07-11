using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class PaymentModel
    {
        [Key]
        public int PaymentReceiptNo { get; set; }

        
        public string? Name { get; set; }
        public int? ServiceId { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? Paymentmode { get; set; }

        public string? collectedby { get; set; }
        public bool? IsActive { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public DateOnly? UpdatedDate { get; set; }

    }
}
