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

    public async Task<Result<string>> GetBolContractHash(CancellationToken token = default)
    {
        try
        {
            using var response = await client.PostAsJsonAsync(
                null as string,
                s_getBolContractHashRequest,
                JsonSerializerDefaults,
                token);

            if (!response.IsSuccessStatusCode)
            {
                return Result.CriticalError(await response.Content.ReadAsStringAsync(token));
            }

            var json = await response.Content.ReadAsStringAsync(token);
            var jsonNode = JsonNode.Parse(json);
            var error = jsonNode!["error"];
            if (error is not null)
            {
                var (code, message) = error.Deserialize<BolRpcErrorResult>(JsonSerializerDefaults);
                return Result.NotFound([
                    $"BoL RPC Error Code: {code}",
                $"BoL RPC Error Message: {message}"
                ]);
            }

            var result = jsonNode["result"];
            if (result is null)
            {
                return Result.CriticalError("No result found");
            }

            return Result.ObtainedResource(result.GetValue<string>());
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }

    public async Task<Result<BolAccount>> GetBolAccount(string codename, CancellationToken token = default)
    {
        try
        {
            using var response = await client.PostAsJsonAsync(
                null as string,
                s_getAccountRequest(codename),
                JsonSerializerDefaults,
                token);

            if (!response.IsSuccessStatusCode)
            {
                return Result.CriticalError(await response.Content.ReadAsStringAsync(token));
            }

            var json = await response.Content.ReadAsStringAsync(token);
            var jsonNode = JsonNode.Parse(json);
            var error = jsonNode!["error"];
            if (error is not null)
            {
                var (code, message) = error.Deserialize<BolRpcErrorResult>(JsonSerializerDefaults);
                return Result.NotFound([
                    $"BoL RPC Error Code: {code}",
                    $"BoL RPC Error Message: {message}"
                ]);
            }

            var result = jsonNode["result"];
            return result is null
                ? Result.CriticalError("No result found")
                : Result.ObtainedResource(result.GetValue<BolAccount>());
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
}
