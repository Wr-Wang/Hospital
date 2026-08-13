using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hospital.Infrastructure.ExternalServices;

public sealed class WeChatSessionResult
{
    public string? Openid { get; set; }
    public string? SessionKey { get; set; }
    public string? Unionid { get; set; }
    public int? Errcode { get; set; }
    public string? Errmsg { get; set; }
}

public sealed class WeChatPhoneResult
{
    public string? PhoneNumber { get; set; }
    public string? PurePhoneNumber { get; set; }
    public string? CountryCode { get; set; }
}

public sealed class WeChatApiException : Exception
{
    public WeChatApiException(string message) : base(message) { }
}

public sealed class WeChatHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeChatHttpClient> _logger;
    private readonly string _appId;
    private readonly string _appSecret;

    public WeChatHttpClient(IConfiguration configuration, HttpClient httpClient, ILogger<WeChatHttpClient> logger)
    {
        _appId = configuration["WeChat:MiniProgram:AppId"]!;
        _appSecret = configuration["WeChat:MiniProgram:AppSecret"]!;
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri("https://api.weixin.qq.com/");
    }

    /// <summary>code 换 session_key 和 openid</summary>
    public async Task<WeChatSessionResult> Code2SessionAsync(string code)
    {
        var url = $"/sns/jscode2session?appid={_appId}&secret={_appSecret}&js_code={code}&grant_type=authorization_code";

        var sw = Stopwatch.StartNew();
        _logger.LogInformation("步骤2.1: 调用微信 code2session (js_code={Code})", code);

        WeChatSessionResult? response;
        try
        {
            response = await _httpClient.GetFromJsonAsync<WeChatSessionResult>(url);
            sw.Stop();
            _logger.LogInformation("步骤2.2: code2session 响应 errcode={Errcode}, 耗时={ElapsedMs}ms",
                response?.Errcode, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "步骤2.3: code2session 调用异常/超时，耗时={ElapsedMs}ms", sw.ElapsedMilliseconds);
            throw;
        }

        if (response is null)
            throw new WeChatApiException("code2session 响应为空");

        if (response.Errcode.HasValue && response.Errcode != 0)
        {
            var message = response.Errcode switch
            {
                40029 => "登录凭证已失效，请重新登录（wx.login 一次性 code 已过期或被使用）",
                _ => $"微信登录失败 (errcode={response.Errcode}): {response.Errmsg}"
            };
            throw new WeChatApiException(message);
        }

        if (string.IsNullOrEmpty(response.Openid))
            throw new WeChatApiException("code2session 未返回 openid");

        return response;
    }

    /// <summary>解密微信加密数据获取手机号</summary>
    public WeChatPhoneResult DecryptPhoneNumber(string encryptedData, string iv, string sessionKey)
    {
        var aesKey = Convert.FromBase64String(sessionKey);
        var aesIv = Convert.FromBase64String(iv);
        var cipherText = Convert.FromBase64String(encryptedData);

        using var aes = Aes.Create();
        aes.Key = aesKey;
        aes.IV = aesIv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
        var plainJson = Encoding.UTF8.GetString(plainBytes);

        var result = System.Text.Json.JsonSerializer.Deserialize<WeChatPhoneResult>(plainJson);
        return result ?? throw new WeChatApiException("解密手机号结果为空");
    }
}
