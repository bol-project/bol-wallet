using System.Text;
using Bol.Address.Abstractions;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IPermissionService _permissionService;
    private readonly ISecureRepository _secureRepository;
    private readonly IExportKeyFactory _exportKeyFactory;
    private readonly ISha256Hasher _sha256;

    public MainViewModel(
        INavigationService navigationService,
        IPermissionService permissionService,
        ISecureRepository secureRepository,
        IExportKeyFactory exportKeyFactory,
        ISha256Hasher sha256)
        : base(navigationService)
    {
        _permissionService = permissionService;
        _secureRepository = secureRepository;
        _exportKeyFactory = exportKeyFactory;
        _sha256 = sha256;
    }

	[RelayCommand]
	private async Task NavigateToCodenameCompanyPage()
	{
        await NavigationService.NavigateTo<CreateCodenameCompanyViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToCodenameIndividualPage()
    {
        await App.Current.MainPage.Navigation.PushAsync(new Views.CitizenshipPage());
    }

    [RelayCommand]
    private async Task ImportYourWallet()
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.MacCatalyst, new[] { "json" } },
                { DevicePlatform.WinUI, new[] { ".json", "application/json" } }
            });

            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = customFileType, PickerTitle = "Import Your Json Wallet"
            });

            if (pickResult == null)
                return;

            var jsonString = File.ReadAllText(pickResult.FullPath);

            var bolWallet =
                JsonSerializer.Deserialize<Bol.Core.Model.BolWallet>(jsonString,
                    Constants.WalletJsonSerializerDefaultOptions);
            
            var passwordPopup = new PasswordPopup();
            await Application.Current.MainPage.ShowPopupAsync(passwordPopup);
            var password = await passwordPopup.TaskCompletionSource.Task;
            
            if (string.IsNullOrEmpty(password)) return;

            var codeNameAccount = bolWallet.accounts.Single(account => account.Label == "codename");
            var codeNameKey = await Task.Run(() => _exportKeyFactory.GetDecryptedPrivateKey(
                codeNameAccount.Key,
                password,
                bolWallet.Scrypt.N,
                bolWallet.Scrypt.R,
                bolWallet.Scrypt.P));
            
            var expectedCodeNameKey = _sha256.Hash(Encoding.ASCII.GetBytes(bolWallet.Name));

            if (!codeNameKey.SequenceEqual(expectedCodeNameKey))
            {
                throw new Exception("Incorrect Password. Please provide a valid password.");
            }

            var userData = new UserData { Codename = bolWallet.Name, BolWallet = bolWallet, WalletPassword = password };

            await _secureRepository.SetAsync("userdata", userData);

            await NavigationService.NavigateTo<MainWithAccountViewModel>(true);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }
}
