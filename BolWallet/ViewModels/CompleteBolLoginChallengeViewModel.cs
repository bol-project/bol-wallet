using Bol.Core.Abstractions;
using Bol.Cryptography;
using Bol.Cryptography.Abstractions;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public partial class CompleteBolLoginChallengeViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _challenge = "";

    private readonly IContextAccessor _contextAccessor;
    private readonly IECCurveSigner _signer;
    private readonly IBase64Encoder _base64Encoder;
    private readonly IBolChallengeService _bolChallengeService;

    public CompleteBolLoginChallengeViewModel(
        INavigationService navigationService,
        IContextAccessor contextAccessor,
        IECCurveSigner signer,
        IBase64Encoder base64Encoder,
        IBolChallengeService bolChallengeService) : base(navigationService)
    {
        _contextAccessor = contextAccessor;
        _signer = signer;
        _base64Encoder = base64Encoder;
        _bolChallengeService = bolChallengeService;
    }

    [RelayCommand]
    private async Task CompleteBolChallenge(CancellationToken token = default)
    {
        var context = _contextAccessor.GetContext();
        var keyPair = context.SocialAddress.Value;
        var publicKeyBytes = keyPair.PublicKey.ToBytes();

        var challenge = _base64Encoder.Decode(Challenge);
        var signature = _signer.Sign(challenge, keyPair.PrivateKey, publicKeyBytes);
        
        var signatureString = _base64Encoder.Encode(signature);
        var publicKey = _base64Encoder.Encode(publicKeyBytes);
        
        var result = await _bolChallengeService.CompleteChallenge(Challenge, signatureString, publicKey, context.CodeName, token);
        if (result.IsFailed)
        {
            await Toast.Make(result.Message).Show(token);
            return;
        }

        await Toast.Make("BoL Login Challenge completed!").Show(token);
        await NavigationService.NavigateTo<MainWithAccountViewModel>(true);
    }
}
