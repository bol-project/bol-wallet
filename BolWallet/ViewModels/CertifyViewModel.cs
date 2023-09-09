using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;
public partial class CertifyViewModel : BaseViewModel
{
	private readonly ISecureRepository _secureRepository;
	private readonly IBolService _bolService;

	public CertifyViewModel(
		INavigationService navigationService,
		ISecureRepository secureRepository,
		IBolService bolService) : base(navigationService)
	{
		_secureRepository = secureRepository;
		_bolService = bolService;
	}

	[ObservableProperty]
	private string _certifierCodename = "";

	[ObservableProperty]
	private bool _isLoading = false;

	[ObservableProperty]
	private int _certifications = 0;

	[ObservableProperty]
	public Dictionary<string, string> _mandatoryCertifiers;

	public async Task Initialize()
	{
		var userData = await _secureRepository.GetAsync<UserData>("userdata");

		if (userData is null) return;

		BolAccount bolAccount = await _bolService.GetAccount(userData.Codename);

		if (bolAccount.AccountStatus == AccountStatus.PendingCertifications)
		{
			Certifications = bolAccount.Certifications + 1;
			MandatoryCertifiers = bolAccount.MandatoryCertifiers;
		}
	}

	[RelayCommand]
	private async Task SelectMandatoryCertifiers()
	{
		try
		{
			IsLoading = true;

			await Task.Delay(100);

			BolAccount bolAccount = await _bolService.SelectMandatoryCertifiers();

			MandatoryCertifiers = bolAccount.MandatoryCertifiers;
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
	private async Task RequestCertification()
	{
		try
		{
			if (string.IsNullOrEmpty(CertifierCodename))
				throw new Exception("Please Select Certifier");

			IsLoading = true;

			await Task.Delay(100);

			BolAccount bolAccount = await _bolService.RequestCertification(CertifierCodename);

			IsLoading = false;
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
