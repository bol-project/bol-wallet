using System.Text.Encodings.Web;
using BolWallet.Models.Messages;

namespace BolWallet;
internal class Constants
{
    public const string AppName = "BOL Wallet";
    public const string WelcomeMessage = "Welcome to BOL!";
    public const string BirthDateFormat = "yyyy-MM-dd";
    public const string MainNet = "mainnet";
    public const string TestNet = "testnet";
    
    // Preferences Keys
    public const string TargetNet = "target-net";
    
    public static readonly TargetNetworkChangedMessage TargetNetworkChangedMessage = new();
    
    public static readonly JsonSerializerOptions WalletJsonSerializerDefaultOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
}
