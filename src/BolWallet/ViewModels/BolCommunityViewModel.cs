using Bol.Core.Abstractions;
using BolWallet.Pages;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;
public partial class BolCommunityViewModel : BaseViewModel
{
    public string BolCommunityHeaderText => "Bol Community";
    public string CertifyText => "Certify";
    public string DeleteFakeAccountText => "Delete Face Account";
    public string DeleteExpiredAccountText => "Delete Expired Account";
    public string VoteText => "Vote";
    public string SignMessageText => "Sign Message";
    public string BolIdentityText => "Bol Identity";

    [ObservableProperty] private bool _isCertifier;
    
    private readonly IBolService _bolService;
    private readonly ISecureRepository _repo;

    public BolCommunityViewModel(INavigationService navigationService, IBolService bolService, ISecureRepository repo) : base(navigationService)
    {
        _bolService = bolService;
        _repo = repo;
    }
    
    [RelayCommand]
    public async Task Initialize()
    {
        try
        {
            userData = await _repo.GetAsync<UserData>("userdata");
            IsCertifier = userData.IsCertifier;
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    [RelayCommand]
    private async Task NavigateToGetCertificationsPage()
    {
        await NavigationService.NavigateTo<GetCertifiedViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToWhitelistAndCertifyPage()
    {
        await NavigationService.NavigateTo<WhitelistViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToRegisterAsCertifierPage()
    {
        await App.Current.Windows[0].Page.Navigation.PushAsync(new Views.RegisterAsCertifierPage());
    }

    [RelayCommand]
    private async Task NavigateToAddMultiCitizenship()
    {
        await App.Current.Windows[0].Page.Navigation.PushAsync(new Views.AddMultiCitizenship());
    }

    [RelayCommand]
    private async Task UnRegisterAsCertifier(CancellationToken token)
    {
        try
        {
            await _bolService.UnRegisterAsCertifier(token);
            await NavigationService.NavigateTo<MainWithAccountViewModel>(true);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }
    
    [RelayCommand]
    private async Task CompleteBolLoginChallenge()
    {
        await App.Current.Windows[0].Page.Navigation.PushAsync(new Views.CompleteBolLoginChallengePage());
    }
}
