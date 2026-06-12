namespace Hospital.Application.DTOs;

public sealed record WeChatLoginRequest(string Code);

public sealed record WeChatLoginResult(
    string? TempToken,       // 需要创建患者时非空
    int? ExpiresIn,
    string? AccessToken,     // 已有绑定账号时非空
    string? RefreshToken,
    long? PatientId,
    string? PatientNo,       // 病历号
    string? Name,            // 患者姓名
    string? Phone,           // 患者手机号
    bool IsNew               // true = 需要创建新患者
);

public sealed record WeChatAuthResult(
    string AccessToken,
    string RefreshToken,
    long PatientId,
    string PatientNo,
    string Name,
    string? Phone,
    bool IsNew
);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record RefreshTokenResult(string AccessToken, string RefreshToken);

public sealed record PatientProfileResult(
    long PatientId,
    string PatientNo,
    string Name,
    string? Phone
);
