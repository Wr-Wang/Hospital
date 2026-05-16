namespace Hospital.Application.Constants;

/// <summary>权限标识常量，用于角色权限控制和菜单过滤</summary>
public static class Permissions
{
    public const string ShellUse = "sys.shell.use";               // 登录使用系统
    public const string SecurityManage = "sys.security.manage";   // 用户与角色管理

    public const string CampusManage = "mdm.campus.manage";       // 院区管理
    public const string DeptManage = "mdm.dept.manage";           // 科室管理
    public const string StaffManage = "mdm.staff.manage";         // 人员管理
    public const string DictManage = "mdm.dict.manage";           // 字典管理

    public const string PatientRegister = "pat.register";          // 患者建档
    public const string PatientSearch = "pat.search";             // 患者检索

    public const string Schedule = "opd.schedule";                // 排班号表
    public const string RegisterWork = "opd.register";            // 挂号工作台
    public const string Encounter = "opd.encounter";              // 门诊医生站

    public const string Dispense = "pha.dispense";                // 发药工作台
    public const string Cashier = "fin.cash";                     // 收费工作台
}
