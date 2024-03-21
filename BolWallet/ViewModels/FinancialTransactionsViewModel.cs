using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels
{
    public partial class FinancialTransactionsViewModel : BaseViewModel
    {
        private readonly IBolService _bolService;
        private readonly ISecureRepository _secureRepository;

        public FinancialTransactionsViewModel(
            INavigationService navigationService,
            IBolService bolService,
            ISecureRepository secureRepository)
            : base(navigationService)
        {
            _bolService = bolService;
            _secureRepository = secureRepository;
        }

        [ObservableProperty]
        private bool _isCompanyAccount = false;

        [RelayCommand]
        public async Task Initialize()
        {
            try
            {
                userData = await _secureRepository.GetAsync<UserData>("userdata");

                if (userData is null) return;

                BolAccount bolAccount = await Task.Run(async () => await _bolService.GetAccount(userData.Codename));

                IsCompanyAccount = bolAccount.CodeName.StartsWith("C");
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
        }

        [RelayCommand]
        private async Task NavigateToTransferPage()
        {
            await NavigationService.NavigateTo<SendBolViewModel>(true);
        }

        [RelayCommand]
        private async Task NavigateToTransferClaimPage()
        {
            await NavigationService.NavigateTo<MoveClaimViewModel>(true);
        }

        [RelayCommand]
        private async Task Claim()
        {
            await NavigationService.NavigateTo<ClaimViewModel>(true);
        }
    }
}
