using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using System.Numerics;

namespace BolWallet.ViewModels;
public partial class MoveClaimViewModel : BaseViewModel
{
	public string MoveClaimLabel => "Move Claim";
	public string ToComAddressText => "To my Commercial Address";
	public string AmountText => "Amount";
	public string MoveText => "Move";


	private readonly IAddressTransformer _addressTransformer;
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	[ObservableProperty]
	private MoveClaimForm _moveClaimForm;

	[ObservableProperty]
	public string _resultLabel;

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	public List<KeyValuePair<string, string>> _commercialBalances;

	[ObservableProperty]
	public List<string> _commercialBalancesDisplayList;

	[ObservableProperty]
	public int _selectedCommercialAddressIndex;

	public MoveClaimViewModel(INavigationService navigationService,
		IAddressTransformer addressTransformer,
		ISecureRepository secureRepository,
		IBolService bolService) : base(navigationService)
	{
		MoveClaimForm = new MoveClaimForm();

		_addressTransformer = addressTransformer;
		_secureRepository = secureRepository;
		_bolService = bolService;
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

		CommercialBalancesDisplayList = CommercialBalances.Select(i => i.Key + " - Balance: " + i.Value).ToList();
	}

	[RelayCommand]
	private async Task MoveClaim()
	{
		try
		{
			MoveClaimForm.ComAddress = CommercialBalances[SelectedCommercialAddressIndex].Key;

			BolAccount = await _bolService.TransferClaim(
			  _addressTransformer.ToScriptHash(MoveClaimForm.ComAddress),
			  new BigInteger(MoveClaimForm.Amount));

			GenerateCommercialBalanceDisplayList();

			ResultLabel = "To Address: " + MoveClaimForm.ComAddress +
						  "\nAmount: " + MoveClaimForm.Amount;
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}
}
