namespace GymMaintenance.Model.ViewModel
{
    public class EquipmentEnrollmentModel
    {

        public int EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
        public DateTime EquipmentPurchaseDate { get; set; }
        public int EquipmentCount { get; set; }
        public string? EquipmentCondition { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
