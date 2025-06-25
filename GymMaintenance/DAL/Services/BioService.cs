using GymMaintenance.DAL.Interface;
using GymMaintenance.Model.Entity;
using GymMaintenance.Controllers;
using GymMaintenance.Model.ViewModel;
using System.IO;
using Microsoft.EntityFrameworkCore;
using GymMaintenance.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

namespace GymMaintenance.DAL.Services
{
    public class BioService : IBioInterface
    {
        private readonly BioContext _bioContext;
        private readonly IMemoryCache _cache;
        public BioService(BioContext bioContext, IMemoryCache cache)

        {
            _bioContext = bioContext;
            _cache = cache;
        }




        #region Login

        public List<LoginModel> GetAllLogin()
        {
            var result = (from a in _bioContext.Login
                          select new
                          {
                              a.Role,
                              a.LoginId,
                              a.UserName,
                              a.Password
                          }).AsEnumerable().Select(x => new LoginModel
                          {
                              LoginId = x.LoginId,
                              Role = x.Role,
                              UserName = x.UserName,
                              Password = x.Password
                          }).ToList();
            return result;
        }
        public LoginModel GetLoginById(int id)
        {
            var result = (from a in _bioContext.Login
                          where a.LoginId == id
                          select new LoginModel
                          {
                              LoginId = a.LoginId,
                              Role = a.Role,
                              UserName = a.UserName,
                              Password = a.Password
                          }).FirstOrDefault();

            return result;
        }

        public Login Addlog(Login login)
        {
            var result = _bioContext.Login.Where(x => x.LoginId == login.LoginId).FirstOrDefault();
            if (result == null)
            {
                result = new Login();
                result.Role = login.Role;
                result.UserName = login.UserName;
                result.Password = login.Password;
                _bioContext.Login.Add(result);
            }
            else
            {
                result.LoginId = login.LoginId;
                result.Role = login.Role;
                result.UserName = login.UserName;
                result.Password = login.Password;
                _bioContext.Login.Update(result);
            }
            _bioContext.SaveChanges();
            return result;
        }

        public bool DeleteById(int id)
        {
            var entity = _bioContext.Login.Find(id);
            if (entity == null) return false;

            _bioContext.Login.Remove(entity);
            _bioContext.SaveChanges();
            return true;
        }
        #endregion
        #region  FingerPrint
        
        public List<FingerPrintModel> GetAllfingerprint()
        {
            var result = (from a in _bioContext.FingerPrint
                          select new
                          {
                              a.FingerPrintID,
                              a.Role,
                              a.FingerPrint1,
                              a.FingerPrint2,
                              a.FingerPrint3
                          }).AsEnumerable().Select(x => new FingerPrintModel
                          {
                              FingerPrintID = x.FingerPrintID,
                              Role = x.Role,
                              FingerPrint1 = x.FingerPrint1,
                              FingerPrint2 = x.FingerPrint2,
                              FingerPrint3 = x.FingerPrint3
                          }).ToList();
            return result;
        }
        public FingerPrintModel GetAllfingerprintbyID(int id)
        {
            var result = (from a in _bioContext.FingerPrint where a.FingerPrintID==id
                          select new FingerPrintModel
                          {
                              FingerPrintID=a.FingerPrintID,
                              Role=a.Role,
                              FingerPrint1=a.FingerPrint1,
                              FingerPrint2=a.FingerPrint2,
                              FingerPrint3=a.FingerPrint3
                          }).FirstOrDefault();
            return result;
        }
       
        public FingerPrint AddFingerPrint(FingerPrint fingerprint)
        {
            var result = _bioContext.FingerPrint.Where(x => x.FingerPrintID == fingerprint.FingerPrintID).FirstOrDefault();
            if (result == null)
            {
                result = new FingerPrint();
                result.Role = fingerprint.Role;
                result.FingerPrint1 = fingerprint.FingerPrint1;
                result.FingerPrint2 = fingerprint.FingerPrint2;
                result.FingerPrint3 = fingerprint.FingerPrint3;
                result.CreatedDate = fingerprint.CreatedDate;
                _bioContext.FingerPrint.Add(result);
            }
            else
            {
                result.FingerPrintID = fingerprint.FingerPrintID;
                result.Role = fingerprint.Role;
                result.FingerPrint1 = fingerprint.FingerPrint1;
                result.FingerPrint2 = fingerprint.FingerPrint2;
                result.FingerPrint3 = fingerprint.FingerPrint3;
                result.CreatedDate = fingerprint.CreatedDate;
                _bioContext.FingerPrint.Update(result);
            }
            _bioContext.SaveChanges();
            return result;
        }


