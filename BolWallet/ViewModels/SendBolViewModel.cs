using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Core.Services;
using CommunityToolkit.Maui.Alerts;
using System.Numerics;

namespace BolWallet.ViewModels;
public partial class SendBolViewModel : BaseViewModel
{
	private UserData _userData;

	public string SendBolLabel => "Send Bol";

	public string FromAddressText => "From Address:";
	public string ReceiverAddressText => "Receiver Address:";
	public string ReceiverCodenameText => "Receiver Codename:";
	public string AmountText => "";

	[ObservableProperty]
	public string _resultLabel;

	[ObservableProperty]
	private SendBolForm _sendBolForm;

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	public List<KeyValuePair<string,string>> _commercialBalances;

	[ObservableProperty]
	public List<string> _displayList;

	[ObservableProperty]
	public int _selectedIndex;

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

			CommercialBalances = BolAccount.CommercialBalances.ToList();

			DisplayList = CommercialBalances.Select(i => i.Key + " - Balance: " + i.Value).ToList();
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}

	[RelayCommand]
	private async Task SendBol()
	{
		try
		{
			SendBolForm.ComAddress = CommercialBalances[SelectedIndex].Key;

			BolAccount bolAccount = await _bolService.Transfer(
			  _addressTransformer.ToScriptHash(SendBolForm.ComAddress),
			  _addressTransformer.ToScriptHash(SendBolForm.ReceiverAddress),
			  SendBolForm.ReceiverCodename,
			  new BigInteger(SendBolForm.Amount));


			ResultLabel = "From Address: " + SendBolForm.ComAddress +
						"\nReceiver Codename: " + SendBolForm.ReceiverCodename +
						"\nReceiver Address: " + SendBolForm.ReceiverAddress +
						"\nAmount: " + SendBolForm.Amount;
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}
}
