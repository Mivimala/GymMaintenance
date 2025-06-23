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


        [HttpGet("GetAllLogin")]
        public List<Login> GetAll()
        {
            return _ibiointerface.GetAll();
        }

        [HttpGet("{id:int}")]
        public ActionResult<Login> GetById(int id)
        {
            var login = _ibiointerface.GetById(id);

            if (login == null)
            {
                return NotFound(); 
            }

            return Ok(login); 
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

        [HttpGet("GetAllFingerPrints")]
        public List<FingerPrint> GetAllfingreprint()
        {
            return _ibiointerface.GetAllfingreprint();
        }

        [HttpGet("fingerprint/{id:int}")]
        public ActionResult<FingerPrint> GetByfingerprintId(int id)
        {
            var FingerPrint = _ibiointerface.GetByfingerprintId(id);

            if (FingerPrint == null)
            {
                return NotFound();
            }

            return Ok(FingerPrint);
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

        [HttpGet("GetAllpayment")]
        public List<Payment> GetAllpaymnent()
        {
            return _ibiointerface.GetAllpaymnent();
        }

        [HttpGet("payment/{id:int}")]
        public ActionResult<Payment> GetBypaymentid(int id)
        {
            var Payment = _ibiointerface.GetBypaymentid(id);

            if (Payment == null)
            {
                return NotFound();
            }

            return Ok(Payment);
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

        [HttpGet("GetAlltrainer")]
        public List<TrainerEnrollment> GetAlltrainer()
        {
            return _ibiointerface.GetAlltrainer();
        }

        [HttpGet("trainer/{id:int}")]

        public ActionResult<TrainerEnrollment> GetBytrainerid(int id)
        {
            var TrainerEnrollment = _ibiointerface.GetBytrainerid(id);

            if (TrainerEnrollment == null)
            {
                return NotFound();
            }

            return Ok(TrainerEnrollment);
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


        [HttpGet("GetAllcandidate")]
        public List<CandidateEnroll> GetAllcandidate()
        {
            return _ibiointerface.GetAllcandidate();
        }

        [HttpGet("candidate/{id:int}")]
        public ActionResult<CandidateEnroll> GetBycandidateid(int id)
        {
            var CandidateEnroll = _ibiointerface.GetBycandidateid(id);

            if (CandidateEnroll == null)
            {
                return NotFound();
            }

            return Ok(CandidateEnroll);
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

        [HttpGet("GetAllattendance")]
        public List<AttendanceTable> GetAllattendance()
        {
            return _ibiointerface.GetAllattendance();
        }

        [HttpGet("attendance/{id:int}")]
      
        public ActionResult<AttendanceTable> GetByattendanceid(int id)
        {
            var AttendanceTable = _ibiointerface.GetByattendanceid(id);

            if (AttendanceTable == null)
            {
                return NotFound();
            }

            return Ok(AttendanceTable);
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
            return _ibiointerface.GetAllLoginlog();
        }



    }
}
 