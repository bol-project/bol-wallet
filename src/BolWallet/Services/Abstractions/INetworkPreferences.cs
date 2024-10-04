namespace BolWallet.Services.Abstractions;

public interface INetworkPreferences
{
    public string Name { get; }
    public bool IsMainNet { get; }
    public string AlternativeName { get; }
    public BolWalletAppConfig TargetNetworkConfig { get; }
    public void SwitchNetwork();
    public void SetBolContractHash(string value);
}
