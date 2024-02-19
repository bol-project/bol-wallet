using Bol.Core.Model;

namespace BolWallet.Models;

public class BolWalletAppConfig : BolConfig
{
    public string BolIdentityEndpoint { get; set; } = "";
}
