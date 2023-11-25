using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolWallet.ViewModels
{
	public partial class FinancialTransactionsViewModel : BaseViewModel
	{
		private readonly IBolService _bolService;
		private readonly ISecureRepository _secureRepository;

		[ObservableProperty]
		private bool _isLoading = false;

		[ObservableProperty]
		private BolAccount _bolAccount = new();

		public FinancialTransactionsViewModel(
			INavigationService navigationService,
			IBolService bolService,
			ISecureRepository secureRepository) : base(navigationService)
		{
			_bolService = bolService;
			_secureRepository = secureRepository;
		}

		public async Task Initialize()
		{
			try
			{
				userData = await _secureRepository.GetAsync<UserData>("userdata");

				if (userData is null) return;

				BolAccount = await Task.Run(async () => await _bolService.GetAccount(userData.Codename));
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
        public async Task Claim()
		{
			try
			{
				IsLoading = true;

				await Task.Delay(100);

				BolAccount = await _bolService.Claim();
			}
			catch (Exception ex)
			{
				await Toast.Make(ex.Message).Show();
			}
			finally
			{
				IsLoading = false;
			}
		}
	}
}
