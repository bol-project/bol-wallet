namespace BolWallet.Services;

public class NetworkPreferences(
    IPreferences preferences,
    [FromKeyedServices(Constants.MainNet)] BolWalletAppConfig mainNetConfig,
    [FromKeyedServices(Constants.TestNet)] BolWalletAppConfig testNetConfig) : INetworkPreferences
{
    private const string TargetNetworkKey = "TargetNetwork";
    public bool IsMainNet => preferences.Get(TargetNetworkKey, Constants.MainNet) == Constants.MainNet;
    public string Name => IsMainNet ? Constants.MainNet : Constants.TestNet;
    public string AlternativeName => IsMainNet ? Constants.TestNet : Constants.MainNet;
    public BolWalletAppConfig TargetNetworkConfig => IsMainNet ? mainNetConfig : testNetConfig;
    public void SwitchNetwork() =>  preferences.Set(TargetNetworkKey, AlternativeName);
}
