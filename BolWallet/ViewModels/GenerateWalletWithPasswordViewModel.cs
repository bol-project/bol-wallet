using Bol.Core.Abstractions;
using Bol.Cryptography;
using System.Text;

namespace BolWallet.ViewModels;
public partial class GenerateWalletWithPasswordViewModel : BaseViewModel
{
    private readonly IWalletService _walletService;
    private readonly ISecureRepository _secureRepository;
    private readonly ISha256Hasher _sha256Hasher;
    private readonly IBase16Encoder _base16Encoder;

    public GenerateWalletWithPasswordViewModel(
        INavigationService navigationService,
        IWalletService walletService,
        ISecureRepository secureRepository,
        ISha256Hasher sha256Hasher,
        IBase16Encoder base16Encoder) : base(navigationService)
    {
        _walletService = walletService;
        _secureRepository = secureRepository;
        _sha256Hasher = sha256Hasher;
        _base16Encoder = base16Encoder;
    }

    [ObservableProperty]
    private string _password = "";


    [ObservableProperty]
    private bool _isLoading = false;

    [RelayCommand]
    private async Task Submit()
    {
        if (string.IsNullOrEmpty(Password))
            return;

        IsLoading = true;

        byte[] hash = _sha256Hasher.Hash(Encoding.UTF8.GetBytes(Password));

        string privateKey = _base16Encoder.Encode(hash);

        UserData userData = await this._secureRepository.GetAsync<UserData>("userdata");

		var bolWallet = await this._walletService.CreateWallet(Password, userData.Codename, userData.Edi, privateKey);

        userData.BolWallet = bolWallet;

        await Clipboard.SetTextAsync(JsonSerializer.Serialize(bolWallet));

        IsLoading = false;
    }
}