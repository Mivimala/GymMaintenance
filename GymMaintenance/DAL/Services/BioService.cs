using GymMaintenance.DAL.Interface;
using GymMaintenance.Model.Entity;
using GymMaintenance.Controllers;
using GymMaintenance.Model.ViewModel;
using System.IO;
using Microsoft.EntityFrameworkCore;
using GymMaintenance.Data;
using Microsoft.AspNetCore.Mvc;

namespace GymMaintenance.DAL.Services
{
    public class BioService : IBioInterface
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
        }

        public Login GetById(int id)
        {
            return _bioContext.Login.Find(id);
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

        public List<FingerPrint> GetAllfingreprint()
        {
            var result = _bioContext.FingerPrint.ToList();

            if (result.Any())
            {
                return result;
            }
            else

                return new List<FingerPrint>();
        }

        public FingerPrint GetByfingerprintId(int id)
        {
            return _bioContext.FingerPrint.Find(id);
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

        public List<Payment> GetAllpaymnent()
        {
            var result = _bioContext.Payment.ToList();

            if (result.Any())
            {
                return result;
            }
            else

                return new List<Payment>();
        }

        public Payment GetBypaymentid(int id)
        {
            return _bioContext.Payment.Find(id);
        }

        public Payment Addpayment(Payment pymnnt)
        {
            var result = _bioContext.Payment.Where(x => x.PaymentReceiptNo == pymnnt.PaymentReceiptNo).FirstOrDefault();
            if (result == null)
            {
                result = new Payment();
                result.MemmberId = pymnnt.MemmberId;
                result.Name = pymnnt.Name;
                result.MobileNumber = pymnnt.MobileNumber;
                result.Service = pymnnt.Service;
                result.Package = pymnnt.Package;
                result.TimeSlot = pymnnt.TimeSlot;
                result.PlanStartingDate = pymnnt.PlanStartingDate;
                result.PlanExpiringDate = pymnnt.PlanExpiringDate;
                result.PlanAmount = pymnnt.PlanAmount;
                result.BalanceAmount = pymnnt.BalanceAmount;
                result.CurrentPayment = pymnnt.CurrentPayment;
                result.ModeOfPayment = pymnnt.ModeOfPayment;
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
                result.Service = pymnnt.Service;
                result.Package = pymnnt.Package;
                result.TimeSlot = pymnnt.TimeSlot;
                result.PlanStartingDate = pymnnt.PlanStartingDate;
                result.PlanExpiringDate = pymnnt.PlanExpiringDate;
                result.PlanAmount = pymnnt.PlanAmount;
                result.BalanceAmount = pymnnt.BalanceAmount;
                result.CurrentPayment = pymnnt.CurrentPayment;
                result.ModeOfPayment = pymnnt.ModeOfPayment;
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

        public List<TrainerEnrollment> GetAlltrainer()
        {
            var result = _bioContext.TrainerEnrollment.ToList();

            if (result.Any())
            {
                return result;
            }
            else

                return new List<TrainerEnrollment>();
        }

        public TrainerEnrollment GetBytrainerid(int id)
        {
            return _bioContext.TrainerEnrollment.Find(id);
        }

        public TrainerEnrollment AddOrUpdateTrainer(TrainerEnrollment trainer)
        {
            var result = _bioContext.TrainerEnrollment
                .FirstOrDefault(x => x.TrainerId == trainer.TrainerId);

            if (result == null)
            { result = new TrainerEnrollment();

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
        public List<CandidateEnroll> GetAllcandidate()
        {
            var result = _bioContext.CandidateEnroll.ToList();

            if (result.Any())
            {
                return result;
            }
            else

                return new List<CandidateEnroll>();
        }
        public CandidateEnroll GetBycandidateid(int id)
        {
            return _bioContext.CandidateEnroll.Find(id);
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
        public List<AttendanceTable> GetAllattendance()
        {
            var result = _bioContext.AttendanceTable.ToList();

            if (result.Any())
            {
                return result;
            }
            else

                return new List<AttendanceTable>();
        }
        public AttendanceTable GetByattendanceid(int id)
        {
            return _bioContext.AttendanceTable.Find(id);
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


    }
}
