using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GymMaintenance.Model.Entity;
using GymMaintenance.Data;
using GymMaintenance.DAL.Interface;
using GymMaintenance.Model.ViewModel;

namespace GymMaintenance.Controllers
{
    [Route("api/[controller]")]
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


        [HttpGet]
        public List<Login> GetAll()
        {
            return _ibiointerface.GetAll();
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
