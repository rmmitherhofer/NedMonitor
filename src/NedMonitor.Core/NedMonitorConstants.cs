namespace NedMonitor.Core;

public static class NedMonitorConstants
{
    public const string CONTEXT_EXCEPTION_KEY = "NedMonitor_Exception";
    public const string CONTEXT_REPONSE_BODY_KEY = "NedMonitor_ResponseBody";
    public const string CONTEXT_REPONSE_BODY_SIZE_KEY = "NedMonitor_ResponseBodySize";
    public const string CONTEXT_NOTIFICATIONS_KEY = "__Notifications__";
    public const string CONTEXT_HTTP_CLIENT_LOGS = "__HttpClientLogs__";

    public static readonly List<string> DEFAULT_KEYS =
    [
        "password", "senha",
        "token", "access_token", "refresh_token",
        "jwt", "jwe", "jws", "jwk", "jwa", "jwm",
        "auth", "authentication", "authorization", "autenticacao", "autorizacao",
        "secret", "client_secret", "api_key", "secret_key", "private_key",
        "assinatura", "signature", "segredo",
        "pin", "otp", "mfa_code", "codigo_mfa"
    ];
}
