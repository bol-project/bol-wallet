using SimpleResults;

namespace BolWallet.Services.Abstractions;

public interface IBolChallengeService
{
    Task<Result> CompleteChallenge(string challenge, string signature, string publicKey, string codename, CancellationToken token = default);
}
