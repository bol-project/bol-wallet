using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using System.Collections.ObjectModel;
using System.Numerics;

namespace BolWallet.ViewModels;
public partial class SendBolViewModel : BaseViewModel
{
	public string AmountText => "";

	[ObservableProperty]
	public string _result;

	[ObservableProperty]
	private SendBolForm _sendBolForm;

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	private BolAccount _searchBolAccount;

    [ObservableProperty]
    private string _searchCodename;

    [ObservableProperty]
	public List<KeyValuePair<string, string>> _commercialBalances;

	[ObservableProperty]
	public List<string> _commercialBalancesDisplayList;

    [ObservableProperty]
    public ObservableCollection<BalanceDisplayItem> _searchCommercialBalancesDisplayList = new();

    [ObservableProperty]
	public int? _selectedCommercialAddressIndex;

	private readonly IAddressTransformer _addressTransformer;
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	public SendBolViewModel(
		INavigationService navigationService,
		IAddressTransformer addressTransformer,
		ISecureRepository secureRepository,
		IBolService bolService) : base(navigationService)
	{
		_addressTransformer = addressTransformer;
		_secureRepository = secureRepository;
		_bolService = bolService;

		SendBolForm = new SendBolForm();
	}

	public async Task Initialize()
	{
		try
		{
			userData = await _secureRepository.GetAsync<UserData>("userdata");

			if (userData is null) return;

			BolAccount = await Task.Run(async () => await _bolService.GetAccount(userData.Codename));

			GenerateCommercialBalanceDisplayList();
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}

	private void GenerateCommercialBalanceDisplayList()
	{
		CommercialBalances = BolAccount.CommercialBalances.ToList();

		CommercialBalancesDisplayList = CommercialBalances.Select(i => "Balance: " + i.Value + " - " + i.Key).ToList();
	}

	[RelayCommand]
	private async Task SendBol()
	{
		try
		{
			if (!SelectedCommercialAddressIndex.HasValue)
				throw new Exception("Please choose a commercial address for this transfer.");

			SendBolForm.ComAddress = CommercialBalances[SelectedCommercialAddressIndex.Value].Key;

			BolAccount bolAccount = await _bolService.Transfer(
			  _addressTransformer.ToScriptHash(SendBolForm.ComAddress.Trim()),
			  _addressTransformer.ToScriptHash(SendBolForm.ReceiverAddress.Trim()),
			  SendBolForm.ReceiverCodename,
			  new BigInteger(SendBolForm.ActualAmount * (decimal)Math.Pow(10, 8)));

			GenerateCommercialBalanceDisplayList();

			Result = "From Address: " + SendBolForm.ComAddress +
						"\nReceiver Codename: " + SendBolForm.ReceiverCodename +
						"\nReceiver Address: " + SendBolForm.ReceiverAddress +
						"\nAmount: " + SendBolForm.ActualAmount;

			SendBolForm.Amount = "";

			await Toast.Make(Result).Show();
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}

    [RelayCommand]
    private async Task FetchBolAccountData()
    {
        try
        {
            if (string.IsNullOrEmpty(SearchCodename))
                return;

            SearchBolAccount = await _bolService.GetAccount(SearchCodename);

			var searchCommercialBalances = SearchBolAccount?.CommercialBalances?.ToList() ?? new(); 

            CommercialBalancesDisplayList.Clear();

            foreach (var commercialBalance in searchCommercialBalances)
            {
                SearchCommercialBalancesDisplayList.Add(new BalanceDisplayItem(address: commercialBalance.Key, balance: commercialBalance.Value));
            }
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    [RelayCommand]
    private async Task SelectCommercialAddress(string commercialAddress)
    {
        try
        {
            SearchBolAccount = await _bolService.GetAccount(SearchCodename);

            SendBolForm.ReceiverAddress = commercialAddress;

            SendBolForm.ReceiverCodename = SearchBolAccount.CodeName;

            SearchCommercialBalancesDisplayList.Clear();

            GenerateCommercialBalanceDisplayList();
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }
}
