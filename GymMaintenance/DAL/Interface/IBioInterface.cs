using GymMaintenance.Model.Entity;
using GymMaintenance.Model.Entity;
using GymMaintenance.Model.ViewModel;

namespace GymMaintenance.DAL.Interface
{
    public interface IBioInterface
    {

        #region Login
        public List<LoginModel> GetAllLogin();
        public LoginModel GetLoginById(int id);
        public Login Addlog(Login login);
        bool DeleteById(int id);
        #endregion

        #region FingerPrint
         public List<FingerPrint> GetAllfingreprint();
        public FingerPrintModel GetAllfingerprintbyID(int id);
        public FingerPrint AddFingerPrint(FingerPrint fingerprint);
        bool DeleteByfingerprintId(int id);
        #endregion

        #region Payment
        public List<PaymentModel> GetAllpayment();
        public PaymentModel GetAllpaymentbyId(int id);
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
       

    }
}