        public bool DeleteByfingerprintId(int id)
        {
            var entity = _bioContext.FingerPrint.Find(id);
            if (entity == null) return false;

            _bioContext.FingerPrint.Remove(entity);
            _bioContext.SaveChanges();
            return true;
        }

        #endregion

        #region Candidate
       
        public List<CandidateEnrollModel> GetAllcandidate()
        {
            var result = (from a in _bioContext.CandidateEnroll
                          select new
                          {
                              a.CandidateId,
                              a.Name,
                              a.Gender,
                              a.Weight,
                              a.Height,
                              a.Waist,
                              a.BMI,
                              a.BloodGroup,
                              a.Age,
                              a.CurrentAddress,
                              a.PermanentAddress,
                              a.AadharNumber,
                              a.MobileNumber,
                              a.EmailId,
                              a.Profession,
                              a.Picture,
                              a.FingerPrintID,
                              a.IsActive,
                              a.CreatedDate,
                          }).AsEnumerable().Select(x => new CandidateEnrollModel
                          {
                              CandidateId = x.CandidateId,
                              Name = x.Name,
                              Gender = x.Gender,
                              Weight = x.Weight,
                              Height = x.Height,
                              Waist = x.Waist,
                              BMI = x.BMI,
                              BloodGroup = x.BloodGroup,
                              Age = x.Age,
                              CurrentAddress = x.CurrentAddress,
                              PermanentAddress = x.PermanentAddress,
                              AadharNumber = x.AadharNumber,
                              MobileNumber = x.MobileNumber,
                              EmailId = x.EmailId,
                              Profession = x.Profession,
                              Picture = x.Picture,
                              FingerPrintID = x.FingerPrintID,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate,
                          }).ToList();
            return result;
        }
        public CandidateEnrollModel GetAllcandidatebyID(int id)
        {
            var result = (from x in _bioContext.CandidateEnroll
                          where x.CandidateId == id
                          select new CandidateEnrollModel
                          {

                              CandidateId = x.CandidateId,
                              Name = x.Name,
                              Gender = x.Gender,
                              Weight = x.Weight,
                              Height = x.Height,
                              Waist = x.Waist,
                              BMI = x.BMI,
                              BloodGroup = x.BloodGroup,
                              Age = x.Age,
                              CurrentAddress = x.CurrentAddress,
                              PermanentAddress = x.PermanentAddress,
                              AadharNumber = x.AadharNumber,
                              MobileNumber = x.MobileNumber,
                              EmailId = x.EmailId,
                              Profession = x.Profession,
                              Picture = x.Picture,
                              FingerPrintID = x.FingerPrintID,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate,
                          }).FirstOrDefault();
            return result;
        }

        public CandidateEnroll AddOrUpdateCandidate(CandidateEnroll candidate)
        {
            var result = _bioContext.CandidateEnroll
                .FirstOrDefault(c => c.CandidateId == candidate.CandidateId);

            if (result == null)
            {

                result = new CandidateEnroll
                {
                    Name = candidate.Name,
                    Gender = candidate.Gender,
                    Weight = candidate.Weight,
                    Height = candidate.Height,
                    BMI = candidate.Weight / (candidate.Height * candidate.Height),
                    BloodGroup = candidate.BloodGroup,
                    Age = candidate.Age,
                    CurrentAddress = candidate.CurrentAddress,
                    PermanentAddress = candidate.PermanentAddress,
                    AadharNumber = candidate.AadharNumber,
                    MobileNumber = candidate.MobileNumber,
                    EmailId = candidate.EmailId,
                    Profession = candidate.Profession,
                    Picture = candidate.Picture,
                    FingerPrintID = candidate.FingerPrintID,
                    IsActive = candidate.IsActive,
                    CreatedDate = candidate.CreatedDate
                };
                _bioContext.CandidateEnroll.Add(result);
            }
            else
            {
                result.CandidateId = candidate.CandidateId;
                result.Name = candidate.Name;
                result.Gender = candidate.Gender;
                result.Weight = candidate.Weight;
                result.Height = candidate.Height;
                result.BMI = candidate.Weight / (candidate.Height * candidate.Height);
                result.BloodGroup = candidate.BloodGroup;
                result.Age = candidate.Age;
                result.CurrentAddress = candidate.CurrentAddress;
                result.PermanentAddress = candidate.PermanentAddress;
                result.AadharNumber = candidate.AadharNumber;
                result.MobileNumber = candidate.MobileNumber;
                result.EmailId = candidate.EmailId;
                result.Profession = candidate.Profession;
                result.Picture = candidate.Picture;
                result.FingerPrintID = candidate.FingerPrintID;
                result.IsActive = candidate.IsActive;
                result.CreatedDate = candidate.CreatedDate;

                _bioContext.CandidateEnroll.Update(result);
            }

            _bioContext.SaveChanges();
            return result;
        }

