using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using System.Numerics;

namespace BolWallet.ViewModels;
public partial class MoveClaimViewModel : BaseViewModel
{
	public string ToComAddressText => "To my Commercial Address";
	public string AmountText => "Amount";

	private readonly IAddressTransformer _addressTransformer;
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	[ObservableProperty]
	private MoveClaimForm _moveClaimForm;

	[ObservableProperty]
	public string _result;

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	public List<KeyValuePair<string, string>> _commercialBalances;

	[ObservableProperty]
	public List<string> _commercialBalancesDisplayList;

	[ObservableProperty]
	public int? _selectedCommercialAddressIndex;

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

		CommercialBalancesDisplayList = CommercialBalances.Select(i => "Balance: " + i.Value + " - " + i.Key).ToList();
	}

	[RelayCommand]
	private async Task MoveClaim()
	{
		try
		{
            if (!SelectedCommercialAddressIndex.HasValue || SelectedCommercialAddressIndex < 0)
                throw new Exception("Please choose a commercial address for this transfer.");

            MoveClaimForm.ComAddress = CommercialBalances[SelectedCommercialAddressIndex.Value].Key;

			BolAccount = await _bolService.TransferClaim(
			  _addressTransformer.ToScriptHash(MoveClaimForm.ComAddress.Trim()),
			  new BigInteger(MoveClaimForm.ActualAmount * (decimal)Math.Pow(10, 8)));

			GenerateCommercialBalanceDisplayList();

			Result = "To Address: " + MoveClaimForm.ComAddress +
					 "\nAmount: " + MoveClaimForm.ActualAmount;

			MoveClaimForm.Amount = "";

			await Toast.Make(Result).Show();
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
	}
}
