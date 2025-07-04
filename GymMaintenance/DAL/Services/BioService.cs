using GymMaintenance.DAL.Interface;
using GymMaintenance.Data;
using GymMaintenance.Model.Entity;
using GymMaintenance.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


//using MFS100;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.ConstrainedExecution;

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

        #region ImageUploadbase64
        public async Task<byte[]> ConvertBase64ToTemplateAsync(string base64Image)
        {
            if (base64Image.Contains(","))
            {
                base64Image = base64Image.Substring(base64Image.IndexOf(",") + 1);
            }

            byte[] imageBytes = Convert.FromBase64String(base64Image);

            using var inputStream = new MemoryStream(imageBytes);
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(inputStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(500, 500),
                Mode = ResizeMode.Max
            }));

            using var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, new PngEncoder());

            return outputStream.ToArray();
        }

        public bool AreFingerprintsMatching(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            for (int i = 0;  i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }
        public async Task<bool> VerifyFingerprintAsync(string base64Image)
        {
            var inputTemplate = await ConvertBase64ToTemplateAsync(base64Image);

            var allFingerprints = await _bioContext.FingerPrint.ToListAsync();

            foreach (var record in allFingerprints)
            {
                if (AreFingerprintsMatching(inputTemplate, record.FingerPrint1) ||
                    AreFingerprintsMatching(inputTemplate, record.FingerPrint2) ||
                    AreFingerprintsMatching(inputTemplate, record.FingerPrint3))
                {
                    var candidate = await _bioContext.CandidateEnrollment
                        .FirstOrDefaultAsync(c => c.FingerPrintID == record.FingerPrintID);

                    if (candidate == null)
                        return false;

                    var alreadyMarked = await _bioContext.AttendanceTable.AnyAsync(a =>
                        a.FingerPrintID == record.FingerPrintID &&
                        a.AttendanceDate == DateTime.Today);

                    if (!alreadyMarked)
                    {
                        try
                        {
                            var attendance = new AttendanceTable
                            {
                                FingerPrintID = record.FingerPrintID,
                                CandidateId = candidate.CandidateId,
                                CandidateName = candidate.Name,
                                AttendanceDate = DateTime.Today,
                                InTime = DateTime.Now.TimeOfDay
                            };

                            _bioContext.AttendanceTable.Add(attendance);
                            await _bioContext.SaveChangesAsync();
                        }
                        catch (DbUpdateException ex)
                        {
                            var inner = ex.InnerException?.Message ?? ex.Message;
                            Console.WriteLine("Error saving attendance: " + inner);
                            throw;
                        }
                    }

                    return true;
                }
            }

            return false; 
        }
        #endregion



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

        public Login AddTrainerlog(Login login)
        {
            var result = _bioContext.Login.Where(x => x.LoginId == login.LoginId).FirstOrDefault();
            if (result == null)
            {
                result = new Login();
                result.Role = "Trainer";
                result.UserName = login.UserName;
                result.Password = login.Password;
                _bioContext.Login.Add(result);
            }
            else
            {
                result.LoginId = login.LoginId;
                result.Role = "Trainer";
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
        #region Email
        public async Task<LoginModel> AuthenticateTrainerLoginAsync(string username, string password)
        {
            var user = _bioContext.Login
                .FirstOrDefault(x => x.UserName == username && x.Password == password);// && x.Role == "Trainer");

            if (user != null)
            {
                // ✅ Send email notification
                await SendLoginNotificationToAdmin(user);

                return new LoginModel
                {
                    LoginId = user.LoginId,
                    Role = user.Role,
                    UserName = user.UserName,
                    Password = user.Password
                };
            }

            return null;
        }
        public async Task SendLoginNotificationToAdmin(Login login)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("GymManagement", "vimalajames2204@gmail.com"));
            message.To.Add(new MailboxAddress("Admin", "tarunsivakumar03@gmail.com"));
            message.Subject = "Trainer Login Notification";

            //message.Body = new TextPart("plain")
            //{
            //    Text = $"Trainer '{login.UserName}' (ID: {login.LoginId}) has logged in at {DateTime.UtcNow} UTC."
            //};
             


            message.Body = new TextPart("html")
            {
                Text = $@"
        

                    <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                    <h2 style='color: #2c3e50;'>Trainer Login Alert 🚨</h2>
                    <p><strong>Trainer Name:</strong> {login.UserName}</p>
                    <p><strong>Login ID:</strong> {login.LoginId}</p>
                    <p><strong>Login Time:</strong> {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UTC</p>
                    <hr style='border: 0; height: 1px; background: #ccc;' />
                    <p style='font-size: 12px; color: #999;'>This is an automated message from Trainer App.</p>
                    </body>
                    </html>"
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("vimalajames2204@gmail.com", "rxqw mgbr ozkg wjhp");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }


        #endregion




        #endregion
        #region  FingerPrint

        public List<FingerPrintModel> GetAllfingerprint()
        {
            return _bioContext.FingerPrint
                .AsEnumerable()
                .Select(x => new FingerPrintModel
                {
                    FingerPrintID = x.FingerPrintID,
                    Role = x.Role,
                    FingerPrint1 = x.FingerPrint1 != null ? $"data:image/png;base64,{Convert.ToBase64String(x.FingerPrint1)}" : null,
                    FingerPrint2 = x.FingerPrint2 != null ? $"data:image/png;base64,{Convert.ToBase64String(x.FingerPrint2)}" : null,
                    FingerPrint3 = x.FingerPrint3 != null ? $"data:image/png;base64,{Convert.ToBase64String(x.FingerPrint3)}" : null,
                    CreatedDate = x.CreatedDate
                }).ToList();
        }

        public FingerPrintModel GetAllfingerprintbyID(int id)
        {    

            var x = _bioContext.FingerPrint.FirstOrDefault(a => a.FingerPrintID == id);

            if (x == null) return null;

            return new FingerPrintModel
            {
                FingerPrintID = x.FingerPrintID,
                Role = x.Role,
                FingerPrint1 = x.FingerPrint1 != null ? $"data:image/png;base64,{Convert.ToBase64String(x.FingerPrint1)}" : null,
                FingerPrint2 = x.FingerPrint2 != null ? $"data:image/png;base64,{Convert.ToBase64String(x.FingerPrint2)}" : null,
                FingerPrint3 = x.FingerPrint3 != null ? $"data:image/png;base64,{Convert.ToBase64String(x.FingerPrint3)}" : null,
                CreatedDate = x.CreatedDate
            };
        }

        public async Task<FingerPrintModel> AddFingerPrintAsync(FingerPrintModel dto)
        {
            var result = _bioContext.FingerPrint.FirstOrDefault(x => x.FingerPrintID == dto.FingerPrintID);

            byte[] fp1 = await ConvertBase64ToTemplateAsync(dto.FingerPrint1);
            byte[] fp2 = await ConvertBase64ToTemplateAsync(dto.FingerPrint2);
            byte[] fp3 = await ConvertBase64ToTemplateAsync(dto.FingerPrint3);

            if (result == null)
            {
                result = new FingerPrint
                {
                    Role = dto.Role,
                    FingerPrint1 = fp1,
                    FingerPrint2 = fp2,
                    FingerPrint3 = fp3,
                    CreatedDate = dto.CreatedDate
                };
                _bioContext.FingerPrint.Add(result);
            }
            else
            {
                result.Role = dto.Role;
                result.FingerPrint1 = fp1;
                result.FingerPrint2 = fp2;
                result.FingerPrint3 = fp3;
                result.CreatedDate = dto.CreatedDate;


                _bioContext.FingerPrint.Update(result);
            }

            await _bioContext.SaveChangesAsync();
            return new FingerPrintModel
            {
                FingerPrintID = result.FingerPrintID,
                Role = result.Role,
                FingerPrint1 = Convert.ToBase64String(result.FingerPrint1),
                FingerPrint2 = Convert.ToBase64String(result.FingerPrint2),
                FingerPrint3 = Convert.ToBase64String(result.FingerPrint3),
                CreatedDate = result.CreatedDate
            };

            
        }

        public bool DeleteByfingerprintId(int id)
        {
            var entity = _bioContext.FingerPrint.Find(id);
            if (entity == null) return false;

            _bioContext.FingerPrint.Remove(entity);
            _bioContext.SaveChanges();
            return true;
        }


        //public IActionResult SaveFingerprint([FromBody] FingerPrintModel model)
        //{
        //    if (string.IsNullOrEmpty(model.FingerPrint1))
        //        return new BadRequestObjectResult("The fingerprint field is required.");

        //    try
        //    {
        //        // Try to decode the Base64 to validate it's correct
        //        byte[] fingerprintData = Convert.FromBase64String(model.FingerPrint1);

        //        var entity = new FingerPrint
        //        {
        //            FingerPrint1 = model.FingerPrint1,   // Store the Base64 string directly
        //            CreatedDate = DateTime.Now,
        //            Role = model.Role
        //        };

        //        _bioContext.FingerPrint.Add(entity);
        //        _bioContext.SaveChanges();

        //        return new OkObjectResult("Fingerprint saved successfully.");
        //    }
        //    catch (FormatException)
        //    {
        //        return new BadRequestObjectResult("Invalid Base64 format in fingerprint.");
        //    }
        //}


        //string fingerprintTemplate = request.FingerPrint1;

        //// You should now compare this fingerprint with registered templates
        //// For example, match with stored fingerprint templates in database

        //bool isMatch = MatchFingerprint(fingerprintTemplate);
        //if (isMatch)
        //{
        //    // Save attendance entry
        //    return Ok(new { success = true, message = "Attendance marked successfully." });
        //}
        //else
        //{
        //    return Unauthorized(new { success = false, message = "Fingerprint not recognized." });
        //}


        //private IActionResult Unauthorized(object value)
        //{
        //    throw new NotImplementedException();
        //}

        //private IActionResult Ok(object value)
        //{
        //    throw new NotImplementedException();
        //}

        //private bool MatchFingerprint(string scannedTemplate)
        //{
        //    // Load enrolled templates from DB and match
        //    // You would use SecuGen SDK for template matching
        //    return true; // simulate a match
        //}




        //private IActionResult BadRequest(string v)
        //{
        //    throw new NotImplementedException();
        //}

        //private IActionResult Ok(string v)
        //{
        //    throw new NotImplementedException();
        //}




        #endregion

        #region Candidate
        public List<CandidateEnrollModel> GetAllcandidate()
        {
            var result = (from a in _bioContext.CandidateEnrollment
                          select new
                          {
                              a.CandidateId,
                              a.Name,
                              a.Gender,
                              a.Address,
                              a.MobileNumber,
                              a.DOB,
                              a.ServiceId,
                              a.PackageId,
                              a.PackageAmount,
                              a.BalanceAmount,
                              a.FromDate,
                              a.ToDate,
                              a.PaymentStatus,
                              a.FingerPrintID,
                              a.IsActive,
                              a.CreatedDate,
                          }).AsEnumerable().Select(x => new CandidateEnrollModel
                          {
                              CandidateId = x.CandidateId,
                              Name = x.Name,
                              Gender = x.Gender,
                              Address = x.Address,
                              MobileNumber = x.MobileNumber,
                              DOB = x.DOB,
                              ServiceId = x.ServiceId,
                              PackageId = x.PackageId,
                              PackageAmount = x.PackageAmount,
                              BalanceAmount = x.BalanceAmount,
                              FromDate = x.FromDate,
                              ToDate = x.ToDate,
                              PaymentStatus = x.PaymentStatus,
                              FingerPrintID = x.FingerPrintID,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate,
                          }).ToList();
            return result;
        }

        public List<CandidateEnrollment> SearchCandidateEnrollByName(string keyword)
        {
            return _bioContext.CandidateEnrollment.Where(e => !string.IsNullOrEmpty(e.Name) &&
                                          e.Name.ToLower().Contains(keyword.ToLower()))
                              .ToList();
        }

        public CandidateEnrollModel GetAllcandidatebyID(int id)
        {
            var result = (from x in _bioContext.CandidateEnrollment
                          where x.CandidateId == id
                          select new CandidateEnrollModel
                          {
                              CandidateId = x.CandidateId,
                              Name = x.Name,
                              Gender = x.Gender,
                              Address = x.Address,
                              MobileNumber = x.MobileNumber,
                              DOB = x.DOB,
                              ServiceId = x.ServiceId,
                              PackageId = x.PackageId,
                              PackageAmount = x.PackageAmount,
                              BalanceAmount = x.BalanceAmount,
                              FromDate = x.FromDate,
                              ToDate = x.ToDate,
                              PaymentStatus = x.PaymentStatus,
                              FingerPrintID = x.FingerPrintID,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate,
                          }).FirstOrDefault();
            return result;
        }



        private DateTime CalculateToDate(DateTime fromDate, int packageMonths)
        {
            return fromDate.AddMonths(packageMonths).AddDays(-1);
        }

        public CandidateEnrollment AddOrUpdateCandidate(CandidateEnrollModel candidateModel)
        {
            if (candidateModel.PackageMonths == null || candidateModel.PackageMonths <= 0)
                throw new ArgumentException("PackageMonths must be provided and greater than zero.");

            var result = _bioContext.CandidateEnrollment
                .FirstOrDefault(c => c.CandidateId == candidateModel.CandidateId);

            DateTime fromDate = DateTime.Now;
            DateTime toDate = CalculateToDate(fromDate, candidateModel.PackageMonths.Value);

            if (result == null)
            {
                result = new CandidateEnrollment
                {
                    Name = candidateModel.Name,
                    Gender = candidateModel.Gender,
                    Address = candidateModel.Address,
                    MobileNumber = candidateModel.MobileNumber,
                    DOB = candidateModel.DOB ?? default,
                    ServiceId = candidateModel.ServiceId ?? 0,
                    PackageId = candidateModel.PackageId ?? 0,
                    PackageAmount = candidateModel.PackageAmount ?? 0,
                    BalanceAmount = candidateModel.BalanceAmount ?? 0,
                    FromDate = DateOnly.FromDateTime(fromDate),
                    ToDate = DateOnly.FromDateTime(toDate),
                    PaymentStatus = candidateModel.PaymentStatus,
                    FingerPrintID = candidateModel.FingerPrintID ?? 0,
                    IsActive = candidateModel.IsActive ?? true,
                    CreatedDate = candidateModel.CreatedDate ?? DateTime.Now
                };

                _bioContext.CandidateEnrollment.Add(result);
            }
            else
            {
                result.Name = candidateModel.Name;
                result.Gender = candidateModel.Gender;
                result.Address = candidateModel.Address;
                result.MobileNumber = candidateModel.MobileNumber;
                result.DOB = candidateModel.DOB ?? result.DOB;
                result.ServiceId = candidateModel.ServiceId ?? result.ServiceId;
                result.PackageId = candidateModel.PackageId ?? result.PackageId;
                result.PackageAmount = candidateModel.PackageAmount ?? result.PackageAmount;
                result.BalanceAmount = candidateModel.BalanceAmount ?? result.BalanceAmount;
                result.FromDate = DateOnly.FromDateTime(fromDate);
                result.ToDate = DateOnly.FromDateTime(toDate);
                result.PaymentStatus = candidateModel.PaymentStatus;
                result.FingerPrintID = candidateModel.FingerPrintID ?? result.FingerPrintID;
                result.IsActive = candidateModel.IsActive ?? result.IsActive;
                result.CreatedDate = candidateModel.CreatedDate ?? result.CreatedDate;

                _bioContext.CandidateEnrollment.Update(result);
            }

            _bioContext.SaveChanges();
            return result;
        }


        public bool DeleteBycandidateId(int id)
        {
            var entity = _bioContext.CandidateEnrollment.Find(id);
            if (entity == null) return false;

            _bioContext.CandidateEnrollment.Remove(entity);
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
                              a.Age,
                              a.Address,
                              a.MobileNumber,
                              a.JoiningDate,
                              a.FingerPrintID,
                              a.IsActive,
                              a.CreatedDate
                          }).AsEnumerable().Select(x => new TrainerEnrollmentModel
                          {
                              TrainerId = x.TrainerId,
                              Password = x.Password,
                              Name = x.Name,
                              Age = x.Age,
                              Address = x.Address,
                              MobileNumber = x.MobileNumber,
                              JoiningDate = x.JoiningDate,
                              FingerPrintID = x.FingerPrintID,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate
                          }).ToList();
            return result;
        }

        public List<TrainerEnrollment> SearchTrainerEnrollByName(string keyword)
        {
            return _bioContext.TrainerEnrollment
                              .Where(e => !string.IsNullOrEmpty(e.Name) &&
                                          e.Name.ToLower().Contains(keyword.ToLower()))
                              .ToList();
        }
        public TrainerEnrollmentModel GetAlltrainerbyID(int id)
        {
            var result = (from x in _bioContext.TrainerEnrollment where x.TrainerId == id 
                          select new  TrainerEnrollmentModel
                          {

                              TrainerId = x.TrainerId,
                              Password = x.Password,
                              Name = x.Name,
                              Age = x.Age,
                              Address = x.Address,
                              MobileNumber = x.MobileNumber,
                              JoiningDate = x.JoiningDate,
                              FingerPrintID = x.FingerPrintID,
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
                result.Age = trainer.Age;
                result.Address = trainer.Address;
                result.Age = trainer.Age;
                result.MobileNumber = trainer.MobileNumber;
                result.JoiningDate = trainer.JoiningDate;
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
                result.Age = trainer.Age;
                result.Address = trainer.Address;
                result.Age = trainer.Age;
                result.MobileNumber = trainer.MobileNumber;
                result.JoiningDate = trainer.JoiningDate;
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
                              a.ServiceId,
                              a.BalanceAmount,
                              a.PaymentAmount,
                              a.Paymentmode,
                              a.collectedby,
                              a.IsActive,
                              a.CreatedDate,
                              a.UpdatedDate
                             
                          }).AsEnumerable().Select(x => new PaymentModel
                          {
                              PaymentReceiptNo = x.PaymentReceiptNo,
                              MemmberId = x.MemmberId,
                              Name = x.Name,
                              ServiceId = x.ServiceId,
                              BalanceAmount = x.BalanceAmount,
                              PaymentAmount = x.PaymentAmount,
                              Paymentmode = x.Paymentmode,
                              collectedby = x.collectedby,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate,
                              UpdatedDate = x.UpdatedDate
                             
                          }).ToList();
            return result;
        }
        
        public PaymentModel GetpaymentbyId( int id,int serviceId)
        {
            var result = (from x in _bioContext.Payment where x.MemmberId == id && x.ServiceId==serviceId 
                          select new PaymentModel
                          {

                              PaymentReceiptNo = x.PaymentReceiptNo,
                              MemmberId = x.MemmberId,
                              Name = x.Name,
                              ServiceId = x.ServiceId,
                              BalanceAmount = x.BalanceAmount,
                              PaymentAmount = x.PaymentAmount,
                              Paymentmode = x.Paymentmode,
                              collectedby = x.collectedby,
                              IsActive = x.IsActive,
                              CreatedDate = x.CreatedDate,
                              UpdatedDate = x.UpdatedDate

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
                result.ServiceId = pymnnt.ServiceId;
                result.BalanceAmount = pymnnt.BalanceAmount;
                result.PaymentAmount = pymnnt.PaymentAmount;
                result.Paymentmode = pymnnt.Paymentmode;
                result.collectedby = pymnnt.collectedby;
                result.IsActive = pymnnt.IsActive;
                result.CreatedDate = pymnnt.CreatedDate;
                result.UpdatedDate = pymnnt.UpdatedDate;

                _bioContext.Payment.Add(result);
            }

            else
            {
                result.PaymentReceiptNo = pymnnt.PaymentReceiptNo;
                result.MemmberId = pymnnt.MemmberId;
                result.Name = pymnnt.Name;
                result.ServiceId = pymnnt.ServiceId;
                result.BalanceAmount = pymnnt.BalanceAmount;
                result.PaymentAmount = pymnnt.PaymentAmount;
                result.Paymentmode = pymnnt.Paymentmode;
                result.collectedby = pymnnt.collectedby;
                result.IsActive = pymnnt.IsActive;
                result.CreatedDate = pymnnt.CreatedDate;
                result.UpdatedDate = pymnnt.UpdatedDate;
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
                              a.InTime
                              


                          }).AsEnumerable().Select(x => new AttendanceTableModel
                          {
                              AttendanceId = x.AttendanceId,
                              CandidateId = x.CandidateId,
                              CandidateName = x.CandidateName,
                              FingerPrintID = x.FingerPrintID,
                              AttendanceDate = x.AttendanceDate,
                              InTime = x.InTime
                             

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
                              InTime = x.InTime

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
                    InTime = attendance.InTime
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

                _bioContext.AttendanceTable.Update(result);
            }

            _bioContext.SaveChanges();
            return result;
        }
        public bool DeleteByattendanceId(int id)
        {
            var entity = _bioContext.CandidateEnrollment.Find(id);
            if (entity == null) return false;

            _bioContext.CandidateEnrollment.Remove(entity);
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

        //#region ImageUpload


        ////public async Task<byte[]> ConvertToBytesAsync(IFormFile file)
        ////{
        ////    if (file == null || file.Length == 0)
        ////        return null;

        ////    using var memoryStream = new MemoryStream();
        ////    await file.CopyToAsync(memoryStream);
        ////    return memoryStream.ToArray();
        ////}
        //public async Task<CandidateEnroll> AddOrUpdateCandidateAsync(CandidateEnroll candidate)
        //{
        //    var result = _bioContext.CandidateEnroll
        //        .FirstOrDefault(c => c.CandidateId == candidate.CandidateId);

        //    // Convert uploaded image to byte array
        //    //byte[] imageData = await ConvertToBytesAsync(candidate.PictureFile);

        //    if (result == null)
        //    {
        //        result = new CandidateEnroll
        //        {
        //            Name = candidate.Name,
        //            Gender = candidate.Gender,
        //            Weight = candidate.Weight,
        //            Height = candidate.Height,
        //            Waist = candidate.Waist,
        //            BMI = candidate.BMI,
        //            BloodGroup = candidate.BloodGroup,
        //            Age = candidate.Age,
        //            CurrentAddress = candidate.CurrentAddress,
        //            PermanentAddress = candidate.PermanentAddress,
        //            AadharNumber = candidate.AadharNumber,
        //            MobileNumber = candidate.MobileNumber,
        //            EmailId = candidate.EmailId,
        //            Profession = candidate.Profession,
        //            Picture = imageData,
        //            FingerPrintID = candidate.FingerPrintID,
        //            IsActive = candidate.IsActive,
        //            CreatedDate = candidate.CreatedDate
        //        };

        //        _bioContext.CandidateEnroll.Add(result);
        //    }
        //    else
        //    {
        //        result.Name = candidate.Name;
        //        result.Gender = candidate.Gender;
        //        result.Weight = candidate.Weight;
        //        result.Height = candidate.Height;
        //        result.Waist = candidate.Waist;
        //        result.BMI = candidate.BMI;
        //        result.BloodGroup = candidate.BloodGroup;
        //        result.Age = candidate.Age;
        //        result.CurrentAddress = candidate.CurrentAddress;
        //        result.   PermanentAddress = candidate.PermanentAddress;
        //        result.AadharNumber = candidate.AadharNumber;
        //        result.MobileNumber = candidate.MobileNumber;
        //        result.EmailId = candidate.EmailId;
        //        result.Profession = candidate.Profession;
        //        result.FingerPrintID = candidate.FingerPrintID;
        //        result.IsActive = candidate.IsActive;
        //        result.CreatedDate = candidate.CreatedDate;

        //        //if (imageData != null)
        //        //{
        //        //    result.Picture = imageData;
        //        //}

        //        _bioContext.CandidateEnroll.Update(result);
        //    }

        //    await _bioContext.SaveChangesAsync();
        //    return result;
        //}




        //#endregion

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

        #region servicetable

        public List<Servicetable> GetallServicetable()
        {
            var result = (from ser in _bioContext.Servicetable
                          select new
                          {
                              ser.ServiceId,
                              ser.ServiceName,
                              ser.CreateAt,
                          }).AsEnumerable().Select(x => new Servicetable
                          {

                              ServiceId = x.ServiceId,
                              ServiceName = x.ServiceName,
                              CreateAt = x.CreateAt,


                          }).ToList();

            return result;

        }

        public Servicetable GetbyidServicetable(int id)
        {
            var result = (from ser in _bioContext.Servicetable
                          where ser.ServiceId == id
                          select new Servicetable
                          {
                              ServiceId = ser.ServiceId,
                              ServiceName = ser.ServiceName,
                              CreateAt = ser.CreateAt,
                          }).FirstOrDefault();
            if (result == null)
            {
                return new Servicetable();
            }
            else
            {
                return result;
            }

        }

        public Servicetable DeletebyidServicetable(int id)
        {
            var result = _bioContext.Servicetable.Find(id);
            _bioContext.Servicetable.Remove(result);
            _bioContext.SaveChanges(true);
            return result;
        }

        public Servicetable AddServicetable(Servicetable servicetable)
        {
            var result = _bioContext.Servicetable.Where(x => x.ServiceId == servicetable.ServiceId).FirstOrDefault();
            if (result == null)
            {
                result = new Servicetable();
                {

                    result.ServiceName = servicetable.ServiceName;
                    result.CreateAt = servicetable.CreateAt;
                    _bioContext.Add(result);


                }

            }
            else
            {
                result.ServiceId = servicetable.ServiceId;
                result.ServiceName = servicetable.ServiceName;
                result.CreateAt = servicetable.CreateAt;
                _bioContext.Update(result);
            }

            _bioContext.SaveChanges();
            return result;
        }
        #endregion

        #region packagetable

        public List<Packagetable> GetallPackagetable()
        {
            var result = (from pac in _bioContext.Packagetable
                          select new
                          {
                              pac.PackageId,
                              pac.PackageName,
                              pac.PackageAmount,
                              pac.CreatedAt,
                          }).AsEnumerable().Select(x => new Packagetable
                          {
                              PackageId=x.PackageId,
                              PackageName = x.PackageName,
                              PackageAmount = x.PackageAmount,
                              CreatedAt = x.CreatedAt,
                          }).ToList();
            return result;

        }
        public Packagetable GetbyidPackagetable(int id)
        {

            var result = (from pac in _bioContext.Packagetable
                          where pac.PackageId == id
                          select new Packagetable

                          {

                              PackageId = pac.PackageId,
                              PackageName = pac.PackageName,
                              PackageAmount = pac.PackageAmount,
                              CreatedAt = pac.CreatedAt,
                          }).FirstOrDefault();
            if (result == null)
            {
                return new Packagetable();
            }
            else
            {
                return result;
            }


        }

        public Packagetable AddPackagetable(Packagetable packagetable)
        {
            var result = _bioContext.Packagetable.Where(x => x.PackageId == packagetable.PackageId).FirstOrDefault();

            if (result == null)
            {
                result = new Packagetable();
                {
                    result.PackageName = packagetable.PackageName;
                    result.PackageAmount = packagetable.PackageAmount;
                    result.CreatedAt = packagetable.CreatedAt;
                    _bioContext.Add(result);
                }

            }
            else
            {
                result.PackageId = packagetable.PackageId;
                result.PackageName = packagetable.PackageName;
                result.PackageAmount = packagetable.PackageAmount;
                result.CreatedAt = packagetable.CreatedAt;
                _bioContext.Update(result);
            }

            _bioContext.SaveChanges();
            return result;
        }
        public Packagetable DeletebyidPackagetable(int id)
        {
            var result = _bioContext.Packagetable.Find(id);
            _bioContext.Packagetable.Remove(result);
            _bioContext.SaveChanges(true);
            return result;
        }

        #endregion
    }
}
