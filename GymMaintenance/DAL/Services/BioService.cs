using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO.Ports;
using System.Management;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
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
//using Neurotec.Biometrics;
//using Neurotec.Biometrics.Client;

//using Neurotec.IO;
using QuestPDF.Infrastructure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Data;
using System.IO.Ports;
using System.Management;
//using Neurotec.Images;
using SkiaSharp;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace GymMaintenance.DAL.Services
{
    public class BioService : IBioInterface
    {
        private readonly BioContext _bioContext;
        private readonly IMemoryCache _cache;
        private readonly SerialPort _serialPort;
        private readonly ILogger<BioService> _logger;
     

        public BioService(BioContext bioContext, IMemoryCache cache, ILogger<BioService> logger) 

        {
         
            _bioContext = bioContext;
          
              _cache = cache;
            _logger = logger;
            string[] availablePorts = SerialPort.GetPortNames();
            Console.WriteLine("Available Ports: " + string.Join(", ", availablePorts));

            string portToUse = GetCH340PortName();

            if (portToUse == null)
 {
     _logger.LogWarning("CH340 Arduino device not found. Proceeding without serial port.");
     return; // Exit constructor early without setting up the serial port
 }
            // Continue using the port
            _serialPort = new SerialPort
            {
                PortName = portToUse,
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                DtrEnable = true,
                RtsEnable = true,
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };

            // _serialPort.DataReceived += SerialPortDataReceived;
            _serialPort.DtrEnable = true;
            _serialPort.RtsEnable = true;

            _serialPort.DataReceived += (s, e) =>
            {
                try
                {
                    string response = _serialPort.ReadLine();
                    _logger.LogInformation("Arduino response: " + response);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error reading from Arduino: " + ex.Message);
                }
            };

            try
            {
                _logger.LogInformation("Available COM Ports: " + string.Join(", ", SerialPort.GetPortNames()));

                if (!_serialPort.IsOpen)
                    _serialPort.Open();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error opening serial port: " + ex.Message);
            }
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

            for (int i = 0; i < a.Length; i++)
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

        public async Task<(bool success, string message)> VerifyFingerprintAsync(string? base64Image, int? candidateId = null)
{

    if (string.IsNullOrWhiteSpace(base64Image) && candidateId == null)
    {
        return (false, "Please provide either a fingerprint image or a candidate ID.");
    }

    if (!string.IsNullOrWhiteSpace(base64Image))
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
                    return (false, "Candidate not found for matched fingerprint.");

                bool isActive = candidate.ToDate <= DateOnly.FromDateTime(DateTime.Today);

                if (!isActive)
                {
                    var existingAttendances = await _bioContext.AttendanceTable
                        .Where(a => a.CandidateId == candidate.CandidateId && a.IsActive)
                        .ToListAsync();

                    foreach (var att in existingAttendances)
                    {
                        att.IsActive = false;
                    }
                    if (existingAttendances.Count > 0)
                    {
                        _bioContext.AttendanceTable.UpdateRange(existingAttendances);
                        await _bioContext.SaveChangesAsync();
                    }
                }

                bool alreadyMarked = await _bioContext.AttendanceTable.AnyAsync(a =>
                    a.FingerPrintID == record.FingerPrintID &&
                    a.AttendanceDate == DateTime.Today);

                if (alreadyMarked)
                    return (true, "Attendance already marked today.");

                var attendance = new AttendanceTable
                {
                    FingerPrintID = record.FingerPrintID,
                    CandidateId = candidate.CandidateId,
                    CandidateName = candidate.Name,
                    AttendanceDate = DateTime.Today,
                    InTime = DateTime.Now.TimeOfDay,
                    IsActive = isActive
                };

                _bioContext.AttendanceTable.Add(attendance);
                await _bioContext.SaveChangesAsync();

                return (true, isActive
                    ? "Attendance marked successfully for active candidate."
                    : "Attendance marked as inactive (enrollment expires in the future).");
            }
        }
    }
    if (candidateId != null)
    {
        var candidate = await _bioContext.CandidateEnrollment
            .FirstOrDefaultAsync(c => c.CandidateId == candidateId.Value);

        if (candidate == null)
            return (false, "Candidate ID not found.");

        bool isActive = candidate.ToDate <= DateOnly.FromDateTime(DateTime.Today);

        if (!isActive)
        {
            var existingAttendances = await _bioContext.AttendanceTable
                .Where(a => a.CandidateId == candidate.CandidateId && a.IsActive)
                .ToListAsync();

            foreach (var att in existingAttendances)
            {
                att.IsActive = false;
            }
            if (existingAttendances.Count > 0)
            {
                _bioContext.AttendanceTable.UpdateRange(existingAttendances);
                await _bioContext.SaveChangesAsync();
            }
        }

        var alreadyMarked = await _bioContext.AttendanceTable.AnyAsync(a =>
            a.CandidateId == candidate.CandidateId &&
            a.AttendanceDate == DateTime.Today);

        if (alreadyMarked)
            return (true, "Attendance already marked today.");

        var manualAttendance = new AttendanceTable
        {
            FingerPrintID = 0,
            CandidateId = candidate.CandidateId,
            CandidateName = candidate.Name,
            AttendanceDate = DateTime.Today,
            InTime = DateTime.Now.TimeOfDay,
            IsActive = isActive
        };

        _bioContext.AttendanceTable.Add(manualAttendance);
        await _bioContext.SaveChangesAsync();

        return (true, isActive
            ? "Attendance marked manually for active candidate."
            : "Manual attendance marked as inactive (enrollment expires in the future).");
    }

    return (false, "Fingerprint did not match and Candidate ID was not provided.");
}

      

        public async Task<(bool success, string message)> VerifyFingerprintByImageAsync(string base64Image)
{
    if (string.IsNullOrWhiteSpace(base64Image))
        return (false, "Fingerprint image is required.");

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
                return (false, "Candidate not found for matched fingerprint.");
            // Get today's date
            var today = DateOnly.FromDateTime(DateTime.Today);
            // var today = DateOnly.ParseExact("11-07-2025", "dd-MM-yyyy", CultureInfo.InvariantCulture);
            // Check if the candidate's end date has already passed
            bool candidateIsInactive = candidate.ToDate > today;

            if (candidateIsInactive)
            {
                // Fetch all attendance records for the candidate that are still marked active
                var activeAttendances = await _bioContext.AttendanceTable
                    .Where(a => a.CandidateId == candidate.CandidateId && a.IsActive)
                    .ToListAsync();

                // Mark each one as inactive
                foreach (var attendance1 in activeAttendances)
                {
                    attendance1.IsActive = false;
                }

                // If any were updated, save changes to the database
                if (activeAttendances.Any())
                {
                    _bioContext.AttendanceTable.UpdateRange(activeAttendances);
                    await _bioContext.SaveChangesAsync();
                }
            }

            bool alreadyMarked = await _bioContext.AttendanceTable.AnyAsync(a =>
                a.FingerPrintID == record.FingerPrintID &&
                a.AttendanceDate == DateTime.Today);

            if (alreadyMarked)
                return (true, "Attendance already marked today.");

            var attendance = new AttendanceTable
            {
                FingerPrintID = record.FingerPrintID,
                CandidateId = candidate.CandidateId,
                CandidateName = candidate.Name,
                AttendanceDate = DateTime.Today,
                InTime = DateTime.Now.TimeOfDay,
                IsActive = candidateIsInactive
            };

            _bioContext.AttendanceTable.Add(attendance);
            await _bioContext.SaveChangesAsync();

            return (true, candidateIsInactive
                ? "Attendance marked successfully for active candidate."
                : "Attendance marked as inactive (enrollment expires in the future).");
        }
    }

    return (false, "Fingerprint did not match.");
}
        public async Task<(bool success, string message)> VerifyAttendanceByCandidateIdAsync(int candidateId)
                {
                    var candidate = await _bioContext.CandidateEnrollment
                        .FirstOrDefaultAsync(c => c.CandidateId == candidateId);

                    if (candidate == null)
                        return (false, "Candidate ID not found.");

                    bool isActive = candidate.ToDate >= DateOnly.FromDateTime(DateTime.Today);

                    if (!isActive)
                    {
                        var existingAttendances = await _bioContext.AttendanceTable
                            .Where(a => a.CandidateId == candidate.CandidateId && a.IsActive)
                            .ToListAsync();

                        foreach (var att in existingAttendances)
                        {
                            att.IsActive = false;
                        }

                        if (existingAttendances.Count > 0)
                        {
                            _bioContext.AttendanceTable.UpdateRange(existingAttendances);
                            await _bioContext.SaveChangesAsync();
                        }
                    }

                    var alreadyMarked = await _bioContext.AttendanceTable.AnyAsync(a =>
                        a.CandidateId == candidate.CandidateId &&
                        a.AttendanceDate == DateTime.Today);

                    if (alreadyMarked)
                        return (true, "Attendance already marked today.");

                    var manualAttendance = new AttendanceTable
                    {
                        FingerPrintID = null,
                        CandidateId = candidate.CandidateId,
                        CandidateName = candidate.Name,
                        AttendanceDate = DateTime.Today,
                        InTime = DateTime.Now.TimeOfDay,
                        IsActive = isActive
                    };

                    _bioContext.AttendanceTable.Add(manualAttendance);
                    await _bioContext.SaveChangesAsync();

                    return (true, isActive
                        ? "Attendance marked manually for active candidate."
                        : "Manual attendance marked as inactive (enrollment expires in the future).");
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
        #endregion
        #region Email
        public async Task<LoginModel> AuthenticateTrainerLoginAsync(string username, string password)
        {
            var user = _bioContext.Login
                .FirstOrDefault(x => x.UserName == username && x.Password == password && x.Role == "Trainer");

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
                    BalanceAmount = candidateModel.PackageAmount ?? 0,
                    FromDate = DateOnly.FromDateTime(fromDate),
                    ToDate = DateOnly.FromDateTime(toDate),
                    PaymentStatus = "Pending",
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
                result.PaymentStatus = result.BalanceAmount == 0 ? "Complete" : "Pending";
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
                result.CreatedDate = DateTime.Now;


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
            var result = (from x in _bioContext.Payment where x.ServiceId==serviceId 
                          select new PaymentModel
                          {

                              PaymentReceiptNo = x.PaymentReceiptNo,
                            
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
        public (Payment? payment, string message) AddpaymentMail(Payment pymnnt, string phone)
        {
            var candidate = _bioContext.CandidateEnrollment
                .FirstOrDefault(c => c.CandidateId == pymnnt.CandiadteId || c.MobileNumber == phone);

            if (candidate != null)
            {
                if (pymnnt.PaymentAmount <= candidate.BalanceAmount)
                {
                    var existingPayment = _bioContext.Payment
                        .FirstOrDefault(x => x.PaymentReceiptNo == pymnnt.PaymentReceiptNo);

                    string message;
                    Payment paymentResult;

                    if (existingPayment == null)
                    {
                        var newPayment = new Payment
                        {
                            PaymentReceiptNo = pymnnt.PaymentReceiptNo,
                            CandiadteId = pymnnt.CandiadteId,
                            Name = pymnnt.Name,
                            ServiceId = pymnnt.ServiceId,
                            PaymentAmount = pymnnt.PaymentAmount,
                            BalanceAmount = candidate.BalanceAmount - pymnnt.PaymentAmount,
                            Paymentmode = pymnnt.Paymentmode,
                            collectedby = pymnnt.collectedby,
                            IsActive = pymnnt.IsActive,
                            CreatedDate = DateTime.Now,
                        };

                        candidate.BalanceAmount -= pymnnt.PaymentAmount;
                        candidate.PaymentStatus = candidate.BalanceAmount == 0 ? "Complete" : "Pending";

                        _bioContext.Payment.Add(newPayment);
                        _bioContext.CandidateEnrollment.Update(candidate);
                        _bioContext.SaveChanges();

                        paymentResult = newPayment;
                        message = candidate.PaymentStatus == "Complete"
                            ? "Payment added. Balance complete."
                            : "Payment added. Balance pending.";
                    }
                    else
                    {
                        existingPayment.CandiadteId = pymnnt.CandiadteId;
                        existingPayment.Name = pymnnt.Name;
                        existingPayment.ServiceId = pymnnt.ServiceId;
                        existingPayment.PaymentAmount = pymnnt.PaymentAmount;
                        existingPayment.BalanceAmount = candidate.BalanceAmount - pymnnt.PaymentAmount;
                        existingPayment.Paymentmode = pymnnt.Paymentmode;
                        existingPayment.collectedby = pymnnt.collectedby;
                        existingPayment.IsActive = pymnnt.IsActive;
                        existingPayment.UpdatedDate = DateTime.Now;

                        candidate.BalanceAmount -= pymnnt.PaymentAmount;
                        candidate.PaymentStatus = candidate.BalanceAmount == 0 ? "Complete" : "Pending";

                        _bioContext.Payment.Update(existingPayment);
                        _bioContext.CandidateEnrollment.Update(candidate);
                        _bioContext.SaveChanges();

                        paymentResult = existingPayment;
                        message = candidate.PaymentStatus == "Complete"
                            ? "Payment updated. Balance complete."
                            : "Payment updated. Balance pending.";
                    }

                    // 📧 Send payment mail
                    SendPaymentNotificationToAdmin(paymentResult, candidate);

                    return (paymentResult, message);
                }

                return (null, "Payment Amount Not Correct.");
            }

            return (null, "Candidate not found");
        }

        public async void SendPaymentNotificationToAdmin(Payment payment, CandidateEnrollment candidate)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("GymManagement", "vimalajames2204@gmail.com"));
            message.To.Add(new MailboxAddress("Admin", "tarunsivakumar03@gmail.com"));
            message.Subject = "💳 Payment Notification - Trainer App";

            message.Body = new TextPart("html")
            {
                Text = $@"
        <html>
        <body style='font-family: Arial, sans-serif; color: #333;'>
            <h2 style='color: #2c3e50; margin-bottom: 0;'>Payment Notification</h2>
<p style='color: #555; font-size: 16px; margin-top: 4px;'>A new or updated payment has been recorded.</p>

            <p><strong>Candidate Name:</strong> {payment.Name}</p>
            <p><strong>Candidate ID:</strong> {payment.CandiadteId}</p>
            <p><strong>Mobile Number:</strong> {candidate.MobileNumber}</p>
            <p><strong>Payment Receipt No:</strong> {payment.PaymentReceiptNo}</p>
            <p><strong>Payment Amount:</strong> ₹{payment.PaymentAmount}</p>
            <p><strong>Remaining Balance:</strong> ₹{payment.BalanceAmount}</p>
            <p><strong>Payment Mode:</strong> {payment.Paymentmode}</p>
            <p><strong>Collected By:</strong> {payment.collectedby}</p>
            <p><strong>Payment Date:</strong> {(payment.CreatedDate != null ? payment.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))}</p>

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
        public List<FingerPrint> GetAllFingerprintsRaw()
        {
            return _bioContext.FingerPrint.ToList();
        }
        public AttendanceTable AddOrUpdateAttendanceNEW(string? fingerprint, int? candidateid)
        {
            FingerPrint? matchedFingerprint = null;
            CandidateEnrollment? candidate = null;
            if (!string.IsNullOrWhiteSpace(fingerprint))
            {
                try
                {
                    var inputBytes = Convert.FromBase64String(fingerprint);
                    var allFP = GetAllFingerprintsRaw();

                    matchedFingerprint = allFP.FirstOrDefault(fp =>
                        (fp.FingerPrint1 != null && fp.FingerPrint1.SequenceEqual(inputBytes)) ||
                        (fp.FingerPrint2 != null && fp.FingerPrint2.SequenceEqual(inputBytes)) ||
                        (fp.FingerPrint3 != null && fp.FingerPrint3.SequenceEqual(inputBytes)));

                    if (matchedFingerprint == null)
                        return null;

                    // Get candidate using FingerPrintID
                    candidate = _bioContext.CandidateEnrollment
                        .FirstOrDefault(c => c.FingerPrintID == matchedFingerprint.FingerPrintID);
                }
                catch
                {
                    return null; // Invalid base64 input
                }
            }
            // Case 2: Only CandidateId is provided
            else if (candidateid.HasValue && candidateid.Value != 0)
            {
                candidate = _bioContext.CandidateEnrollment
                    .FirstOrDefault(c => c.CandidateId == candidateid);

                if (candidate == null)
                    return null;

                // Get matching fingerprint using FingerPrintID from candidate record
                matchedFingerprint = _bioContext.FingerPrint
                    .FirstOrDefault(fp => fp.FingerPrintID == candidate.FingerPrintID);
            }

            if (matchedFingerprint == null || candidate == null)
                return null;

            // Check if attendance already exists for today
            var existingAttendance = _bioContext.AttendanceTable
                .FirstOrDefault(a =>
                    a.FingerPrintID == matchedFingerprint.FingerPrintID &&
                    a.AttendanceDate == DateTime.Today);

            if (existingAttendance == null)
            {
                var newAttendance = new AttendanceTable
                {
                    CandidateId = candidate.CandidateId,
                    CandidateName = candidate.Name,
                    FingerPrintID = matchedFingerprint.FingerPrintID,
                    AttendanceDate = DateTime.Today,
                    InTime = DateTime.Now.TimeOfDay
                };

                _bioContext.AttendanceTable.Add(newAttendance);
                _bioContext.SaveChanges();
                return newAttendance;
            }
            else
            {
                // Update in-time if needed
                existingAttendance.InTime = DateTime.Now.TimeOfDay;
                _bioContext.AttendanceTable.Update(existingAttendance);
                _bioContext.SaveChanges();
                return existingAttendance;
            }
        }

        public AttendanceTable AddOrUpdateAttendance(AttendanceTable attendance)
        {
            var result = _bioContext.AttendanceTable
                .FirstOrDefault(a => a.FingerPrintID == attendance.AttendanceId  || a.CandidateId == attendance.CandidateId);

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
        #endregion

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

        #region audrino

        // Add this at the top

        public string GetCH340PortName()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%)'"))
            {
                foreach (var device in searcher.Get())
                {
                    string name = device["Name"]?.ToString();
                    if (name != null && name.Contains("CH340"))
                    {
                        // Extract COM port from the name, e.g., "USB-SERIAL CH340 (COM5)"
                        int start = name.LastIndexOf("(COM");
                        if (start >= 0)
                        {
                            int end = name.IndexOf(")", start);
                            string port = name.Substring(start + 1, end - start - 1); // gets COM5
                            return port;
                        }
                    }
                }
            }

            return null; // not found
        }

        public List<AlertModel> GetAlerts(AlertModel alertModel)
        {
            try
            {
                SendBuzzCommand(alertModel.IsAlert);
                Disconnect();
                return new List<AlertModel> { alertModel };
                //Disconnect();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Arduino error: {ex.Message}");
                return new List<AlertModel>();
            }

        }



        public void SendBuzzCommand(bool isAlert)
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();

            string command = isAlert ? "BUZZ_ON" : "BUZZ_OFF";
            _serialPort.WriteLine(command);
        }


        public void Connect()
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public void Disconnect()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        public void SendCommand(string command)
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();

            _serialPort.WriteLine(command);
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string response = _serialPort.ReadLine();
            Console.WriteLine($"Received from Arduino: {response}");
            // You can store this response or trigger an event/callback
        }

        public AttendanceTable AddOrUpdateAttendanceNEW(AttendanceTableModel attendanceTableModel)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Reports
       
        public List<PaymentModel> GetPaymentReportByDate(DateTime fromDate, DateTime toDate)
        {
            var result = _bioContext.Payment
                .Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate)
                .Select(x => new PaymentModel
                {
                    PaymentReceiptNo = x.PaymentReceiptNo,

                    Name = x.Name,
                    ServiceId = x.ServiceId,
                    BalanceAmount = x.BalanceAmount,
                    PaymentAmount = x.PaymentAmount,
                    Paymentmode = x.Paymentmode,
                    collectedby = x.collectedby,
                    IsActive = x.IsActive,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate
                })
                .ToList();

            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return new List<PaymentModel>();
            }
        }
              
               
        public async Task<List<CandidateEnrollModel>> GetCandidateReportByDate(DateTime fromDate, DateTime toDate)
        {
            var result= await _bioContext.CandidateEnrollment
                .Where(c => c.CreatedDate.Date >= fromDate.Date && c.CreatedDate.Date <= toDate.Date)
                .Select(c => new CandidateEnrollModel
                {
                    CandidateId = c.CandidateId,
                    Name = c.Name,
                    Gender = c.Gender,
                    Address = c.Address,
                    MobileNumber = c.MobileNumber,
                    DOB = c.DOB,
                    ServiceId = c.ServiceId,
                    PackageId = c.PackageId,
                    PackageAmount = c.PackageAmount,
                    BalanceAmount = c.BalanceAmount,
                    FromDate = c.FromDate,
                    ToDate = c.ToDate,
                    PaymentStatus = c.PaymentStatus,
                    FingerPrintID = c.FingerPrintID,
                    IsActive = c.IsActive,
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync();
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return new List<CandidateEnrollModel>();
            }
        }
       
       
        public async Task<List<AttendanceTableModel>> GetAttendanceReportByDate(DateTime fromDate, DateTime toDate)
        {
            var result= await _bioContext.AttendanceTable
                .Where(a => a.AttendanceDate.Date >= fromDate.Date && a.AttendanceDate.Date <= toDate.Date)
                .Select(a => new AttendanceTableModel
                {
                    AttendanceId = a.AttendanceId,
                    CandidateId = a.CandidateId,
                    CandidateName = a.CandidateName,
                    AttendanceDate = a.AttendanceDate,
                    InTime = a.InTime
                })
                .ToListAsync();
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return new List<AttendanceTableModel>();
            }
        }
            
       
       
        public List<TrainerEnrollmentModel> GetTrainerReportByDate(DateTime fromDate, DateTime toDate)
        {
            var result = _bioContext.TrainerEnrollment
                .Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate)
                .Select(x => new TrainerEnrollmentModel
                {
                    TrainerId = x.TrainerId,
                    Name = x.Name,
                    Age = x.Age,
                    Address = x.Address,
                    MobileNumber = x.MobileNumber,
                    JoiningDate = x.JoiningDate,
                    FingerPrintID = x.FingerPrintID,
                    IsActive = x.IsActive,
                    CreatedDate = x.CreatedDate
                })
                .ToList();
            if (result != null && result.Count>0)
            {
                return result;
            }
            else
            {
                return new List<TrainerEnrollmentModel>();
            }
        }
             


        #endregion

        

       

    }
}

