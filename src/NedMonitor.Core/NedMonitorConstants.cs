/// <summary>
/// Contains constant keys and default sensitive terms used throughout the NedMonitor logging system.
/// </summary>
public static class NedMonitorConstants
{
    /// <summary>
    /// Key used to store exception information in the HttpContext.
    /// </summary>
    public const string CONTEXT_EXCEPTION_KEY = "NedMonitor_Exception";

    /// <summary>
    /// Key used to store the response body in the HttpContext.
    /// </summary>
    public const string CONTEXT_REPONSE_BODY_KEY = "NedMonitor_ResponseBody";

    /// <summary>
    /// Key used to store the size of the response body in the HttpContext.
    /// </summary>
    public const string CONTEXT_REPONSE_BODY_SIZE_KEY = "NedMonitor_ResponseBodySize";

    /// <summary>
    /// Key used to store notifications generated during the request lifecycle.
    /// </summary>
    public const string CONTEXT_NOTIFICATIONS_KEY = "__Notifications__";

    /// <summary>
    /// Key used to store HTTP client logs collected during the request.
    /// </summary>
    public const string CONTEXT_HTTP_CLIENT_LOGS_KEY = "__HttpClientLogs__";

    /// <summary>
    /// Key used to store internal logger adapter messages.
    /// </summary>
    public const string CONTEXT_LOGS_KEY = "__NedMonitor_Logger_Adapter_";

    /// <summary>
    /// Default sensitive keys to be masked or excluded from logs and monitoring payloads.
    /// Includes both English and Portuguese terms commonly used for credentials, tokens, secrets, and authentication.
    /// </summary>
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
