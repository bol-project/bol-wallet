using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Bol.Core.Model;
using SimpleResults;

namespace BolWallet.Services;

internal class BolRpcService(HttpClient client) : IBolRpcService
{
    private readonly record struct BolRpcErrorResult(int Code, string Message);

    private static JsonSerializerOptions JsonSerializerDefaults => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly record struct BolRpcRequest(
        string Method,
        string[] Params,
        string JsonRpcVersion = "2.0",
        int Id = 1);

    private static readonly BolRpcRequest s_getBolContractHashRequest = new("getbolhash", []);
    private static BolRpcRequest s_getAccountRequest(string codename) => new("getAccount", [codename]);

    
    public async Task<Result<string>> GetBolContractHash(CancellationToken token = default) =>
        await PerformRpcRequest<string>(s_getBolContractHashRequest, token: token);

    public async Task<Result<BolAccount>> GetBolAccount(string codename, CancellationToken token = default) =>
        await PerformRpcRequest<BolAccount>(s_getAccountRequest(codename), token: token);

    private async Task<Result<T>> PerformRpcRequest<T>(
        BolRpcRequest request,
        string resultKey = "result",
        string errorKey = "error",
        CancellationToken token = default)
    {
        try
        {
            using var response = await client.PostAsJsonAsync(
                null as string,
                request,
                JsonSerializerDefaults,
                token);

            if (!response.IsSuccessStatusCode)
            {
                return Result.CriticalError(await response.Content.ReadAsStringAsync(token));
            }

            var json = await response.Content.ReadAsStringAsync(token);
            var jsonNode = JsonNode.Parse(json);
            var error = jsonNode![errorKey];
            if (error is not null)
            {
                (int code, string message) = error.Deserialize<BolRpcErrorResult>(JsonSerializerDefaults);
                return Result.NotFound([
                    $"BoL RPC Error Code: {code}",
                    $"BoL RPC Error Message: {message}"
                ]);
            }

            var result = jsonNode[resultKey];
            return result is null ?
                Result.CriticalError("No result found") :
                Result.ObtainedResource(result.GetValue<T>());
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
}
