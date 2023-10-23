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
	private string _certifierCodename = string.Empty;

	[ObservableProperty]
	private bool _isLoading = false;

	[ObservableProperty]
	private int _certifications = 0;

	[ObservableProperty]
	private bool _isCertified = false;

	[ObservableProperty]
	private BolAccount _bolAccount = new();

	[ObservableProperty]
	public Dictionary<string, string> _mandatoryCertifiers;

	private UserData userData;

	public async Task Initialize()
	{
		userData = await _secureRepository.GetAsync<UserData>("userdata");

		await UpdateBolAccount();
	}

	[RelayCommand]
	private async Task UpdateBolAccount()
	{
		try
		{
			BolAccount = await _bolService.GetAccount(userData.Codename);

			if (BolAccount.AccountStatus == AccountStatus.PendingFees)
			{
				IsCertified = true;
				return;
			}

			if (BolAccount.AccountStatus == AccountStatus.PendingCertifications)
			{
				Certifications = BolAccount.Certifications + 1;
				MandatoryCertifiers = BolAccount.MandatoryCertifiers;
			}
		}
		catch (Exception ex)
		{
			await Toast.Make(ex.Message).Show();
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

			await Toast.Make("Certifiers selected for this certification round").Show();
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

			CertifierCodename = string.Empty;

			await Toast.Make("Certification request sent successfully.").Show();

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

	[RelayCommand]
	private async Task PayCertificationFees()
	{
		try
		{
			IsLoading = true;

			await Task.Delay(100);

			BolAccount bolAccount = await _bolService.PayCertificationFees();

			userData.AccountStatus = BolAccount.AccountStatus;

			await Task.Run(async () => await _secureRepository.SetAsync("userdata", userData));

			IsLoading = false;

			await NavigationService.NavigateTo<MainWithAccountViewModel>(true);
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
