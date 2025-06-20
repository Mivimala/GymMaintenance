using GymMaintenance.DAL.Interface;
using GymMaintenance.Model.Entity;
using GymMaintenance.Controllers;
using GymMaintenance.Model.ViewModel;
using System.IO;
using Microsoft.EntityFrameworkCore;
using GymMaintenance.Data;

namespace GymMaintenance.DAL.Services
{
    public class BioService:IBioInterface
    {
        private readonly BioContext _bioContext;
        public BioService(BioContext bioContext)

        {
            _bioContext = bioContext;
        }

        public List<Login> GetAll()
        {
            var result = _bioContext.Login.ToList();

            if (result.Any())
            {
                return result;
            }
            else

                return new List<Login>();
            //Tiva
            //new
            //Arthi

        }
    }
}
