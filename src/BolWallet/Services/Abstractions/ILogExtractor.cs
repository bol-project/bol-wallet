namespace BolWallet.Services.Abstractions;

public interface ILogExtractor
{
    Task ExtractLog(CancellationToken token = default);
}
