using Bol.Core.Abstractions;

namespace BolWallet.Services.Abstractions;

public interface IAppCaching : ICachingService
{
    void Remove(string key);
}
