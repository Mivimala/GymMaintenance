using GymMaintenance.Model.Entity;
using GymMaintenance.Model.Entity;
using GymMaintenance.Model.ViewModel;

namespace GymMaintenance.DAL.Interface
{
    public interface IBioInterface
    {

       

            public List<Login> GetAll();
        public Login GetById(int id);
        public Login Addlog(Login login);
        bool DeleteById(int id);
        public List<FingerPrint> GetAllfingreprint();
        public FingerPrint GetByfingerprintId(int id);
        public FingerPrint AddFingerPrint(FingerPrint fingerprint);
        bool DeleteByfingerprintId(int id);
        public List<Payment> GetAllpaymnent();
        public Payment GetBypaymentid(int id);
        public Payment Addpayment(Payment pymnnt);
        public bool DeleteBypymntId(int id);
        public List<TrainerEnrollment> GetAlltrainer();
        public TrainerEnrollment GetBytrainerid(int id);
        public TrainerEnrollment AddOrUpdateTrainer(TrainerEnrollment trainer);
        public bool DeleteBytrainerId(int id);
        public List<CandidateEnroll> GetAllcandidate();
        public CandidateEnroll GetBycandidateid(int id);
        public CandidateEnroll AddOrUpdateCandidate(CandidateEnroll candidate);
        public bool DeleteBycandidateId(int id);
        public List<AttendanceTable> GetAllattendance();
        public AttendanceTable GetByattendanceid(int id);
        public AttendanceTable AddOrUpdateAttendance(AttendanceTable attendance);
        public bool DeleteByattendanceId(int id);

        public List<LoginModel> GetAllLoginlog();

    }
}
