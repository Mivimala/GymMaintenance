using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GymMaintenance.Model.Entity;
using GymMaintenance.Data;
using GymMaintenance.DAL.Interface;
using GymMaintenance.Model.ViewModel;

namespace GymMaintenance.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class BioController : ControllerBase
    {
        public readonly BioContext _ibioContext;
        public readonly IBioInterface _ibiointerface;
        public BioController(BioContext bioContext, IBioInterface bioInterface)
        {
            _ibioContext = bioContext;
            _ibiointerface = bioInterface;
        }

        #region Login

        [HttpGet]
        public List<LoginModel> GetAllLoginlog()
        {
            return _ibiointerface.GetAllLogin();
        }

        [HttpGet("{id:int}")]

        public ActionResult<LoginModel> GetLoginById(int id)
        {
            var result = _ibiointerface.GetLoginById(id);

            return result;
        }

        [HttpPost]

        public Login AddTrainerlog(Login login)
        {
            return _ibiointerface.AddTrainerlog(login);
        }
       
        [HttpDelete("{id:int}")]
        public IActionResult DeleteById(int id)
        {
            var result = _ibiointerface.DeleteById(id);
            return Ok();
        }

        [HttpPost]

        public Task<LoginModel> AuthenticateTrainerLoginAsync(string username, string password)
        {
            return _ibiointerface.AuthenticateTrainerLoginAsync(username, password);
        }
        #endregion


        #region FingerPrint

        [HttpGet]
        public List<FingerPrintModel> GetAllfingerprint()
        {
            return _ibiointerface.GetAllfingerprint();
        }

        [HttpGet("{id:int}")]

        public ActionResult<FingerPrintModel> GetAllfingerprintbyID(int id)
        {
            var result = _ibiointerface.GetAllfingerprintbyID(id);

            return result;
        }

        [HttpPost]
        public FingerPrint AddFingerPrint(FingerPrint fingerprint)
        {
            return _ibiointerface.AddFingerPrint(fingerprint);
        }

        [HttpDelete]
        public IActionResult DeleteByfingerprintId(int id)
        {
            var result = _ibiointerface.DeleteByfingerprintId(id);
            return Ok();
        }
        #endregion

        #region Payment
        [HttpGet]
        public List<PaymentModel> GetAllpayment()
        {
            return _ibiointerface.GetAllpayment();
        }

        [HttpGet]

        public ActionResult<PaymentModel> GetpaymentbyId(int id, int serviceId)
        {
            var result = _ibiointerface.GetpaymentbyId(id,serviceId);

            return result;
        }

        [HttpPost]
        public Payment Addpayment(Payment pymnnt)
        {
            return _ibiointerface.Addpayment(pymnnt);
        }

        [HttpDelete]
        public IActionResult DeleteBypymntId(int id)
        {
            var result = _ibiointerface.DeleteBypymntId(id);
            return Ok();
        }
        #endregion


        #region Trainer

        [HttpGet]
        public List<TrainerEnrollmentModel> GetAlltrainer()
        {
            return _ibiointerface.GetAlltrainer();
        }

        [HttpGet("{id:int}")]

        public ActionResult<TrainerEnrollmentModel> GetAlltrainerbyID(int id)
        {
            var result = _ibiointerface.GetAlltrainerbyID(id);

            return result;
        }

        [HttpPost("Addtrainer")]
        public TrainerEnrollment AddOrUpdateTrainer(TrainerEnrollment trainer)
        {
            return _ibiointerface.AddOrUpdateTrainer(trainer);
        }

        [HttpDelete("trainer/{id:int}")]
        public IActionResult DeleteBytrainerId(int id)
        {
            var result = _ibiointerface.DeleteBytrainerId(id);
            if (!result)
                return NotFound($"No login found with ID = {id}");

            return Ok($"Login with ID = {id} deleted successfully");
        }
        #endregion


        #region Candidate
        [HttpGet]
        public List<CandidateEnrollModel> GetAllcandidate()
        {
            return _ibiointerface.GetAllcandidate();
        }

        [HttpGet]

        public ActionResult<CandidateEnrollModel> GetAllcandidatebyID(int id)
        {
            var result = _ibiointerface.GetAllcandidatebyID(id);

            return result;
        }

        [HttpPost]
        public CandidateEnroll AddOrUpdateCandidate(CandidateEnroll candidate)
        {
            return _ibiointerface.AddOrUpdateCandidate(candidate);
        }

        [HttpDelete]
        public IActionResult DeleteBycandidateId(int id)
        {
            var result = _ibiointerface.DeleteBycandidateId(id);
            return Ok();
        }
        #endregion

        #region Attendance

        [HttpGet]
        public List<AttendanceTableModel> GetAllAttendance()
        {
            return _ibiointerface.GetAllAttendance();
        }
        [HttpGet]

        public ActionResult<AttendanceTableModel> GetAllAttendancebyID(int id)
        {
            var result = _ibiointerface.GetAllAttendancebyID(id);

            return result;
        }

        [HttpPost]
        public AttendanceTable AddOrUpdateAttendance(AttendanceTable attendance)
        {
            return _ibiointerface.AddOrUpdateAttendance(attendance);
        }

        [HttpDelete]
        public IActionResult DeleteByattendanceId(int id)
        {
            var result = _ibiointerface.DeleteByattendanceId(id);
            return Ok();
        }

        #endregion



        #region EquipmentEnrollment

        [HttpGet]
        public List<EquipmentEnrollmentModel> GetallEquipmentEnrollments()
        {
            return _ibiointerface.GetallEquipmentEnrollments();

        }

        [HttpGet]

        public ActionResult<EquipmentEnrollmentModel> GetEquipmentEnrollmentbyid(int id)
        {
            var result = _ibiointerface.GetEquipmentEnrollmentbyid(id);
            return result;
        }


        [HttpPost]

        public EquipmentEnrollment AddequipmentEnrollment( EquipmentEnrollment equipment)
        {
           var result= _ibiointerface.AddEquipmentEnrollment(equipment);
            return result;
        }


        [HttpDelete]

        public ActionResult<EquipmentEnrollment> DeleteEquipmentEnrollmentbyid(int id)
        {
            var result = _ibiointerface.deleteEquipmentbyid(id);
            return result;
        }
        #endregion



        #region HealthProgressTracking

        [HttpGet]

        public List<HealthProgressTrackingModel> GetallHealthProgressTrackings()
        {
            var result = _ibiointerface.GetallHealthProgressTrackings();
            return result;

        }

        [HttpGet]

        public ActionResult<HealthProgressTrackingModel> GetbyidHealthProgressTracking(int id)
        {
            var result = _ibiointerface.GetbyidHealthProgressTracking(id);
            return result;
        }

        [HttpPost]
        public HealthProgressTracking Addhealthprogresstracking(HealthProgressTracking healthProgress)
        {
            var result= _ibiointerface.AddHealthProgressTracking(healthProgress);
            return result;
        }
       

        [HttpDelete]

        public ActionResult <HealthProgressTracking> deleteHealthprogresstrackingbyid(int id)
        {
            var result=_ibiointerface.deleteHealthprogresstrackingbyid(id);
            return result;
        }

        #endregion
        #region ServiceMaster
        [HttpGet]

        public List<ServiceMaster> GetAllServiceMaster()
        {
            var result = _ibiointerface.GetAllServiceMaster();
            return result;

        }
        [HttpGet]

        public ActionResult<ServiceMaster> GetServiceMasterbyID(int id)
        {
            var result = _ibiointerface.GetServiceMasterbyID(id);
            return result;
        }
        [HttpPost]
        public ServiceMaster AddOrUpdateServiceMaster(ServiceMaster service)
        {
            var result = _ibiointerface.AddOrUpdateServiceMaster(service);
            return result;
        }


        [HttpDelete]

        public ActionResult<ServiceMaster> DeleteServiceMasterbyId(int id)
        {
            var result = _ibiointerface.DeleteServiceMasterbyId(id);
            return Ok();
        }

        #endregion

    }
}
 