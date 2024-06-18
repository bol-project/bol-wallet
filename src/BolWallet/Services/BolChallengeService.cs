using System.Net.Http.Json;
using SimpleResults;

namespace BolWallet.Services;

public class BolChallengeService(HttpClient httpClient) : IBolChallengeService
{
    public async Task<Result> CompleteChallenge(string challenge, string signature, string publicKey, string codename,
        CancellationToken token = default)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("challenge", new
            {
                challenge,
                signature,
                publicKey,
                codename
            }, token);

            if (!response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Result>(token);
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.CriticalError(e.Message);
        }
    }
}
