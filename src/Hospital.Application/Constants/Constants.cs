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
}

/// <summary>JWT Token 声明键名常量</summary>
public static class JwtClaims
{
    public const string CampusName = "campus_name";
    public const string Permissions = "permissions";
}
