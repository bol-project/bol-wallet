using SimpleResults;

namespace BolWallet.Services.BolRpc;

internal static class Extensions
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
