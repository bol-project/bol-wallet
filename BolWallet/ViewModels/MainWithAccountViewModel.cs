﻿using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using System.Collections.ObjectModel;

namespace BolWallet.ViewModels;
public partial class MainWithAccountViewModel : BaseViewModel
{
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	public string WelcomeText => "Welcome";
	public string BalanceText => "Total Balance";
    public string AccountText => "Account";
	public string SendText => "Transfer";
	public string CommunityText => "Bol Community";

	[ObservableProperty]
	public List<KeyValuePair<string, string>> _commercialBalances = new();

	[ObservableProperty]
	public ObservableCollection<BalanceDisplayItem> _commercialBalancesDisplayList = new();

	[ObservableProperty]
	private string _codeName = "";

	[ObservableProperty]
	private string _mainAddress = "";

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	private bool _isLoading = false;

	[ObservableProperty]
	private bool _isAccountOpen = false;

	[ObservableProperty]
	private bool _isNotRegistered = true;

    [ObservableProperty]
    private bool _isCommercialAddressesExpanded = false;

    public MainWithAccountViewModel(
		INavigationService navigationService,
		ISecureRepository secureRepository,
		IBolService bolService) : base(navigationService)
	{
		_secureRepository = secureRepository;
		_bolService = bolService;
	}

	public async Task Initialize()
	{
		await FetchBolAccountData();

		await GenerateCommercialBalanceDisplayList();
	}

    [RelayCommand]
    private async Task FetchBolAccountData()
    {
        try
        {
            userData = await _secureRepository.GetAsync<UserData>("userdata");

            CodeName = userData.Codename;
            MainAddress = userData.BolWallet.accounts?.FirstOrDefault(a => a.Label == "main").Address;

            BolAccount = await _bolService.GetAccount(userData.Codename);

            IsNotRegistered = false;

            if (BolAccount.AccountStatus == AccountStatus.Open)
                IsAccountOpen = true;
            else
                await NavigationService.NavigateTo<CertifyViewModel>(true);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    private async Task GenerateCommercialBalanceDisplayList()
	{
		try
		{
			CommercialBalances = BolAccount?.CommercialBalances?.ToList() ?? new();

			CommercialBalancesDisplayList.Clear();

			foreach (var commercialBalance in CommercialBalances)
			{
				CommercialBalancesDisplayList.Add(new BalanceDisplayItem(address: commercialBalance.Key, balance: commercialBalance.Value));
			}
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}

	[RelayCommand]
	private async Task Register()
	{
		try
		{
			IsLoading = true;

			await Task.Delay(100);

			BolAccount = await _bolService.Register();

			await Toast.Make("Your Account is registered now.").Show();
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

    [RelayCommand]
    private void ExpandCommercialAddress()
    {
       IsCommercialAddressesExpanded = !IsCommercialAddressesExpanded;
    }

    [RelayCommand]
	private async Task NavigateToTransactionsPage()
	{
		await NavigationService.NavigateTo<TransactionsViewModel>(true);
	}

	[RelayCommand]
	private async Task NavigateToBolCommunityPage()
	{
		await NavigationService.NavigateTo<BolCommunityViewModel>(true);
	}

	[RelayCommand]
	private async Task NavigateToFinancialTransactionsPage()
	{
		await NavigationService.NavigateTo<FinancialTransactionsViewModel>(true);
	}

	[RelayCommand]
	private async Task NavigateToCertifierPage()
	{
		await NavigationService.NavigateTo<CertifierViewModel>(true);
	}

	[RelayCommand]
	private async Task NavigateToAccountPage()
	{
		await NavigationService.NavigateTo<AccountViewModel>(true);
	}
}