        public bool DeleteBycandidateId(int id)
        {
            var entity = _bioContext.CandidateEnroll.Find(id);
            if (entity == null) return false;

            _bioContext.CandidateEnroll.Remove(entity);
            _bioContext.SaveChanges();
            return true;
        }

        #endregion
        #region Trainer
       
        public List<TrainerEnrollmentModel> GetAlltrainer()
        {
            var result = (from a in _bioContext.TrainerEnrollment
                          select new
                          {
                              a.TrainerId,
                              a.Password,
                              a.Name,
                              a.Gender,
                              a.BloodGroup,
                              a.FingerPrintID,
                              a.Age,
                              a.CurrentAddress,
                              a.PermanentAddress,
                              a.AadharNumber,
                              a.MobileNumber,
                              a.EmailId,
                              a.EmploymentType,
                              a.Experience,
                              a.Qualification,
                              a.JoiningDate,
                              a.Picture,
                              a.IsActive,
                              a.CreatedDate
                          }).AsEnumerable().Select(x => new TrainerEnrollmentModel
                          {
                              TrainerId = x.TrainerId,
                              Password = x.Password,
                              Name = x.Name,
                              Gender = x.Gender,
                              BloodGroup = x.BloodGroup,
                              FingerPrintID = x.FingerPrintID,
                              Age = x.Age,
                              CurrentAddress = x.CurrentAddress,
                              PermanentAddress = x.PermanentAddress,
                              AadharNumber = x.AadharNumber,
                              MobileNumber = x.MobileNumber,
                              EmailId = x.EmailId,
                              EmploymentType = x.EmploymentType,
                              Experience = x.Experience,
                              Qualification = x.Qualification,
                              JoiningDate = x.JoiningDate,
                              Picture = x.Picture,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate
                          }).ToList();
            return result;
        }
        public TrainerEnrollmentModel GetAlltrainerbyID(int id)
        {
            var result = (from x in _bioContext.TrainerEnrollment where x.TrainerId == id 
                          select new  TrainerEnrollmentModel
                          {
                            
                              TrainerId = x.TrainerId,
                              Password = x.Password,
                              Name = x.Name,
                              Gender = x.Gender,
                              BloodGroup = x.BloodGroup,
                              FingerPrintID = x.FingerPrintID,
                              Age = x.Age,
                              CurrentAddress = x.CurrentAddress,
                              PermanentAddress = x.PermanentAddress,
                              AadharNumber = x.AadharNumber,
                              MobileNumber = x.MobileNumber,
                              EmailId = x.EmailId,
                              EmploymentType = x.EmploymentType,
                              Experience = x.Experience,
                              Qualification = x.Qualification,
                              JoiningDate = x.JoiningDate,
                              Picture = x.Picture,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate
                          }).FirstOrDefault();
            return result;
        }



