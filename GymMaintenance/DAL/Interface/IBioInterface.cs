using GymMaintenance.Model.Entity;
using GymMaintenance.Model.Entity;
using GymMaintenance.Model.ViewModel;
using GymMaintenance.Model.ViewModel;


namespace GymMaintenance.DAL.Interface
{
    public interface IBioInterface
    {

        #region Login
        Payment Addpayment(Payment payment, string sessionId);
        public List<LoginModel> GetAllLogin();
        public LoginModel GetLoginById(int id);
        public Login Addlog(Login login);
        bool DeleteById(int id);
        #endregion

        #region FingerPrint
        public List<FingerPrintModel> GetAllfingerprint();
        public FingerPrintModel GetAllfingerprintbyID(int id);
        public FingerPrint AddFingerPrint(FingerPrint fingerprint);
        bool DeleteByfingerprintId(int id);
        #endregion

        #region Payment
        public List<PaymentModel> GetAllpayment();
        public PaymentModel GetpaymentbyId(int id, int serviceId);
        public Payment Addpayment(Payment pymnnt);
        public bool DeleteBypymntId(int id);
        #endregion

        #region TrainerEnrollment
        public List<TrainerEnrollmentModel> GetAlltrainer();
        public TrainerEnrollmentModel GetAlltrainerbyID(int id);
        public TrainerEnrollment AddOrUpdateTrainer(TrainerEnrollment trainer);
        public bool DeleteBytrainerId(int id);
        #endregion

        #region CandidateEnroll
        public List<CandidateEnrollModel> GetAllcandidate();
        public CandidateEnrollModel GetAllcandidatebyID(int id);
        public CandidateEnroll AddOrUpdateCandidate(CandidateEnroll candidate);
        public bool DeleteBycandidateId(int id);
        #endregion

        #region AttendanceTable
        public List<AttendanceTableModel> GetAllAttendance();
        public AttendanceTableModel GetAllAttendancebyID(int id);
        public AttendanceTable AddOrUpdateAttendance(AttendanceTable attendance);
        public bool DeleteByattendanceId(int id);
        #endregion 
       

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

        #region ServiceMaster
        public List<ServiceMaster> GetAllServiceMaster();
        public ServiceMaster GetServiceMasterbyID(int id);
        public ServiceMaster AddOrUpdateServiceMaster(ServiceMaster service);
        public bool DeleteServiceMasterbyId(int id);
        #endregion



    }
}
