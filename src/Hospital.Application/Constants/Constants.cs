namespace Hospital.Application.Constants;

/// <summary>导航路由键常量，与后端路由注册一一对应</summary>
public static class RouteKeys
{
    // ===== 首页 =====
    public const string Home = "shell.home";

    // ===== M1 主数据 =====
    public const string Campus = "mdm.campus";
    public const string Department = "mdm.dept";
    public const string Staff = "mdm.staff";
    public const string Dictionary = "mdm.dict";

    // ===== M2 患者 =====
    public const string PatientRegister = "pat.register";
    public const string PatientSearch = "pat.search";
    public const string Patient360 = "pat.360";

    // ===== M3 挂号 =====
    public const string Schedule = "opd.schedule";
    public const string RegisterWorkbench = "opd.register";

    // ===== M5 门诊 =====
    public const string Encounter = "opd.encounter";

    // ===== M6 发药 =====
    public const string Dispense = "pha.dispense";

    // ===== M11 收费 =====
    public const string Cashier = "fin.cash";

    // ===== M13 系统 =====
    public const string UserRole = "sys.userrole";
}

/// <summary>API 路由路径常量，对应后端 Controller 路由</summary>
public static class ApiRoutes
{
    public const string AuthenticationLogin = "Authentication/login";

    public static class Patient
    {
        public const string Base = "patient";
        public static string ById(long id) => $"patient/{id}";
        public static string ByPatientNo(string patientNo) => $"patient/by-patient-no/{patientNo}";
        public static string ByIdCard(string idCard) => $"patient/by-idcard/{idCard}";
        public const string SuspectDuplicates = "patient/suspect-duplicates";
        public static string Search(string? keyword, int page, int size) =>
            $"patient/search?keyword={keyword}&page={page}&size={size}";
        public static string Profile(long id) => $"patient/{id}/profile";
    }

    public static class Schedule
    {
        public const string Base = "schedule";
        public static string ById(long id) => $"schedule/{id}";
        public static string ByDoctor(long doctorId) => $"schedule/by-doctor/{doctorId}";
        public static string ByDept(long deptId, string? date) => $"schedule/by-dept/{deptId}?date={date}";
        public static string Available(long deptId, long? doctorId, string date) =>
            $"schedule/available?deptId={deptId}&doctorId={doctorId}&date={date}";
        public static string Publish(long id) => $"schedule/{id}/publish";
        public static string Deactivate(long id) => $"schedule/{id}/deactivate";
        public static string SlotQuota(long id) => $"schedule/{id}/slot-quota";
    }

    public static class Registration
    {
        public const string Base = "registration";
        public static string ById(long id) => $"registration/{id}";
        public static string ByPatient(long patientId) => $"registration/by-patient/{patientId}";
        public static string ByDoctor(long doctorId, string? date) =>
            $"registration/by-doctor/{doctorId}?date={date}";
        public static string Void(long id) => $"registration/{id}/void";
    }

    public static class Encounter
    {
        public const string Base = "encounter";
        public static string Queue(long doctorId, string date) =>
            $"encounter/queue?doctorId={doctorId}&date={date}";
        public static string StartConsultation(long id) => $"encounter/{id}/start";
        public static string CompleteConsultation(long id) => $"encounter/{id}/complete";
    }

    public static class MedicalRecord
    {
        public const string Base = "medical-record";
        public static string ByEncounter(long encounterId) => $"medical-record/by-encounter/{encounterId}";
        public static string Save(long encounterId) => $"medical-record/{encounterId}";
    }

    public static class Diagnosis
    {
        public const string Base = "diagnosis";
        public static string ByEncounter(long encounterId) => $"diagnosis/by-encounter/{encounterId}";
        public static string ById(long id) => $"diagnosis/{id}";
    }

    public static class Prescription
    {
        public const string Base = "prescription";
        public static string ByEncounter(long encounterId) => $"prescription/by-encounter/{encounterId}";
        public static string ById(long id) => $"prescription/{id}";
        public static string Void(long id) => $"prescription/{id}/void";
    }

    public static class LabOrder
    {
        public const string Base = "lab-order";
        public static string ByEncounter(long encounterId) => $"lab-order/by-encounter/{encounterId}";
        public static string ById(long id) => $"lab-order/{id}";
        public static string Cancel(long id) => $"lab-order/{id}/cancel";
    }

    public static class RadOrder
    {
        public const string Base = "rad-order";
        public static string ByEncounter(long encounterId) => $"rad-order/by-encounter/{encounterId}";
        public static string ById(long id) => $"rad-order/{id}";
        public static string Cancel(long id) => $"rad-order/{id}/cancel";
    }

    public static class Cashier
    {
        public const string Base = "cashier";
        public static string PendingItems(long patientId) => $"cashier/pending-items/{patientId}";
        public const string Pay = "cashier/pay";
    }

    public static class Dispense
    {
        public const string Base = "dispense";
        public static string PaidPrescriptions(long patientId) => $"dispense/paid-prescriptions/{patientId}";
        public static string DispenseItem(long id) => $"dispense/{id}/dispense";
        public static string ReturnItem(long id) => $"dispense/{id}/return";
    }

    public static class User
    {
        public const string Base = "user";
        public static string ById(long id) => $"user/{id}";
    }

    public static class Role
    {
        public const string Base = "role";
        public static string ById(long id) => $"role/{id}";
    }
}

/// <summary>JWT Token 声明键名常量</summary>
public static class JwtClaims
{
    public const string CampusName = "campus_name";
    public const string Permissions = "permissions";
}
