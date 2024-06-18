namespace BolWallet.Services.BolRpc;

internal static class BolRpcConstants
{
    internal static readonly JsonSerializerOptions JsonSerializerDefaults = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
