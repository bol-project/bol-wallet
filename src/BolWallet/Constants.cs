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
    public const string DefaultHash = "0000000000000000000000000000000000000000000000000000000000000000";

    // Preferences Keys
    public const string TargetNet = "target-net";
    
    // Static messages to re-use
    public static readonly TargetNetworkChangedMessage TargetNetworkChangedMessage = new();
    public static readonly WalletClosedMessage WalletClosedMessage = new();
    public static readonly WalletCreatedMessage WalletCreatedMessage = new();
    public static readonly WalletCleanupCompletedMessage WalletCleanupCompletedMessage = new();
    public static readonly SaveLogfileMessage SaveLogfileMessage = new();
    
    public static readonly JsonSerializerOptions WalletJsonSerializerDefaultOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
}