        public TrainerEnrollment AddOrUpdateTrainer(TrainerEnrollment trainer)
        {
            var result = _bioContext.TrainerEnrollment
                .FirstOrDefault(x => x.TrainerId == trainer.TrainerId);

            if (result == null)
            {
                result = new TrainerEnrollment();

                result.Password = trainer.Password;
                result.Name = trainer.Name;
                result.Gender = trainer.Gender;
                result.BloodGroup = trainer.BloodGroup;
                result.Age = trainer.Age;
                result.CurrentAddress = trainer.CurrentAddress;
                result.PermanentAddress = trainer.PermanentAddress;
                result.AadharNumber = trainer.AadharNumber;
                result.MobileNumber = trainer.MobileNumber;
                result.EmailId = trainer.EmailId;
                result.EmploymentType = trainer.EmploymentType;
                result.Experience = trainer.Experience;
                result.Qualification = trainer.Qualification;
                result.JoiningDate = trainer.JoiningDate;
                result.Picture = trainer.Picture;
                result.FingerPrintID = trainer.FingerPrintID;
                result.IsActive = trainer.IsActive;
                result.CreatedDate = trainer.CreatedDate;


                _bioContext.TrainerEnrollment.Add(result);
            }
            else
            {
                result.TrainerId = trainer.TrainerId;
                result.Password = trainer.Password;
                result.Name = trainer.Name;
                result.Gender = trainer.Gender;
                result.BloodGroup = trainer.BloodGroup;
                result.Age = trainer.Age;
                result.CurrentAddress = trainer.CurrentAddress;
                result.PermanentAddress = trainer.PermanentAddress;
                result.AadharNumber = trainer.AadharNumber;
                result.MobileNumber = trainer.MobileNumber;
                result.EmailId = trainer.EmailId;
                result.EmploymentType = trainer.EmploymentType;
                result.Experience = trainer.Experience;
                result.Qualification = trainer.Qualification;
                result.JoiningDate = trainer.JoiningDate;
                result.Picture = trainer.Picture;
                result.FingerPrintID = trainer.FingerPrintID;
                result.IsActive = trainer.IsActive;
                result.CreatedDate = trainer.CreatedDate;

                _bioContext.TrainerEnrollment.Update(result);
            }

            _bioContext.SaveChanges();
            return result;
        }

        public bool DeleteBytrainerId(int id)
        {
            var entity = _bioContext.TrainerEnrollment.Find(id);
            if (entity == null) return false;

            _bioContext.TrainerEnrollment.Remove(entity);
            _bioContext.SaveChanges();
            return true;
        }

        
        #endregion

        #region Payment
       

        public List<PaymentModel> GetAllpayment()
        {
            var result = (from a in _bioContext.Payment
                          select new
                          {
                              a.PaymentReceiptNo,
                              a.MemmberId,
                              a.Name,
                              a.MobileNumber,
                              a.ServiceId,
                              a.Package,
                              a.TimeSlot,
                              a.PlanStartingDate,
                              a.PlanExpiringDate,
                              a.PlanAmount,
                              a.BalanceAmount,
                              a.CurrentPayment,
                              a.ModeOfPayment,
                              a.collectedby,
                              a.IsActive,
                              a.CreatedDate
                             
                          }).AsEnumerable().Select(x => new PaymentModel
                          {
                              PaymentReceiptNo = x.PaymentReceiptNo,
                              MemmberId = x.MemmberId,
                              Name = x.Name,
                              MobileNumber = x.MobileNumber,
                              ServiceId = x.ServiceId,
                              Plan = x.Package,
                              TimeSlot = x.TimeSlot,
                              PlanStartingDate = x.PlanStartingDate,
                              PlanExpiringDate = x.PlanExpiringDate,
                              PlanAmount = x.PlanAmount,
                              BalanceAmount = x.BalanceAmount,
                              CurrentPayment = x.CurrentPayment,
                              ModeOfPayment = x.ModeOfPayment,
                              collectedby=x.collectedby,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate
                             
                          }).ToList();
            return result;
        }
        
        public PaymentModel GetpaymentbyId( int id,int serviceId)
        {
            var result = (from a in _bioContext.Payment where a.MemmberId == id && a.ServiceId==serviceId 
                          select new PaymentModel
                          {
                             
                              PaymentReceiptNo = a.PaymentReceiptNo,
                              MemmberId = a.MemmberId,
                              Name = a.Name,
                              MobileNumber = a.MobileNumber,
                              ServiceId = a.ServiceId,
                              Plan = a.Package,
                              TimeSlot = a.TimeSlot,
                              PlanStartingDate = a.PlanStartingDate,
                              PlanExpiringDate = a.PlanExpiringDate,
                              PlanAmount = a.PlanAmount,
                              BalanceAmount = a.BalanceAmount,
                              CurrentPayment = a.CurrentPayment,
                              ModeOfPayment = a.ModeOfPayment,
                              collectedby = a.collectedby,
                              IsActive = a.IsActive,
                              CreatedDate = a.CreatedDate

                          }).FirstOrDefault();
            return result;
        }

