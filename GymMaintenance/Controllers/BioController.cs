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



        [HttpPost("AddLogin")]
        public Login Addlog(Login login)
        {
            return _ibiointerface.Addlog(login);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteById(int id)
        {
            var result = _ibiointerface.DeleteById(id);
            if (!result)
                return NotFound($"No login found with ID = {id}");

            return Ok($"Login with ID = {id} deleted successfully");
        }


        [HttpPost("AddFingerprint")]
        public FingerPrint AddFingerPrint(FingerPrint fingerprint)
        {
            return _ibiointerface.AddFingerPrint(fingerprint);
        }

        [HttpDelete("fingerprint/{id:int}")]
        public IActionResult DeleteByfingerprintId(int id)
        {
            var result = _ibiointerface.DeleteByfingerprintId(id);
            if (!result)
                return NotFound($"No login found with ID = {id}");

            return Ok($"Login with ID = {id} deleted successfully");
        }


        [HttpPost("Addpayment")]
        public Payment Addpayment(Payment pymnnt)
        {
            return _ibiointerface.Addpayment(pymnnt);
        }

        [HttpDelete("payment/{id:int}")]
        public IActionResult DeleteBypymntId(int id)
        {
            var result = _ibiointerface.DeleteBypymntId(id);
            if (!result)
                return NotFound($"No login found with ID = {id}");

            return Ok($"Login with ID = {id} deleted successfully");
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



        [HttpPost("Addcandidate")]
        public CandidateEnroll AddOrUpdateCandidate(CandidateEnroll candidate)
        {
            return _ibiointerface.AddOrUpdateCandidate(candidate);
        }

        [HttpDelete("candidate/{id:int}")]
        public IActionResult DeleteBycandidateId(int id)
        {
            var result = _ibiointerface.DeleteBycandidateId(id);
            if (!result)
                return NotFound($"No login found with ID = {id}");

            return Ok($"Login with ID = {id} deleted successfully");
        }

       

        [HttpPost("Addattendance")]
        public AttendanceTable AddOrUpdateAttendance(AttendanceTable attendance)
        {
            return _ibiointerface.AddOrUpdateAttendance(attendance);
        }

        [HttpDelete("attendance/{id:int}")]
        public IActionResult DeleteByattendanceId(int id)
        {
            var result = _ibiointerface.DeleteByattendanceId(id);
            if (!result)
                return NotFound($"No login found with ID = {id}");

            return Ok($"Login with ID = {id} deleted successfully");
        }


        [HttpGet]
        public List<LoginModel> GetAllLoginlog()
        {
            return _ibiointerface.GetAllLogin();
        }
        
        [HttpGet]
        public List<CandidateEnrollModel> GetAllcandidate()
        {
            return _ibiointerface.GetAllcandidate();
        }
        [HttpGet]
        public List<TrainerEnrollmentModel> GetAlltrainer()
        {
            return _ibiointerface.GetAlltrainer();
        }
        [HttpGet]
        public List<AttendanceTableModel> GetAllAttendance()
        {
            return _ibiointerface.GetAllAttendance();
        }
        [HttpGet]
        public List<PaymentModel> GetAllpayment()
        {
            return _ibiointerface.GetAllpayment();
        }

        [HttpGet("{id:int}")]

        public ActionResult<LoginModel> GetLoginById(int id)
        {
            var result = _ibiointerface.GetLoginById(id);

            return result;
        }

        [HttpGet("{id:int}")]

        public ActionResult<FingerPrintModel> GetAllfingerprintbyID(int id)
        {
            var result = _ibiointerface.GetAllfingerprintbyID(id);

            return result;
        }
        [HttpGet("{id:int}")]

        public ActionResult<PaymentModel> GetAllpaymentbyId(int id)
        {
            var result = _ibiointerface.GetAllpaymentbyId(id);

            return result;
        }

       
        [HttpGet("{id:int}")]

        public ActionResult<TrainerEnrollmentModel> GetAlltrainerbyID(int id)
        {
            var result = _ibiointerface.GetAlltrainerbyID(id);

            return result;
        }
        [HttpGet("{id:int}")]

        public ActionResult<CandidateEnrollModel> GetAllcandidatebyID(int id)
        {
            var result = _ibiointerface.GetAllcandidatebyID(id);

            return result;
        }
        [HttpGet("{id:int}")]

        public ActionResult<AttendanceTableModel> GetAllAttendancebyID(int id)
        {
            var result = _ibiointerface.GetAllAttendancebyID(id);

            return result;
        }


        #region EquipmentEnrollment

        [HttpPost("AddEnrollequipment")]

        public EquipmentEnrollment AddequipmentEnrollment( EquipmentEnrollment equipment)
        {
           var result= _ibiointerface.AddEquipmentEnrollment(equipment);
            return result;
        }


        [HttpGet("getallequipmentenrollments")]
        public List<EquipmentEnrollmentModel> GetallEquipmentEnrollments ()
        {
            return _ibiointerface.GetallEquipmentEnrollments();

        }

        [HttpGet("{id}")]

        public ActionResult<EquipmentEnrollmentModel> GetEquipmentEnrollmentbyid (int id)
        {
            var result =_ibiointerface.GetEquipmentEnrollmentbyid(id);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        [HttpDelete("DeleteEquipmentEnrollmentbyid{id}")]

        public ActionResult<EquipmentEnrollment> DeleteEquipmentEnrollmentbyid(int id)
        {
            var result = _ibiointerface.deleteEquipmentbyid(id);

            return result;
        }
        #endregion






        #region HealthProgressTracking

        [HttpPost("AddHealthProgressTracking")]
        public HealthProgressTracking Addhealthprogresstracking(HealthProgressTracking healthProgress)
        {
            var result= _ibiointerface.AddHealthProgressTracking(healthProgress);
            return result;
        }
        [HttpGet("Getallhealthprogresstracking")]

        public List<HealthProgressTrackingModel> GetallHealthProgressTrackings()
        {
            var result = _ibiointerface.GetallHealthProgressTrackings();
            return result;

        }

        [HttpGet("getbyidhealthprogress{id}")]

        public ActionResult <HealthProgressTrackingModel> GetbyidHealthProgressTracking (int id)
        {
            var result=_ibiointerface.GetbyidHealthProgressTracking(id);

            return result;
        }

        [HttpDelete("deleteHealthprogresstrackingbyid{id}")]

        public ActionResult <HealthProgressTracking> deleteHealthprogresstrackingbyid(int id)
        {
            var result=_ibiointerface.deleteHealthprogresstrackingbyid(id);

            return result;
        }

        #endregion




    }
}
 