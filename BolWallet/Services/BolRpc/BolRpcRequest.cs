namespace BolWallet.Services.BolRpc;

internal readonly record struct BolRpcRequest(
    string Method,
    string[] Params,
    string JsonRpcVersion = "2.0",
    int Id = 1);
