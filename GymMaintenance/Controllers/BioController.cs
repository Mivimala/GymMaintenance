using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GymMaintenance.Model.Entity;
using GymMaintenance.Data;
using GymMaintenance.DAL.Interface;

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

    }
}
