﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMaintenance.Model.Entity
{
    public class Payment
    {
        [Key]
        public int PaymentReceiptNo { get; set; }

        public int MemmberId { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public int ServiceId { get; set; }
        public string Package { get; set; }
        public string TimeSlot { get; set; }

        public DateOnly PlanStartingDate { get; set; }
        public DateOnly PlanExpiringDate { get; set; }

        public decimal PlanAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal CurrentPayment { get; set; }

        public string ModeOfPayment { get; set; }
        public string collectedby { get; set; }
        public bool IsActive { get; set; }
        public DateOnly CreatedDate { get; set; }

        //extra field
        [NotMapped]
        public int months { get; set; }
    }
}
