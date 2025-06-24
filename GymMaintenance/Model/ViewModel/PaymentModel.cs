using System.ComponentModel.DataAnnotations;

namespace GymMaintenance.Model.ViewModel
{
    public class PaymentModel
    {
        [Key]
        public int PaymentReceiptNo { get; set; }

        public int? MemmberId { get; set; }
        public string? Name { get; set; }
        public string? MobileNumber { get; set; }
        public string? Service { get; set; }
        public string? Package { get; set; }
        public string? TimeSlot { get; set; }

        public DateOnly PlanStartingDate { get; set; }
        public DateOnly PlanExpiringDate { get; set; }

        public decimal? PlanAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal? CurrentPayment { get; set; }

        public string? ModeOfPayment { get; set; }
        public bool? IsActive { get; set; }
        public DateOnly CreatedDate { get; set; }
    }
}
