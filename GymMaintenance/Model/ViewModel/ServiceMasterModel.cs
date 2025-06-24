namespace GymMaintenance.Model.ViewModel
{
    public class ServiceMasterModel
    {
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? PlanDuration { get; set; }
        public decimal? PlanAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
