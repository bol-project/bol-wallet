using System.Text.Encodings.Web;

namespace BolWallet;
internal class Constants
{
    public static readonly JsonSerializerOptions WalletJsonSerializerDefaultOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
}
