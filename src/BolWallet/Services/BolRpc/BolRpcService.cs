using System.Net.Http.Json;
using Bol.Core.Model;
using Microsoft.Extensions.Logging;
using SimpleResults;

namespace BolWallet.Services.BolRpc;

internal class BolRpcService(HttpClient client, INetworkPreferences networkPreferences, ILogger<BolRpcService> logger) : IBolRpcService
{
    public async Task<Result<string>> GetBolContractHash(CancellationToken token = default) =>
        await PerformRpcRequest<string>(BolRpcMethods.GetBolContractHashRequest, token: token);

    public async Task<Result<BolAccount>> GetBolAccount(string codename, CancellationToken token = default) =>
        await PerformRpcRequest<BolAccount>(BolRpcMethods.GetAccountRequest(codename), token: token);

    private async Task<Result<T>> PerformRpcRequest<T>(
        BolRpcRequest request,
        CancellationToken token = default)
    {
        try
        {
            using var response = await client.PostAsJsonAsync(
                networkPreferences.TargetNetworkConfig.RpcEndpoint,
                request,
                BolRpcConstants.JsonSerializerDefaults,
                token);

            if (!response.IsSuccessStatusCode)
            {
                var result = Result.CriticalError(await response.Content.ReadAsStringAsync(token));
                logger.LogCritical("BOL RPC request error: {BolRpcError}", result.Message);
                return result;
            }

            var responseResult = await response.Content.ReadFromJsonAsync<BolRpcResponse<T>>(token);
            return responseResult.ToResult();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "BOL RPC request error");
            return Result.CriticalError(ex.Message);
        }
    }
}
