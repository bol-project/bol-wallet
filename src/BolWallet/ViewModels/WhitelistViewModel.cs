using Bol.Address;
using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;
public partial class WhitelistViewModel : BaseViewModel
{
	private readonly IBolService _bolService;
	private readonly IAddressTransformer _addressTransformer;

	public WhitelistViewModel(
		INavigationService navigationService,
		IBolService bolService,
		IAddressTransformer addressTransformer) : base(navigationService)
	{
		_bolService = bolService;
		_addressTransformer = addressTransformer;
	}

	[ObservableProperty]
	private string _codeNameToCertify = "";

	[ObservableProperty]
	private string _mainAddressToWhitelist = "";

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	private bool _isLoading = false;

	[RelayCommand]
	private async Task Whitelist()
	{
		try
		{
			if (string.IsNullOrEmpty(MainAddressToWhitelist))
				throw new Exception("Please Select a Main Address");

			IsLoading = true;

			await Task.Delay(100);

			var isWhitelisted = await _bolService.Whitelist(_addressTransformer.ToScriptHash(MainAddressToWhitelist));

			await Toast.Make($"Main Address {MainAddressToWhitelist} is whitelisted now.").Show();
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
		}
		finally
		{
			IsLoading = false;
			MainAddressToWhitelist = string.Empty;
		}
	}

}
