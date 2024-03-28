using Bol.Core.Model;
using SimpleResults;

namespace BolWallet.Services.Abstractions;

public interface IBolRpcService
{
    Task<Result<string>> GetBolContractHash(CancellationToken token = default);

    Task<Result<BolAccount>> GetBolAccount(string codename, CancellationToken token = default);
}
