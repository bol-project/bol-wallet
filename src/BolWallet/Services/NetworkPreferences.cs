using BolWallet.Models.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace BolWallet.Services;

public class NetworkPreferences(
    IPreferences preferences,
    BolWalletAppConfig mainNetConfig,
    BolWalletAppConfig testNetConfig) : INetworkPreferences
{
    private BolWalletAppConfig _mainNetConfig = mainNetConfig;
    private BolWalletAppConfig _testNetConfig = testNetConfig;
    
    private const string TargetNetworkKey = "TargetNetwork";
    public bool IsMainNet => preferences.Get(TargetNetworkKey, Constants.MainNet) == Constants.MainNet;

    public string Name => IsMainNet ? Constants.MainNet : Constants.TestNet;
    public string AlternativeName => IsMainNet ? Constants.TestNet : Constants.MainNet;
    public BolWalletAppConfig TargetNetworkConfig => IsMainNet ? _mainNetConfig : _testNetConfig;

    public void SwitchNetwork()
    {
        preferences.Set(TargetNetworkKey, AlternativeName);
        WeakReferenceMessenger.Default.Send<TargetNetworkChangedMessage>();
    }

    public void SetBolContractHash(string value)
    {
        if (IsMainNet)
        {
            _mainNetConfig = _mainNetConfig with { Contract = value };
        }
        
        _testNetConfig = _testNetConfig with { Contract = value };
    }
}
