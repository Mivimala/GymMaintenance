﻿using GymMaintenance.Model.Entity;
using GymMaintenance.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;


namespace GymMaintenance.DAL.Interface
{
    public interface IBioInterface
    {

        #region Login
        Payment Addpayment(Payment payment, string sessionId);
        public List<LoginModel> GetAllLogin();
        public LoginModel GetLoginById(int id);
        public Login AddTrainerlog(Login login);
        bool DeleteById(int id);
        Task SendLoginNotificationToAdmin(Login login);
        public Task<LoginModel> AuthenticateTrainerLoginAsync(string username, string password);
        #endregion

        #region FingerPrint
        public List<FingerPrintModel> GetAllfingerprint();
        public FingerPrintModel GetAllfingerprintbyID(int id);
        //public FingerPrint AddFingerPrint(FingerPrint fingerprint);
        Task<FingerPrintModel> AddFingerPrintAsync(FingerPrintModel dto);
      //  public Task<(bool success, string message)> VerifyFingerprintAsync(string base64Image, int? candidateId = null);
        Task<(bool success, string message)> VerifyFingerprintByImageAsync(string base64Image);
        Task<(bool success, string message)> VerifyAttendanceByCandidateIdAsync(int candidateId);
        public  Task<IActionResult> VerifyFingerprintAsync1(string? base64Image, int? candidateId);


        bool DeleteByfingerprintId(int id);
        //public IActionResult SaveFingerprint([FromBody] FingerPrintModel model);

        //public  Task<(bool, string)> VerifyFingerprintVim(string base64Image);
        #endregion

        #region Payment
        public List<PaymentModel> GetAllpayment();
        public PaymentModel GetpaymentbyId(int id, int serviceId);
        public (Payment? payment, string message) Addpayment(Payment pymnnt);
        public bool DeleteBypymntId(int id);
        public (Payment? payment, string message) AddpaymentMail(Payment pymnnt, string phone);
        #endregion

        #region TrainerEnrollment
        public List<TrainerEnrollmentModel> GetAlltrainer();
        public List<TrainerEnrollment> SearchTrainerEnrollByName(string keyword);
        public TrainerEnrollmentModel GetAlltrainerbyID(int id);
        public TrainerEnrollment AddOrUpdateTrainer(TrainerEnrollment trainer);
        public bool DeleteBytrainerId(int id);
        #endregion

        #region CandidateEnroll
        public List<CandidateEnrollModel> GetAllcandidate();
        public List<CandidateEnrollment> SearchCandidateEnrollByName(string keyword);
        public CandidateEnrollModel GetAllcandidatebyID(int id);
        public CandidateEnrollment AddOrUpdateCandidate(CandidateEnrollModel candidateModel);
        public bool DeleteBycandidateId(int id);
        #endregion

        #region AttendanceTable
        public List<AttendanceTableModel> GetAllAttendance();
        public AttendanceTableModel GetAllAttendancebyID(int id);
        public AttendanceTable AddOrUpdateAttendance(AttendanceTable attendance);
        public bool DeleteByattendanceId(int id);
        public AttendanceTable AddOrUpdateAttendanceNEW(AttendanceTableModel attendanceTableModel);
        #endregion

        public List<AlertModel> GetAlerts(AlertModel alertModel);
        public List<Login> GetAll();

        #region EquipmentEnrollment
        public EquipmentEnrollment AddEquipmentEnrollment(EquipmentEnrollment equipment);
        public List<EquipmentEnrollmentModel> GetallEquipmentEnrollments();

        public EquipmentEnrollmentModel GetEquipmentEnrollmentbyid(int id);
        public EquipmentEnrollment deleteEquipmentbyid(int id);

        #endregion


        #region HealthProgressTracking
        public HealthProgressTracking AddHealthProgressTracking(HealthProgressTracking healthProgressTracking);
        public List<HealthProgressTrackingModel> GetallHealthProgressTrackings();

        public HealthProgressTrackingModel GetbyidHealthProgressTracking(int id);
        public HealthProgressTracking deleteHealthprogresstrackingbyid(int id);
        #endregion

        #region ServiceMaster
        public List<ServiceMaster> GetAllServiceMaster();
        public ServiceMaster GetServiceMasterbyID(int id);
        public ServiceMaster AddOrUpdateServiceMaster(ServiceMaster service);
        public bool DeleteServiceMasterbyId(int id);
        #endregion

        #region Imageuploadbase64
        Task<bool> VerifyFingerprintAsync(string base64Image);
        Task<byte[]> ConvertBase64ToTemplateAsync(string base64Image);
        #endregion


        #region servicetable
        public List<Servicetable> GetallServicetable();
        public Servicetable GetbyidServicetable(int id);
        public Servicetable AddServicetable(Servicetable servicetable);
        public Servicetable DeletebyidServicetable(int id);

        #endregion

        #region packagetable
        public List<Packagetable> GetallPackagetable();
        public Packagetable GetbyidPackagetable(int id);
        public Packagetable AddPackagetable(Packagetable packagetable);
        public Packagetable DeletebyidPackagetable(int id);
        #endregion


        #region GetPaymentReportByDate

        List<PaymentModel> GetPaymentReportByDate(DateTime fromDate, DateTime toDate);
        #endregion
        #region GetCandidateReportByDate
        Task<List<CandidateEnrollModel>> GetCandidateReportByDate(DateTime fromDate, DateTime toDate);
        #endregion
        #region GetAttendanceReportByDate
        Task<List<AttendanceTableModel>> GetAttendanceReportByDate(DateTime fromDate, DateTime toDate);
        #endregion
        #region GetTrainerReportByDate
       
        List<TrainerEnrollmentModel> GetTrainerReportByDate(DateTime fromDate, DateTime toDate);

        #endregion


    }
}
