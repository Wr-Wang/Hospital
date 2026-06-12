using System.Collections.Generic;

namespace Hospital.App.Constants;

/// <summary>WPF 应用常量：前端特有硬编码值集中管理</summary>
public static class AppConstants
{
    // ===== 分页 =====
    public const int SearchPageSize = 20;

    // ===== 身份证号 =====
    public const int IdCardFullLength = 18;

    // ===== 默认显示值 =====
    public const string NullDisplay = "--";
    public const string Separator = " | ";

    // ===== 占位文本 =====
    public const string NoVisitHistory = "暂无就诊记录";
    public const string NoPrescription = "暂无处方记录";
    public const string NoLabReport = "暂无检查报告";
    public const string PatientNotFound = "未找到该患者";
}

/// <summary>性别中文 ↔ 英文枚举值映射</summary>
public static class GenderMapper
{
    private static readonly Dictionary<string, string> ChineseToEnglish = new()
    {
        ["男"] = "Male",
        ["女"] = "Female",
        ["其他"] = "Other"
    };

    private static readonly Dictionary<string, string> EnglishToChinese = new()
    {
        ["Male"] = "男",
        ["Female"] = "女",
        ["Other"] = "其他"
    };

    /// <summary>中文显示值转英文枚举（供 API 调用）</summary>
    public static string? ToApiValue(string? chinese) =>
        chinese is not null && ChineseToEnglish.TryGetValue(chinese, out var english) ? english : null;

    /// <summary>英文枚举转中文显示值</summary>
    public static string? ToDisplayValue(string? english) =>
        english is not null && EnglishToChinese.TryGetValue(english, out var chinese) ? chinese : english;

    /// <summary>获取所有中文选项列表</summary>
    public static List<string> DisplayOptions => new() { "男", "女", "其他" };
}
