using Emgu.CV.Util;
using GymMaintenance.DAL.Interface;
using GymMaintenance.DAL.Services;
using GymMaintenance.Data;
using GymMaintenance.Model.Entity;
using GymMaintenance.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Neurotec.Biometrics.Client;
using Neurotec.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GymMaintenance.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class BioController : ControllerBase
    {
        public readonly BioContext _ibioContext;
        public readonly IBioInterface _ibiointerface;
        private readonly IMemoryCache _cache;
        private readonly NBiometricClient _biometricClient;

        public BioController(BioContext bioContext, IBioInterface bioInterface, IMemoryCache cache, NBiometricClient biometricClient)
        {
            _ibioContext = bioContext;
            _ibiointerface= bioInterface;
            _ibiointerface = bioInterface;
            _cache = cache;
           
        }
        [HttpPost("match")]
        public async Task<IActionResult> MatchFingerprint([FromBody] string probeBase64)
        {
            var result = await _ibiointerface.MatchAndMarkAttendanceAsync(probeBase64);

            if (!result.matched)
                return BadRequest(new { matched = false, message = result.message });

            return Ok(new
            {
                matched = true,
                message = result.message,
                candidateId = result.candidateId,
                name = result.name,
                inTime = result.inTime
            });
        }


        #region imageuploadbase64
        [HttpPost]
        //public async Task<IActionResult> CreateTemplate([FromBody] Imageupload dto)
        //{
        //    if (string.IsNullOrWhiteSpace(dto.Image))
        //        return BadRequest("Image is required.");

        //    var imageBytes = await _ibiointerface.ConvertBase64ToTemplateAsync(dto.Image);

        //    return File(imageBytes, "image/png");
        //}

        //[HttpPost]
        //public async Task<(bool, string)> VerifyFingerprintVim(string base64Image)
        //{
        //   return await _ibiointerface.VerifyFingerprintVim(base64Image);
        //}

        [HttpPost]
        public async Task<IActionResult> VerifyByFingerprint([FromBody] FingerprintRequestModel request)
        {
            var result = await _ibiointerface.VerifyFingerprintByImageAsync(request.Base64Image);
            return result.success ? Ok(result.message) : BadRequest(result.message);
         }

        [HttpPost]
        public async Task<IActionResult> VerifyByCandidate([FromBody] CandidateIdRequestModel request)
        {
            var result = await _ibiointerface.VerifyAttendanceByCandidateIdAsync(request.CandidateId);
            return result.success ? Ok(result.message) : BadRequest(result.message);
        }
        [HttpPost]
        public async Task< IActionResult> VerifyFingerprintAsync1(string? base64Image, int? candidateId)
        { 
            return await _ibiointerface.VerifyFingerprintAsync1(base64Image, candidateId);
        }
        #endregion

        #region Login
        [HttpPost]
        public async Task<LoginModel> AuthenticateTrainerLoginAsync(string username, string password)
        {
            return await _ibiointerface.AuthenticateTrainerLoginAsync(username, password);
        }
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _ibioContext.Login
                        .FirstOrDefault(x => x.UserName == login.UserName && x.Password == login.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");
            var sessionId = Guid.NewGuid().ToString();
            _cache.Set(sessionId, user.UserName, TimeSpan.FromMinutes(30)); 
            return Ok(new { sessionId, message = "Login success" });
        }

        [HttpGet]
        public List<LoginModel> GetAllLogin()
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
        public (Payment? payment, string message) AddpaymentMail(Payment pymnnt, string phone)
        {
            return _ibiointerface.AddpaymentMail(pymnnt, phone);
        }
        //[HttpPost]

        //public Task<LoginModel> AuthenticateTrainerLoginAsync(string username, string password)
        //{
        //    return _ibiointerface.AuthenticateTrainerLoginAsync(username, password);
        //}
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
        public async Task<ActionResult<FingerPrintModel>> AddFingerPrintAsync([FromBody] FingerPrintModel fingerprintDto)
        {
            if (fingerprintDto == null)
                return BadRequest("Invalid fingerprint data.");

            var result = await _ibiointerface.AddFingerPrintAsync(fingerprintDto);
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult DeleteByfingerprintId(int id)
        {
            var result = _ibiointerface.DeleteByfingerprintId(id);
            return Ok();
        }

        //[HttpPost]
        //public IActionResult SaveFingerprint([FromBody] FingerPrintModel model)
        //{
        //    return _ibiointerface.SaveFingerprint(model);
        //}



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
        public (Payment? payment, string message) Addpayment(Payment pymnnt)
        {
           return _ibiointerface.Addpayment(pymnnt);  
        }

        [HttpDelete]
        public IActionResult DeleteBypymntId(int id)
        {
            var result = _ibiointerface.DeleteBypymntId(id);
            return Ok();
        }
        //[HttpPost]

        //public Task<LoginModel> AuthenticateTrainerLoginAsync(string username, string password)
        //{
        //    return _ibiointerface.AuthenticateTrainerLoginAsync(username, password);
        //}
        #endregion


        #region Trainer

        [HttpGet]
        public List<TrainerEnrollmentModel> GetAlltrainer()
        {
            return _ibiointerface.GetAlltrainer();
        }

        [HttpGet]
        public IActionResult SearchTrainerEnroll([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required" });

            var results = _ibiointerface.SearchTrainerEnrollByName(keyword);

            if (results.Count == 0)
                return NotFound(new { message = "No equipment found" });

            return Ok(results);
        }

        [HttpGet("{id:int}")]

        public ActionResult<TrainerEnrollmentModel> GetAlltrainerbyID(int id)
        {
            var result = _ibiointerface.GetAlltrainerbyID(id);

            return result;
        }

        [HttpPost]
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
        public IActionResult SearchCandidateEnroll([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required" });

            var results = _ibiointerface.SearchCandidateEnrollByName(keyword);

            if (results.Count == 0)
                return NotFound(new { message = "No equipment found" });

            return Ok(results);
        }

        [HttpGet]

        public ActionResult<CandidateEnrollModel> GetAllcandidatebyID(int id)
        {
            var result = _ibiointerface.GetAllcandidatebyID(id);

            return result;
        }
        [HttpPost]
        public ActionResult<CandidateEnrollment> AddOrUpdateCandidate([FromBody] CandidateEnrollModel candidateModel)
        {
            if (candidateModel == null)
                return BadRequest("Candidate data is required.");

            var result = _ibiointerface.AddOrUpdateCandidate(candidateModel);
            return Ok(result);
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
        [HttpPost]
        public AttendanceTable AddOrUpdateAttendanceNEW(AttendanceTableModel attendanceTableModel)
        {
            return _ibiointerface.AddOrUpdateAttendanceNEW(attendanceTableModel);
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

        #region servicetable
        

        [HttpGet]

        public List<Servicetable> GetallServicetable()
        {
            var result = _ibiointerface.GetallServicetable();
            return result;
        }

       
        [HttpGet]
        public ActionResult<Servicetable> GetbyidServicetable(int id)
        {
            var result = _ibiointerface.GetbyidServicetable(id);
            if (result == null)
            {
                return NotFound();

            }
            else
            {
                return result;
            }

        }

        [HttpPost]
        public Servicetable AddServicetable(Servicetable servicetables)
        {
            var result = _ibiointerface.AddServicetable(servicetables);
            return result;
        }



        [HttpDelete]

        public ActionResult<Servicetable> Deletebyidservicetable(int id)
        {
            var result = _ibiointerface.DeletebyidServicetable(id);

            return result;
        }


        #endregion

        #region packagetable

        [HttpGet]

        public List<Packagetable> GetAllPackagetable()
        {
            var result = _ibiointerface.GetallPackagetable();
            return result;
        }


        [HttpGet]
        public ActionResult<Packagetable> getbyidpackagetable(int id)
        {
            var result = _ibiointerface.GetbyidPackagetable(id);
            if (result == null)
            {

                return NotFound();

            }
            else
            {
                return result;
            }

        }


        [HttpPost]
        public Packagetable AddPackagetable(Packagetable packagetable)
        {
            var result = _ibiointerface.AddPackagetable(packagetable);
            return result;

        }

        [HttpDelete]

        public ActionResult<Packagetable> Deletebyidpackagetable(int id)
        {
            var result = _ibiointerface.DeletebyidPackagetable(id);

            return result;
        }
        #endregion

        [HttpPost]
        public IActionResult GetAlerts(AlertModel alertModel)
        {
            var result = _ibiointerface.GetAlerts(alertModel);
            return Ok(new
            {
                message = alertModel.IsAlert ? "Buzzed for 2 seconds" : "Short buzzed",
                result
            });

        }


        #region GetPaymentReportByDate




        [HttpGet]
        //public IActionResult GetPaymentReportByDate([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        //{
        //    if (!DateOnly.TryParse(fromDate, out var from) || !DateOnly.TryParse(toDate, out var to))
        //    {
        //        return BadRequest("Invalid date format. Use yyyy-MM-dd.");
        //    }

        //    var data = _ibiointerface.GetPaymentReportByDate(from, to);

        //    if (data == null || data.Count == 0)
        //        return NotFound();

        //    return Ok(data);
        //}
        [HttpGet("GetPaymentReportByDate")]
        public IActionResult GetPaymentReportByDate([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            // Convert DateTime to DateOnly
            var from = DateOnly.FromDateTime(fromDate);
            var to = DateOnly.FromDateTime(toDate);

            if (from > to)
            {
                return BadRequest("From date must be earlier than or equal to To date.");
            }

            var data = _ibiointerface.GetPaymentReportByDate(fromDate, toDate);

            if (data == null || data.Count == 0)
                return NotFound("No payment records found in the given date range.");

            return Ok(data);
        }



        #endregion

        #region GetCandidateReportByDate

        [HttpGet]
        public async Task<IActionResult> GetCandidateReportByDate(DateTime fromDate, DateTime toDate)
        {
            var data = await _ibiointerface.GetCandidateReportByDate(fromDate, toDate);

            if (data == null || data.Count == 0)
            {
                return NotFound();
            }

            return Ok(data);
        }

        #endregion


        #region GetAttendanceReportByDate
        [HttpGet]
        public async Task<IActionResult> GetAttendanceReportByDate(DateTime fromDate, DateTime toDate)
        {
            var data = await _ibiointerface.GetAttendanceReportByDate(fromDate, toDate);

            if (data == null || data.Count == 0)
            {
                return NotFound();
            }

            return Ok(data);
        }


        #endregion


        #region GetTrainerReportByDate


        [HttpGet]
        public IActionResult GetTrainerReportByDate([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            if (!DateOnly.TryParse(fromDate, out var from) || !DateOnly.TryParse(toDate, out var to))
            {
                return BadRequest("Invalid date format. Use yyyy-MM-dd.");
            }

            var data = _ibiointerface.GetTrainerReportByDate(from, to);

            if (data == null || data.Count == 0)
            {
                return NotFound();
            }

            return Ok(data);
        }



        #endregion

    }
}
 