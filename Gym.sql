create database Bio;
use Bio;
create table Login (
LoginId int auto_increment primary key,
Role varchar(25),
UserName varchar(25),
Password varchar(25));

insert into Login values (1,'admin','admin','admin');             

CREATE TABLE fingerPrint (
fingerPrintID INT AUTO_INCREMENT PRIMARY KEY,
Role NVARCHAR(255),
fingerPrint1 LONGBLOB,
fingerPrint2 LONGBLOB,
fingerPrint3 LONGBLOB,
CreatedDate DATE);








create table Servicetable(
ServiceId  int  AUTO_INCREMENT PRIMARY KEY,
ServiceName nvarchar(255),
CreateAt date) ;

INSERT INTO Servicetable (ServiceName, CreateAt)
VALUES 
('Personal', '2025-07-04'),
('General', '2025-07-04');

create table Packagetable (
PackageId int  AUTO_INCREMENT PRIMARY KEY,
PackageName nvarchar(255),
PackageAmount decimal(18,2),
CreatedAt date );

INSERT INTO Packagetable ( PackageName, PackageAmount, CreatedAt)
VALUES 
('Monthly Plan',      1500.00, '2025-07-01'),
('Quarterly Plan',   3000.00, '2025-07-02'),
('Halfyearly Plan',   4000.00, '2025-07-03'),
('Yearly Plan',   5000.00, '2025-07-04');

CREATE TABLE CandidateEnrollment (
    CandidateId INT AUTO_INCREMENT PRIMARY KEY,
    Name NVARCHAR(255),
    Gender NVARCHAR(50),
    Address NVARCHAR(500),
    MobileNumber VARCHAR(15),
    DOB DATE,
    ServiceId INT,
    PackageId INT,
    PackageAmount DECIMAL(18,2),
    BalanceAmount DECIMAL(18,2),
    FromDate DATE,
    ToDate DATE,
    PaymentStatus VARCHAR(50),
    FingerPrintID INT,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME,
    CONSTRAINT FK_Candidate_Service FOREIGN KEY (ServiceId) REFERENCES Servicetable(ServiceId),
    CONSTRAINT FK_Candidate_Package FOREIGN KEY (PackageId) REFERENCES Packagetable(PackageId),
    CONSTRAINT FK_Candidate_FingerPrint FOREIGN KEY (FingerPrintID) REFERENCES fingerPrint(fingerPrintID)
);


CREATE TABLE TrainerEnrollment (
    TrainerId INT AUTO_INCREMENT PRIMARY KEY,
    Password VARCHAR(255),
    Name NVARCHAR(255),
    Age INT,
    Address NVARCHAR(255),
    MobileNumber VARCHAR(15),
    JoiningDate DATE,
    FingerPrintID INT,
    IsActive BOOLEAN ,
    CreatedDate DATE,
    
    FOREIGN KEY (FingerPrintID) REFERENCES fingerPrint(fingerPrintID)
);          

 

create table EquipmentEnrollment(
EquipmentId int ,
EquipmentName nvarchar(255),
EquipmentPurchaseDate date ,
EquipmentCount int ,
EquipmentCondition nvarchar(255),
CreatedDate date)   ; 
					


CREATE TABLE Payment (
    PaymentReceiptNo INT AUTO_INCREMENT PRIMARY KEY,
    MemmberId INT,
    Name NVARCHAR(255),
    Service INT,
    BalanceAmount DECIMAL(10,2),
    PaymentAmount DECIMAL(10,2),
    Paymentmode VARCHAR(50),
    collectedby NVARCHAR(255),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATE,
    UpdatedDate DATE
);
 
create table AttendanceTable (
AttendanceId int AUTO_INCREMENT primary key  ,
CandidateId int,
CandidateName nvarchar(255),
fingerPrintID int ,
AttendanceDate date,
InTime time,
OutTime time ,
FOREIGN KEY (fingerPrintID) REFERENCES fingerPrint(fingerPrintID))