        public Payment Addpayment([FromBody] Payment pymnnt, [FromHeader(Name = "X-Session-ID")] string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))// || !_cache.TryGetValue(sessionId, out string username))
            {
                return null;
            }
            var result = _bioContext.Payment.Where(x => x.PaymentReceiptNo == pymnnt.PaymentReceiptNo).FirstOrDefault();
            if (result == null)
            {
                result = new Payment();
                result.MemmberId = pymnnt.MemmberId;
                result.Name = pymnnt.Name;
                result.MobileNumber = pymnnt.MobileNumber;
                result.ServiceId = pymnnt.ServiceId;
                result.Package = pymnnt.Package;
                result.months= Convert.ToInt32( pymnnt.Package.Substring(0, 1));
                result.TimeSlot = pymnnt.TimeSlot;
                result.PlanStartingDate = (pymnnt.PlanStartingDate);
                result.PlanExpiringDate = pymnnt.PlanStartingDate.AddMonths(result.months);
                result.PlanAmount = pymnnt.PlanAmount;
                result.CurrentPayment = pymnnt.CurrentPayment;
                result.BalanceAmount = pymnnt.PlanAmount - pymnnt.CurrentPayment;
                result.ModeOfPayment = pymnnt.ModeOfPayment;
                result.collectedby = sessionId;
                result.IsActive = pymnnt.IsActive;
                result.CreatedDate = pymnnt.CreatedDate;

                _bioContext.Payment.Add(result);
            }

            else
            {
                result.PaymentReceiptNo = pymnnt.PaymentReceiptNo;
                result.MemmberId = pymnnt.MemmberId;
                result.Name = pymnnt.Name;
                result.MobileNumber = pymnnt.MobileNumber;
                result.ServiceId = pymnnt.ServiceId;
                result.Package = pymnnt.Package;
                result.TimeSlot = pymnnt.TimeSlot;
                result.PlanStartingDate = pymnnt.PlanStartingDate;
                result.months = Convert.ToInt32(pymnnt.Package.Substring(0, 1));
                result.PlanExpiringDate = pymnnt.PlanStartingDate.AddMonths(result.months);
                result.PlanAmount = pymnnt.PlanAmount;
                result.CurrentPayment = pymnnt.CurrentPayment;
                result.BalanceAmount = pymnnt.BalanceAmount-pymnnt.CurrentPayment;
                result.ModeOfPayment = pymnnt.ModeOfPayment;
                result.collectedby = sessionId;
                result.IsActive = pymnnt.IsActive;
                result.CreatedDate = pymnnt.CreatedDate;
                _bioContext.Payment.Update(result);
            }
            _bioContext.SaveChanges();

