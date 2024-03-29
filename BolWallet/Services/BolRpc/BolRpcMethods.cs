namespace BolWallet.Services.BolRpc;

internal static class BolRpcMethods
{
    private const string GetBolHash = "getbolhash";
    private const string GetAccount = "getAccount";
    
    internal static readonly BolRpcRequest GetBolContractHashRequest = new(GetBolHash, []);
    internal static BolRpcRequest GetAccountRequest(string codename) => new(GetAccount, [codename]);
}
