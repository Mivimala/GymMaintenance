using GymMaintenance.Model.Entity;
using GymMaintenance.Model.ViewModel;


namespace GymMaintenance.DAL.Interface
{
    public interface IBioInterface
    {

       

         public List<Login> GetAll();

        #region EquipmentEnrollment
        public EquipmentEnrollment AddEquipmentEnrollment(EquipmentEnrollment equipment);
        public List<EquipmentEnrollmentModel> GetallEquipmentEnrollments();

        public EquipmentEnrollmentModel GetEquipmentEnrollmentbyid(int id);
        public EquipmentEnrollment deleteEquipmentbyid(int id);

        #endregion


        #region HealthProgressTracking
        public HealthProgressTracking AddHealthProgressTracking(HealthProgressTracking healthProgressTracking);
        public List<HealthProgressTrackingModel> GetallHealthProgressTrackings();

        public HealthProgressTrackingModel GetbyidHealthProgressTracking(int id);
        public HealthProgressTracking deleteHealthprogresstrackingbyid(int id);
        #endregion





    }
}
