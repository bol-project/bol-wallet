namespace BolWallet.Models;

public readonly record struct BolWalletAppConfig(
    string RpcEndpoint,
    string BolIdentityEndpoint,
    string BolExplorerEndpoint,
    string BolCertifierEndpoint,
    string Contract);
