namespace Hospital.Domain.Enums;

/// <summary>
/// 科室类型（与数据库 mdm.Departments.DeptType 列的值一致）
/// </summary>
public enum DepartmentType
{
    Admin = 1,
    Clinical = 2,
    Lab = 3,
    Radiology = 4,
    Pharmacy = 5
}
