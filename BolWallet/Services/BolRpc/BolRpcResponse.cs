namespace BolWallet.Services.BolRpc;

internal readonly record struct BolRpcResponse<T>(T Result, BolRpcErrorResult? Error);