            return result;
        }

        public bool DeleteBypymntId(int id)
        {
            var entity = _bioContext.Payment.Find(id);
            if (entity == null) return false;

            _bioContext.Payment.Remove(entity);
            _bioContext.SaveChanges();
            return true;
        }
        #endregion


        #region Attendance

        public List<AttendanceTableModel> GetAllAttendance()
        {
            var result = (from a in _bioContext.AttendanceTable
                          select new
                          {
                              a.AttendanceId,
                              a.CandidateId,
                              a.CandidateName,
                              a.FingerPrintID,
                              a.AttendanceDate,
                              a.InTime,
                              a.OutTime


                          }).AsEnumerable().Select(x => new AttendanceTableModel
                          {
                              AttendanceId = x.AttendanceId,
                              CandidateId = x.CandidateId,
                              CandidateName = x.CandidateName,
                              FingerPrintID = x.FingerPrintID,
                              AttendanceDate = x.AttendanceDate,
                              InTime = x.InTime,
                              OutTime = x.OutTime

                          }).ToList();
            return result;
        }

        public AttendanceTableModel GetAllAttendancebyID(int id)
        {
            var result = (from x in _bioContext.AttendanceTable
                          where x.CandidateId == id
                          select new AttendanceTableModel
                          {

                              AttendanceId = x.AttendanceId,
                              CandidateId = x.CandidateId,
                              CandidateName = x.CandidateName,
                              FingerPrintID = x.FingerPrintID,
                              AttendanceDate = x.AttendanceDate,
                              InTime = x.InTime,
                              OutTime = x.OutTime

                          }).FirstOrDefault();
            return result;
        }

        public AttendanceTable AddOrUpdateAttendance(AttendanceTable attendance)
        {
            var result = _bioContext.AttendanceTable
                .FirstOrDefault(a => a.AttendanceId == attendance.AttendanceId);

            if (result == null)
            {
                result = new AttendanceTable
                {
                    CandidateId = attendance.CandidateId,
                    CandidateName = attendance.CandidateName,
                    FingerPrintID = attendance.FingerPrintID,
                    AttendanceDate = attendance.AttendanceDate,
                    InTime = attendance.InTime,
                    OutTime = attendance.OutTime
                };

                _bioContext.AttendanceTable.Add(result);
            }
            else
            {
                result.AttendanceId = attendance.AttendanceId;
                result.CandidateId = attendance.CandidateId;
                result.CandidateName = attendance.CandidateName;
                result.FingerPrintID = attendance.FingerPrintID;
                result.AttendanceDate = attendance.AttendanceDate;
                result.InTime = attendance.InTime;
                result.OutTime = attendance.OutTime;

                _bioContext.AttendanceTable.Update(result);
            }

            _bioContext.SaveChanges();
            return result;
        }
        public bool DeleteByattendanceId(int id)
        {
            var entity = _bioContext.CandidateEnroll.Find(id);
            if (entity == null) return false;

            _bioContext.CandidateEnroll.Remove(entity);
            _bioContext.SaveChanges();
            return true;
        }

        public List<FingerPrint> GetAllfingreprint()
        {
            throw new NotImplementedException();
               
            //thiva
        }
        #region EquipmentEnrollment
        public EquipmentEnrollment AddEquipmentEnrollment (EquipmentEnrollment equipment)
      {
            var result = _bioContext.EquipmentEnrollment.Where(x => x.EquipmentId == equipment.EquipmentId).FirstOrDefault();
            if (result == null)
            {
                result = new EquipmentEnrollment();
                result.EquipmentId = equipment.EquipmentId;
                result.EquipmentName = equipment.EquipmentName;
                result.EquipmentPurchaseDate = equipment.EquipmentPurchaseDate;
                result.EquipmentCount = equipment.EquipmentCount;
                result.EquipmentCondition = equipment.EquipmentCondition;
                result.CreatedDate = equipment.CreatedDate;
                _bioContext.EquipmentEnrollment.Add(result);
            }
            else
            {
                result.EquipmentId = equipment.EquipmentId;
                result.EquipmentName = equipment.EquipmentName;
                result.EquipmentPurchaseDate = equipment.EquipmentPurchaseDate;
                result.EquipmentCount = equipment.EquipmentCount;
                result.EquipmentCondition = equipment.EquipmentCondition;
                result.CreatedDate = equipment.CreatedDate;
                _bioContext.Update(result);
            }
            _bioContext.SaveChanges();
            return result;
      }
        public List<EquipmentEnrollmentModel> GetallEquipmentEnrollments()
        {
           

            var result = (from equi in _bioContext.EquipmentEnrollment
                          select new
                          {
                              equi.EquipmentId,
                              equi.EquipmentName,
                              equi.EquipmentPurchaseDate,
                              equi.EquipmentCount,
                              equi.EquipmentCondition,
                              equi.CreatedDate,

                          }).AsEnumerable().Select(x => new EquipmentEnrollmentModel
                          {
                              EquipmentId = x.EquipmentId,
                              EquipmentName = x.EquipmentName,
                              EquipmentPurchaseDate = x.EquipmentPurchaseDate,
                              EquipmentCount = Convert.ToInt32(x.EquipmentCount),
                              EquipmentCondition = x.EquipmentCondition,
                              CreatedDate = x.CreatedDate,
                          }).ToList();
            return result;
        }


        public EquipmentEnrollmentModel GetEquipmentEnrollmentbyid(int id )
        {

            var result = (from eq in _bioContext.EquipmentEnrollment
                          where eq.EquipmentId == id
                          select new EquipmentEnrollmentModel
                          {

                              EquipmentId = eq.EquipmentId,
                              EquipmentName = eq.EquipmentName,
                              EquipmentPurchaseDate = eq.EquipmentPurchaseDate,
                              EquipmentCondition=eq.EquipmentCondition,
                              EquipmentCount=Convert.ToInt32(eq.EquipmentCount),
                              CreatedDate = eq.CreatedDate,

                          }).FirstOrDefault();

            return result;

        }

        public EquipmentEnrollment deleteEquipmentbyid (int id)
        {
            var result = _bioContext.EquipmentEnrollment.Find(id);
            _bioContext.EquipmentEnrollment.Remove(result);
            _bioContext.SaveChanges(true);
            return result;

            
                      



        }
        #endregion


        #region HealthProgressTracking


        public HealthProgressTracking AddHealthProgressTracking(HealthProgressTracking healthProgressTracking)
        {
            if (healthProgressTracking.Height == null || healthProgressTracking.Height == 0)
                throw new ArgumentException("Height must be provided and greater than zero.");

            if (healthProgressTracking.CurrentWeight == null)
                throw new ArgumentException("Current weight is required.");

            var weight = healthProgressTracking.CurrentWeight;
            var height = healthProgressTracking.Height;

            var bmi = Math.Round(weight / (height * height), 3);

            var result = _bioContext.HealthProgressTracking
                .FirstOrDefault(x => x.CandidateId == healthProgressTracking.CandidateId);

            if (result == null)
            {
                result = new HealthProgressTracking
                {
                    CandidateId = healthProgressTracking.CandidateId,
                    Name = healthProgressTracking.Name,
                    InitialWeight = healthProgressTracking.InitialWeight,
                    Height = healthProgressTracking.Height,
                    CurrentWeight = healthProgressTracking.CurrentWeight,
                    InitialBMI = healthProgressTracking.InitialBMI,
                    CurrentBMI = bmi,
                    CurrentDate = healthProgressTracking.CurrentDate
                };

                _bioContext.HealthProgressTracking.Add(result);
            }
            else
            {
                result.Name = healthProgressTracking.Name;
                result.InitialWeight = healthProgressTracking.InitialWeight;
                result.CurrentWeight = healthProgressTracking.CurrentWeight;
                result.Height = healthProgressTracking.Height;
                result.InitialBMI = healthProgressTracking.InitialBMI;
                result.CurrentBMI = bmi;
                result.CurrentDate = healthProgressTracking.CurrentDate;

                _bioContext.HealthProgressTracking.Update(result);
            }

            _bioContext.SaveChanges();
            return result;
        }

        public List<HealthProgressTrackingModel> GetallHealthProgressTrackings()
        {
            

            var result = (from he in _bioContext.HealthProgressTracking
                          select new
                          {
                              he.CandidateId,
                              he.Name,
                              he.Height,
                              he.InitialWeight,
                              he.CurrentWeight,
                              he.InitialBMI,
                              he.CurrentBMI,
                              he.CurrentDate,



                          }).AsEnumerable().Select(x => new HealthProgressTrackingModel
                          {
                              CandidateId=x.CandidateId,
                              Name = x.Name,
                              Height = x.Height,
                              InitialWeight = x.InitialWeight,
                              CurrentWeight = x.CurrentWeight,
                              InitialBMI = x.InitialBMI,
                              CurrentBMI = x.CurrentBMI,
                              CurrentDate = x.CurrentDate,


                          }).ToList();

            return result;
            
        }

        public HealthProgressTrackingModel GetbyidHealthProgressTracking(int id)
        {
            var result = (from he in _bioContext.HealthProgressTracking
                          where he.CandidateId == id
                          select new HealthProgressTrackingModel
                          {
                              CandidateId = he.CandidateId,
                              Name = he.Name,
                              Height = he.Height,
                              InitialWeight = he.InitialWeight,
                              CurrentWeight = he.CurrentWeight,
                              InitialBMI = he.InitialBMI,
                              CurrentBMI = he.CurrentBMI,
                              CurrentDate = he.CurrentDate,

                          }).FirstOrDefault();

            return result;
        }

        public HealthProgressTracking deleteHealthprogresstrackingbyid  (int id)
        {
            var result = _bioContext.HealthProgressTracking.Find(id);
            _bioContext.HealthProgressTracking.Remove(result);
            _bioContext.SaveChanges(true);
            return result;

       
        }

        public List<Login> GetAll()
        {
            throw new NotImplementedException();
        }
        #endregion





        #endregion

        #region ImageUpload


        public async Task<byte[]> ConvertToBytesAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        public async Task<CandidateEnroll> AddOrUpdateCandidateAsync(CandidateEnroll candidate)
        {
            var result = _bioContext.CandidateEnroll
                .FirstOrDefault(c => c.CandidateId == candidate.CandidateId);

            // Convert uploaded image to byte array
            byte[] imageData = await ConvertToBytesAsync(candidate.PictureFile);

            if (result == null)
            {
                result = new CandidateEnroll
                {
                    Name = candidate.Name,
                    Gender = candidate.Gender,
                    Weight = candidate.Weight,
                    Height = candidate.Height,
                    Waist = candidate.Waist,
                    BMI = candidate.BMI,
                    BloodGroup = candidate.BloodGroup,
                    Age = candidate.Age,
                    CurrentAddress = candidate.CurrentAddress,
                    PermanentAddress = candidate.PermanentAddress,
                    AadharNumber = candidate.AadharNumber,
                    MobileNumber = candidate.MobileNumber,
                    EmailId = candidate.EmailId,
                    Profession = candidate.Profession,
                    Picture = imageData,
                    FingerPrintID = candidate.FingerPrintID,
                    IsActive = candidate.IsActive,
                    CreatedDate = candidate.CreatedDate
                };

                _bioContext.CandidateEnroll.Add(result);
            }
            else
            {
                result.Name = candidate.Name;
                result.Gender = candidate.Gender;
                result.Weight = candidate.Weight;
                result.Height = candidate.Height;
                result.Waist = candidate.Waist;
                result.BMI = candidate.BMI;
                result.BloodGroup = candidate.BloodGroup;
                result.Age = candidate.Age;
                result.CurrentAddress = candidate.CurrentAddress;
                result.PermanentAddress = candidate.PermanentAddress;
                result.AadharNumber = candidate.AadharNumber;
                result.MobileNumber = candidate.MobileNumber;
                result.EmailId = candidate.EmailId;
                result.Profession = candidate.Profession;
                result.FingerPrintID = candidate.FingerPrintID;
                result.IsActive = candidate.IsActive;
                result.CreatedDate = candidate.CreatedDate;

                if (imageData != null)
                {
                    result.Picture = imageData;
                }

                _bioContext.CandidateEnroll.Update(result);
            }

            await _bioContext.SaveChangesAsync();
            return result;
        }




        #endregion

        #region ServiceMaster
        public List<ServiceMaster> GetAllServiceMaster()
        {
            var result = _bioContext.ServiceMaster.ToList();
            return result;

        }

        public ServiceMaster GetServiceMasterbyID(int id)
        {
            var result = _bioContext.ServiceMaster.FirstOrDefault(x => x.ServiceId == id);
            return result;
        }

        public ServiceMaster AddOrUpdateServiceMaster(ServiceMaster service)
        {

            var result = _bioContext.ServiceMaster.Where(x => x.ServiceId == service.ServiceId).FirstOrDefault();
            if(result== null)
            {
                result = new ServiceMaster();
                result.ServiceId = service.ServiceId;
                result.ServiceName = service.ServiceName;
                result.PlanDuration = service.PlanDuration;
                result.PlanAmount = service.PlanAmount;
                result.CreatedDate = DateTime.UtcNow;
                _bioContext.ServiceMaster.Add(result);

            }
            else
            {
                result.ServiceName = service.ServiceName;
                result.PlanDuration = service.PlanDuration;
                result.PlanAmount = service.PlanAmount;
                result.UpdatedDate = DateTime.UtcNow;
                _bioContext.ServiceMaster.Update(result);
            }

                _bioContext.SaveChanges();
            return result;
        }
        public bool DeleteServiceMasterbyId(int id)
        {
            var entity = _bioContext.ServiceMaster.Find(id);
            if (entity == null) return false;

            _bioContext.ServiceMaster.Remove(entity);
            _bioContext.SaveChanges();
            return true;
        }

        public Payment Addpayment(Payment pymnnt)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
