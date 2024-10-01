namespace BolWallet.Services.Abstractions;

public interface INetworkPreferences
{
    public bool IsMainNet { get; }
    public string Name { get; }
    public string AlternativeName { get; }
    public BolWalletAppConfig TargetNetworkConfig { get; }
    public void SwitchNetwork();
}
