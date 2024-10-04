using BolWallet.Models.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace BolWallet.Services;

public class NetworkPreferences(
    IPreferences preferences,
    BolWalletAppConfig mainNetConfig,
    BolWalletAppConfig testNetConfig,
    ILogger<NetworkPreferences> logger) : INetworkPreferences
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
        logger.LogInformation("Switching target network from {Name} to {AlternativeName}", Name, AlternativeName);

        preferences.Set(TargetNetworkKey, AlternativeName);
    }

    public void SetBolContractHash(string value)
    {
        logger.LogInformation("Received BOL Contract Hash for {TargetNetwork}: {BolContractHash}", Name, value);
        
        if (IsMainNet)
        {
            _mainNetConfig = _mainNetConfig with { Contract = value };
        }
        
        _testNetConfig = _testNetConfig with { Contract = value };
    }
}
