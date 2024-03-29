using System.Net.Http.Json;
using Bol.Core.Model;
using SimpleResults;

namespace BolWallet.Services;

internal readonly record struct BolRpcRequest(
    string Method,
    string[] Params,
    string JsonRpcVersion = "2.0",
    int Id = 1);

internal readonly record struct BolRpcErrorResult(int Code, string Message);

internal readonly record struct BolRpcResponse<T>(T Result, BolRpcErrorResult? Error);

internal static class BolRpcResponseExtensions
{
    internal static Result<T> ToResult<T>(this BolRpcResponse<T> response) => response switch
    {
        { Error: not null } => Result.NotFound([
            $"BoL RPC Error Code: {response.Error.Value.Code}",
            $"BoL RPC Error Message: {response.Error.Value.Message}"
        ]),
        { Result: null } => Result.CriticalError("No result found"),
        { Result: not null } => Result.ObtainedResource(response.Result)
    };
}

internal class BolRpcService(HttpClient client) : IBolRpcService
{
    private static JsonSerializerOptions JsonSerializerDefaults => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
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

            var responseResult = await response.Content.ReadFromJsonAsync<BolRpcResponse<T>>(token);
            return responseResult.ToResult();
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
}
