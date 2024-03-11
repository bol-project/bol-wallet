using SimpleResults;

namespace BolWallet.Services.Abstractions;

public interface IBolContractHashService
{
    Task<Result<string>> GetBolContractHash(CancellationToken token = default);
}
